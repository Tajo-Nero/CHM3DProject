// PlayerCarMode.cs 최적화 버전
using UnityEngine;
using System.Collections.Generic;

public class PlayerCarMode : MonoBehaviour
{
    [Header("이동 설정")]
    public float rotationSpeed = 100f;
    public float moveSpeed = 3f;
    public float sprintSpeed = 5f;

    [Header("카메라 설정")]
    public Transform camPos;
    public Camera cam;
    public float cameraRotationX = 45f;

    [Header("드릴 설정")]
    [SerializeField] private float digDistance = 3f;
    public float digRadius = 3f;
    public float digDepth = 0.2f;
    public float sprintDigDepth = 0.4f;

    [Header("참조")]
    public GameObject _PlayerMode;
    private GameManager gameManager;

    // 성능 최적화
    private float lastDigTime;
    private float digCooldown = 0.05f; // 초당 20회 제한

    // 경로 기록 시스템
    private List<Vector3> playerPath = new List<Vector3>();
    private float pathRecordDistance = 2f;
    private Vector3 lastRecordedPosition;
    private bool isRecording = false;

    // 캐싱된 참조값
    private Vector3 moveDir;
    private Rigidbody rb;
    private TerrainManager terrainManager;

    void Start()
    {
        // 필요한 컴포넌트 초기화
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        gameManager = FindObjectOfType<GameManager>();
        terrainManager = FindObjectOfType<TerrainManager>();

        // 경로 기록 초기화
        lastRecordedPosition = transform.position;
        playerPath.Add(transform.position);
        isRecording = true;

        // 디버그 로그
        Debug.Log("플레이어 카 모드 시작");
    }

    void Update()
    {
        HandleMovement();
        HandleDrilling();

        // F5 키 입력 시 오브젝트 파괴 및 Terrain 리셋
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ResetLevel();
        }
    }

    void LateUpdate()
    {
        // 카메라 위치 및 회전 업데이트
        UpdateCamera();
    }

    private void HandleMovement()
    {
        // 입력 값 받기
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 이동 처리
        if (verticalInput != 0)
        {
            // 스프린트 확인
            bool isSprinting = Input.GetMouseButton(0);
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

            // 이동 방향 계산
            moveDir = transform.forward * verticalInput * currentSpeed * Time.deltaTime;
            transform.position += moveDir;

            // 경로 기록
            if (isRecording && moveDir.magnitude > 0)
            {
                RecordPlayerPath();
            }

            // 좌우 회전 처리
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
            }

            // 드릴 깊이 조정
            digDepth = isSprinting ? sprintDigDepth : 0.3f;
        }
    }

    private void HandleDrilling()
    {
        // 쿨다운 기반 드릴링
        if (Time.time - lastDigTime >= digCooldown)
        {
            PerformDig();
            lastDigTime = Time.time;
        }
    }

    private void UpdateCamera()
    {
        // 카메라 위치 업데이트
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
            cam.transform.rotation = Quaternion.Euler(cameraRotationX, transform.rotation.eulerAngles.y, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Nexus"))
        {
            // 경로 저장
            SavePlayerPath();

            // 플레이어 모드 전환
            gameManager.SpawnPlayer(_PlayerMode);
            Destroy(gameObject);
        }
    }

    private void SavePlayerPath()
    {
        if (playerPath.Count > 0)
        {
            PathManager pathManager = FindObjectOfType<PathManager>();
            if (pathManager != null)
            {
                pathManager.SetMainPath(playerPath);
                Debug.Log($"플레이어 경로 저장: {playerPath.Count}개 지점");
            }
        }
    }

    private void RecordPlayerPath()
    {
        float distance = Vector3.Distance(transform.position, lastRecordedPosition);
        if (distance >= pathRecordDistance)
        {
            playerPath.Add(transform.position);
            lastRecordedPosition = transform.position;
        }
    }

    private void PerformDig()
    {
        // 레이캐스트로 지형 감지
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, digDistance))
        {
            TerrainCollider terrainCollider = hit.collider as TerrainCollider;
            if (terrainCollider != null)
            {
                Dig(terrainCollider, hit.point);
            }
        }
    }

    private void Dig(TerrainCollider terrainCollider, Vector3 hitPoint)
    {
        Terrain terrain = terrainCollider.GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        // 히트 포인트의 Heightmap 좌표 계산
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        // 정수 좌표로 변환
        int xBase = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * heightmapWidth);
        int yBase = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * heightmapHeight);

        // 반경 범위 계산
        int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * heightmapWidth);

        // 디깅에 영향을 받는 지형 범위 계산
        int xMin = Mathf.Max(0, xBase - radius);
        int xMax = Mathf.Min(heightmapWidth - 1, xBase + radius);
        int yMin = Mathf.Max(0, yBase - radius);
        int yMax = Mathf.Min(heightmapHeight - 1, yBase + radius);

        // 최적화: 한 번에 영향 받는 전체 영역의 높이 데이터 가져오기
        int width = xMax - xMin + 1;
        int height = yMax - yMin + 1;
        float[,] heights = terrainData.GetHeights(xMin, yMin, width, height);

        // 높이 수정
        bool modified = false;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int xCoord = xMin + x;
                int yCoord = yMin + y;

                float distX = (xCoord - xBase) / (float)heightmapWidth * terrainData.size.x;
                float distY = (yCoord - yBase) / (float)heightmapHeight * terrainData.size.z;
                float distance = Mathf.Sqrt(distX * distX + distY * distY);

                if (distance <= digRadius)
                {
                    float depthFactor = Mathf.Lerp(1, 0, distance / digRadius); // 거리 비례 깊이
                    float newHeight = Mathf.Max(0, heights[y, x] - digDepth * depthFactor);

                    if (heights[y, x] != newHeight)
                    {
                        heights[y, x] = newHeight;
                        modified = true;
                    }
                }
            }
        }

        // 변경된 경우에만 높이 데이터 업데이트
        if (modified)
        {
            terrainData.SetHeights(xMin, yMin, heights);
        }
    }

    private void ResetLevel()
    {
        Destroy(gameObject);
        if (terrainManager != null)
        {
            terrainManager.ResetTerrain();
        }
    }

    private void OnDrawGizmos()
    {
        // 드릴 시각화
        Gizmos.color = Color.red;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * digDistance;
        Gizmos.DrawLine(startPosition, endPosition);

        // 드릴 범위 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(endPosition, digRadius);
    }
}