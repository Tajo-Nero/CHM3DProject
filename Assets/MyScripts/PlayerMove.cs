using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    Vector3 moveDir;
    float moveSpeed = 5;
    float jumpPow = 4;

    public float mouseX;
    public float mouseY;
    public Transform camPos;
    public float camRotSpeed = 1;
    public Camera cam;
    public Transform target;
    Transform tr;
    public NavMeshSurface navMeshSurface;
    //[SerializeField] GameObject _Obj;
    //[SerializeField] private TerrainData terrainData;
    //[SerializeField] private int resolution;//터레인 크기
    //[SerializeField] private float scale = 1f; //설정할 터레인 사이즈,이건 PerlinNoise 노이즈= 굴곡 설정할떄 쓰임 
    //[SerializeField] private float heightScale = 1.5f; // 터레인 높이
                                                       //public float digDistance = 1.0f;  // 땅을 팔 거리
                                                       //void SetTerrainHeights()//터레인 생성 함수
                                                       //{
                                                       //terrainData.heightmapResolution = resolution;
                                                       //terrainData.size = new Vector3(resolution, heightScale, resolution);
                                                       //float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];


    //// 지형 데이터의 현재 해상도를 가져오기
    ////int currentResolution = terrainData.heightmapResolution;

    //// 새로운 해상도로 설정
    ////if (currentResolution != resolution)
    ////{
    ////    terrainData.heightmapResolution = resolution;
    ////}

    //// 지형 데이터의 크기를 설정
    ////terrainData.size = new Vector3(resolution, heightScale, resolution);

    //// 새로운 해상도에 맞는 높이 배열 생성
    ////float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

    ////for 문이라 0,0 에서부터 시작해서 x,y 0 좌표값 부터 시작됨 
    ////0,0 에서 resolution에 넣어준 값 까지 현재 100,100 까지 설정된 높이까지 올라오게함 
    //for (int x = 1; x < terrainData.heightmapResolution-1; x++)
    //{
    //for (int y = 1; y < terrainData.heightmapResolution-1; y++)
    //{
    ////굴곡 설정할때 쓰이는 노이즈 함수 PerlinNoise(너비,너비) 넣어주면 
    ////heights[x, y] = perlin * heightScale; 너비 * 높이 설정된 높이 값만큼 위로 올라가면서 선형보간되서 굴곡이 완성됨
    ////반대로 다음 너비까지 내려오면서 내려오는 굴곡 형성 나는 그냥 생성만하면 됨으로 안써도 됨 
    ////float perlin = Mathf.PerlinNoise((float)x / resolution * scale, (float)y / resolution * scale);
    ////heights[x, y] = resolution * heightScale;
    //heights[y, x] = heightScale / terrainData.size.y;
    //}

    //terrainData.SetHeights(0, 0, heights);
    //}
    //// 새로운 해상도로 설정
    ////terrainData.heightmapResolution = resolution;

    //// 지형 데이터의 크기를 설정
    ////terrainData.size = new Vector3(resolution, heightScale, resolution);

    //// 새로운 해상도에 맞는 높이 배열 생성
    ////int terrainWidth = terrainData.heightmapResolution;
    ////int terrainHeight = terrainData.heightmapResolution;
    ////float[,] heights = new float[terrainHeight, terrainWidth];
    ////
    ////// 높이 배열 설정 (모든 높이 값을 일정한 높이로 설정)
    ////for (int y = 1; y < terrainHeight-1; y++)
    ////{
    ////    for (int x = 1; x < terrainWidth-1; x++)
    ////    {
    ////        heights[y, x] = heightScale / terrainData.size.y;
    ////    }
    ////}

    //// 높이 값을 설정
    //terrainData.SetHeights(0, 0, heights);

    //}
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

    public void LowerTerrainHeight(int x, int z)//터레인 낮추는 함수 
    {

        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);
        heights[x, z] -= 0.1f;

        terrainData.SetHeights(0, 0, heights);
    }


    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();

    }
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X");
        mouseY -= Input.GetAxis("Mouse Y");
        cam.transform.rotation = Quaternion.Euler(mouseY * camRotSpeed, mouseX * camRotSpeed, 0);
        transform.rotation = Quaternion.Euler(0, mouseX * camRotSpeed, 0);

        PlayermoveMent();

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
    void PlayermoveMent()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 전후좌우 이동 방향 벡터 계산
        moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Translate(이동 방향 * 속력 * Time.deltaTime)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        animator.SetFloat("Move", moveDir.magnitude);

    }

    void OnAttack()
    {
        animator.SetBool("IsHammer", true);
    }
    void OnJump()
    {
        rb.AddForce(Vector3.up * jumpPow, ForceMode.Impulse);
        animator.SetBool("IsJump", true);
    }


    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }

}