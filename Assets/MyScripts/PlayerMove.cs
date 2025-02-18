using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

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
    //public float digDistance = 1.0f;  // 땅을 팔 거리

    Transform tr;


    [SerializeField] private TerrainData terrainData;
    [SerializeField] private int resolution = 30;//터레인 크기
    //[SerializeField] private float scale = 1f; //설정할 터레인 사이즈,이건 PerlinNoise 노이즈= 굴곡 설정할떄 쓰임 
    [SerializeField] private float heightScale = 1.5f; // 터레인 높이
    void SetTerrainHeights()//터레인 생성 함수
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(resolution, heightScale, resolution);
        float[,] heights = new float[resolution, resolution];

        //for 문이라 0,0 에서부터 시작해서 x,y 0 좌표값 부터 시작됨 
        //0,0 에서 resolution에 넣어준 값 까지 현재 100,100 까지 설정된 높이까지 올라오게함 
        for (int x = 2; x < resolution; x++)
        {
            for (int y = 2; y < resolution; y++)
            {
                //굴곡 설정할때 쓰이는 노이즈 함수 PerlinNoise(너비,너비) 넣어주면 
                //heights[x, y] = perlin * heightScale; 너비 * 높이 설정된 높이 값만큼 위로 올라가면서 선형보간되서 굴곡이 완성됨
                //반대로 다음 너비까지 내려오면서 내려오는 굴곡 형성 나는 그냥 생성만하면 됨으로 안써도 됨 
                //float perlin = Mathf.PerlinNoise((float)x / resolution * scale, (float)y / resolution * scale);
                heights[x, y] = resolution * heightScale;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
    public void LowerTerrainHeight(int x, int z)//터레인 낮추는 함수 
    {

        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        //int x = (int)transform.localPosition.x *2;
        //int y = (int)transform.localPosition.z *2 ;
        heights[x, z] -= 0.1f;


        //heights[2, 8] -= 0.1f;             

        //함수 실행시 0.1 만큼 감소 
        //for (int x = 0; x < width; x++)
        //{
        //    for (int y = 0; y < height; y++)
        //    {
        //        heights[x, y] -= 0.1f;
        //       
        //    }
        //}

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

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IsHammer", false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetTerrainHeights();
        }    

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
    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Floor")
    //    {            
    //        animator.SetBool("IsJump",false);
    //        Debug.Log("d");
    //        LowerTerrainHeight();
    //        //terrainData.heightmapResolution
    //    }
    //    
    //}
    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }
    //void OnCollisionEnter(Collision collision)
    //{
    //    // 충돌된 오브젝트가 터레인인지 확인합니다.
    //    Terrain terrain = collision.collider.GetComponent<Terrain>();
    //    if (terrain != null)
    //    {
    //        // 터레인 데이터에 접근하여 정보를 얻습니다.
    //        TerrainData terrainData = terrain.terrainData;
    //        Vector3 collisionPoint = collision.contacts[0].point;
    //        int x = Mathf.FloorToInt(collisionPoint.x);
    //        int z = Mathf.FloorToInt(collisionPoint.z);
    //        float height = terrainData.GetHeight(x, z);
    //        Vector3 normal = terrainData.GetInterpolatedNormal(x / (float)terrainData.heightmapResolution, z / (float)terrainData.heightmapResolution);
    //        LowerTerrainHeight(x, z);
    //        Debug.Log($"Collision Point: {collisionPoint}, Height: {height}, Normal: {normal}");
    //    }
    //    else
    //    {
    //        Debug.Log("Terrain이 아닙니다.");
    //    }
    //}
    //void OnCollisionStay(Collision collision)
    //{
       
    //    // 충돌된 오브젝트가 터레인인지 확인합니다.
    //    Terrain terrain = collision.collider.GetComponent<Terrain>();
    //    if (terrain != null)
    //    {
    //        // 터레인 데이터에 접근하여 정보를 얻습니다.
    //        TerrainData terrainData = terrain.terrainData;
    //        Vector3 collisionPoint = collision.contacts[0].point;
    
    //        // 충돌 지점의 월드 좌표를 터레인 데이터의 로컬 좌표로 변환합니다.
    //        Vector3 terrainPosition = terrain.transform.position;
    //        int x = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
    //        int z = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
    
    //        // 높이를 줄이기 위해 현재 높이를 가져옵니다.
    //        float[,] heights = terrainData.GetHeights(x, z, 1, 1);
    //        heights[0, 0] = Mathf.Max(0, heights[0, 0] - 0.1f); // 높이를 0.1만큼 줄입니다.
    
    //        // 높이를 업데이트합니다.
    //        terrainData.SetHeights(x, z, heights);
    //        RayTerrain();
    //        Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced at: ({x}, {z})");
    //    }
    //    else
    //    {
    //        Debug.Log("Terrain이 아닙니다.");
    //    }
    //}

    //void RayTerrain()
    //{
    //    Vector3 forward = transform.TransformDirection(Vector3.forward) * digDistance;
    //    Ray ray = new Ray(transform.position, forward);
    //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out RaycastHit hit))
    //    {
    //        // 히트된 오브젝트가 터레인인지 확인합니다.
    //        Terrain terrain = hit.collider.GetComponent<Terrain>();
    //        if (terrain != null)
    //        {
    //            // 터레인 데이터에 접근하여 정보를 얻습니다.
    //            TerrainData terrainData = terrain.terrainData;
    //            Vector3 hitPoint = hit.point;

    //            // 히트된 지점의 월드 좌표를 터레인 데이터의 로컬 좌표로 변환합니다.
    //            Vector3 terrainPosition = terrain.transform.position;
    //            int x = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
    //            int z = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

    //            // 높이를 줄이기 위해 현재 높이를 가져옵니다.
    //            float[,] heights = terrainData.GetHeights(x, z, 1, 1);
    //            heights[0, 0] = Mathf.Max(0, heights[0, 0] - 0.1f); // 높이를 0.1만큼 줄입니다.

    //            // 높이를 업데이트합니다.
    //            terrainData.SetHeights(x, z, heights);

    //            Debug.Log($"Hit Point: {hitPoint}, Terrain Height Reduced at: ({x}, {z})");
    //        }
    //        else
    //        {
    //            Debug.Log("Terrain이 아닙니다.");
    //        }
    //    }
    //}
}