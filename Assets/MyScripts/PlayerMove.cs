using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpPower = 4f;
    [SerializeField] private float camRotSpeed = 1f;
    [SerializeField] private Transform camPos;
    private Animator animator;
    private Rigidbody rb;
    private Transform playerTransform;

    [Header("Camera Settings")]
    private Camera cam;
    private float mouseX, mouseY;

    [Header("NavMesh Settings")]
    public Transform target;
    public NavMeshSurface navMeshSurface;

    [Header("Terrain Settings")]
    [SerializeField] GameObject _Obj;
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private int resolution = 65; // 유효한 해상도 값
    [SerializeField] private float heightScale = 1.5f; // 터레인 높이
    [SerializeField] private float terrainWidth = 200f; // 지형 너비
    [SerializeField] private float terrainLength = 200f; // 지형 길이
    

    void SetTerrainHeights() // 터레인 생성 함수
    {
        // 유효한 해상도로 설정
        terrainData.heightmapResolution = resolution;

        // 지형 데이터의 크기를 설정 (너비와 길이 조정)
        terrainData.size = new Vector3(terrainWidth, heightScale, terrainLength);

        // 해상도에 맞는 높이 배열 생성
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        // 높이 배열 설정 (모든 높이 값을 일정한 높이로 설정)
        for (int y = 2; y < terrainData.heightmapResolution - 2; y++)
        {
            for (int x = 2; x < terrainData.heightmapResolution - 2; x++)
            {
                heights[y, x] = heightScale / terrainData.size.y;              
                
            }
        }
        // 높이 값을 스무딩
        SmoothTerrain(heights);
        // 높이 값을 설정
        terrainData.SetHeights(0, 0, heights);
        // Terrain 컴포넌트를 가져와 Flush 호출
        Terrain terrain = _Obj.GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.Flush();  // 지형 데이터를 강제로 업데이트하여 변화를 적용합니다.

        }

    }
    void SmoothTerrain(float[,] heights)
    {
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);

        for (int i = 0; i < 5; i++) // 스무딩 반복 횟수
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    // 주변 높이 값들의 평균을 계산하여 현재 높이 값에 설정
                    float avgHeight = (heights[y - 1, x] + heights[y + 1, x] + heights[y, x - 1] + heights[y, x + 1]) / 4f;
                    heights[y, x] = avgHeight;
                }
            }
        }
    }


    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();

    }
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleJump();
        cam.transform.position = camPos.position;

        if (Input.GetMouseButton(1))
        {
            animator.SetBool("IsHammer", false);
        }        
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetTerrainHeights();
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            _Obj.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            BakeNavMesh();
        }


    }
    void BakeNavMesh()
    {
        // NavMesh 다시 베이크
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh가 다시 베이크되었습니다.");
    }  

    void OnAttack()
    {
        animator.SetBool("IsHammer", true);
    }
    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        playerTransform.Translate(moveDir * moveSpeed * Time.deltaTime);
        animator.SetFloat("Move", moveDir.magnitude);
    }
    private void HandleRotation()
    {
        mouseX += Input.GetAxis("Mouse X") * camRotSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * camRotSpeed;
        cam.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        playerTransform.rotation = Quaternion.Euler(0, mouseX, 0);
    }
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("IsJump", true);
        }


    }

    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }

}