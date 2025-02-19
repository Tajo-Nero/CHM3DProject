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
    Transform tr;
    
    
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private int resolution = 32;//터레인 크기
    //[SerializeField] private float scale = 1f; //설정할 터레인 사이즈,이건 PerlinNoise 노이즈= 굴곡 설정할떄 쓰임 
    [SerializeField] private float heightScale = 1.5f; // 터레인 높이
    //public float digDistance = 1.0f;  // 땅을 팔 거리
    void SetTerrainHeights()//터레인 생성 함수
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(resolution, heightScale, resolution);
        float[,] heights = new float[resolution, resolution];

        //for 문이라 0,0 에서부터 시작해서 x,y 0 좌표값 부터 시작됨 
        //0,0 에서 resolution에 넣어준 값 까지 현재 100,100 까지 설정된 높이까지 올라오게함 
        for (int x = 1; x < resolution; x++)
        {
            for (int y = 1; y < resolution; y++)
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

   
    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }
    
}