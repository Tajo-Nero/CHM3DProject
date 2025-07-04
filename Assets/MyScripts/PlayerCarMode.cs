using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarMode : MonoBehaviour
{
    public Transform camPos;
    public Camera cam;
    public Transform target;
    public float rotationSpeed = 100f;
    public float moveSpeed = 2f;
    private Vector3 moveDir;
    public GameObject _PlayerMode;
    private GameManager gameManager;

    public float cameraRotationX = 45f; // 카메라 X축 회전 각도
    private GameObject drillSphere;
    [SerializeField]
    private float digDistance = 3f; // 드릴 거리
    public float digRadius = 3f; // 드릴 범위 반경
    public float digDepth = 0.2f; // 드릴 깊이

    // ⭐ 성능 최적화: 드릴링 쿨다운 추가
    private float lastDigTime;
    private float digCooldown = 0.05f; // 0.05초마다 드릴링 (초당 20회로 제한)
    private AutoWaypointGenerator waypointGenerator;
    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // GameManager 인스턴스 찾기
    }

    void Update()
    {
        // 입력 값 받기
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 이동 처리
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;

            // 마우스 왼쪽 버튼 클릭 시 이동 속도 증가
            if (Input.GetMouseButton(0))
            {
                moveSpeed = 5f; // 이동 속도 증가
                digDepth = 0.4f; // 드릴 깊이 증가
            }
            else
            {
                moveSpeed = 3f; // 기본 이동 속도
                digDepth = 0.3f; // 기본 드릴 깊이
            }

            // 좌우 회전 처리
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        // 카메라 회전 및 위치 업데이트
        if (cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(cameraRotationX, transform.rotation.eulerAngles.y, 0); // 카메라 회전 처리
        }

        // F5 키 입력 시 오브젝트 파괴 및 Terrain 리셋
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Destroy(gameObject);
            TerrainManager terrainManager = FindObjectOfType<TerrainManager>();
            if (terrainManager != null)
            {
                terrainManager.ResetTerrain();
            }
        }

        // ⭐ 수정된 부분: 쿨다운을 적용한 드릴링
        if (Time.time - lastDigTime >= digCooldown)
        {
            PerformDig(); // 드릴링 및 지형 파괴 처리
            lastDigTime = Time.time;
        }
    }

    private void LateUpdate()
    {
        // 카메라 위치 업데이트
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Nexus"))
        {
            // 경로 생성!
            if (waypointGenerator != null)
            {
                List<Vector3> generatedPath = waypointGenerator.GenerateWaypoints();
                Debug.Log($"경로 생성 완료! 웨이포인트 {generatedPath.Count}개");

                // PathManager에 경로 저장
                PathManager pathManager = FindObjectOfType<PathManager>();
                if (pathManager != null)
                {
                    pathManager.SetMainPath(generatedPath);
                }
            }

            gameManager.SpawnPlayer(_PlayerMode);
            Destroy(gameObject);
        }
    }

    private void PerformDig()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, digDistance))
        {
            TerrainCollider terrainCollider = hit.collider as TerrainCollider;
            if (terrainCollider != null)
            {
                Dig(terrainCollider, hit.point);
            }
            if (drillSphere != null)
            {
                drillSphere.transform.position = hit.point;
                drillSphere.SetActive(true); // 오브젝트 활성화
            }
        }
        else
        {
            if (drillSphere != null)
            {
                drillSphere.SetActive(false); // 오브젝트 비활성화
            }
        }
    }

    private void Dig(TerrainCollider terrainCollider, Vector3 hitPoint)
    {
        Terrain terrain = terrainCollider.GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        // 히트 포인트의 Heightmap 좌표 계산
        int xBase = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
        int yBase = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

        // 반경 내의 지형 파괴
        int radius = Mathf.FloorToInt(digRadius / terrainData.size.x * terrainData.heightmapResolution);
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int xPos = xBase + x;
                int yPos = yBase + y;

                if (xPos >= 0 && xPos < terrainData.heightmapResolution && yPos >= 0 && yPos < terrainData.heightmapResolution)
                {
                    float distance = Mathf.Sqrt(x * x + y * y);
                    if (distance <= radius)
                    {
                        float[,] heights = terrainData.GetHeights(xPos, yPos, 1, 1);
                        float depthFactor = Mathf.Lerp(1, 0, distance / radius); // 거리 비례 깊이
                        heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // 지형 파괴

                        // 업데이트된 Heightmap를 TerrainData에 적용
                        terrainData.SetHeights(xPos, yPos, heights);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 드릴 레이저를 시각적으로 표시
        Gizmos.color = Color.red;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * digDistance;
        Gizmos.DrawLine(startPosition, endPosition);

        // 히트 포인트를 중심으로 드릴 범위를 시각적으로 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(endPosition, digRadius);
    }
}