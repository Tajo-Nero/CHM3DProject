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
    //public float digDistance = 1.0f;  // ���� �� �Ÿ�

    Transform tr;


    [SerializeField] private TerrainData terrainData;
    [SerializeField] private int resolution = 30;//�ͷ��� ũ��
    //[SerializeField] private float scale = 1f; //������ �ͷ��� ������,�̰� PerlinNoise ������= ���� �����ҋ� ���� 
    [SerializeField] private float heightScale = 1.5f; // �ͷ��� ����
    void SetTerrainHeights()//�ͷ��� ���� �Լ�
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(resolution, heightScale, resolution);
        float[,] heights = new float[resolution, resolution];

        //for ���̶� 0,0 �������� �����ؼ� x,y 0 ��ǥ�� ���� ���۵� 
        //0,0 ���� resolution�� �־��� �� ���� ���� 100,100 ���� ������ ���̱��� �ö������ 
        for (int x = 2; x < resolution; x++)
        {
            for (int y = 2; y < resolution; y++)
            {
                //���� �����Ҷ� ���̴� ������ �Լ� PerlinNoise(�ʺ�,�ʺ�) �־��ָ� 
                //heights[x, y] = perlin * heightScale; �ʺ� * ���� ������ ���� ����ŭ ���� �ö󰡸鼭 ���������Ǽ� ������ �ϼ���
                //�ݴ�� ���� �ʺ���� �������鼭 �������� ���� ���� ���� �׳� �������ϸ� ������ �Ƚᵵ �� 
                //float perlin = Mathf.PerlinNoise((float)x / resolution * scale, (float)y / resolution * scale);
                heights[x, y] = resolution * heightScale;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
    public void LowerTerrainHeight(int x, int z)//�ͷ��� ���ߴ� �Լ� 
    {

        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);

        //int x = (int)transform.localPosition.x *2;
        //int y = (int)transform.localPosition.z *2 ;
        heights[x, z] -= 0.1f;


        //heights[2, 8] -= 0.1f;             

        //�Լ� ����� 0.1 ��ŭ ���� 
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

        // �����¿� �̵� ���� ���� ���
        moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Translate(�̵� ���� * �ӷ� * Time.deltaTime)
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
    //    // �浹�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
    //    Terrain terrain = collision.collider.GetComponent<Terrain>();
    //    if (terrain != null)
    //    {
    //        // �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
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
    //        Debug.Log("Terrain�� �ƴմϴ�.");
    //    }
    //}
    //void OnCollisionStay(Collision collision)
    //{
       
    //    // �浹�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
    //    Terrain terrain = collision.collider.GetComponent<Terrain>();
    //    if (terrain != null)
    //    {
    //        // �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
    //        TerrainData terrainData = terrain.terrainData;
    //        Vector3 collisionPoint = collision.contacts[0].point;
    
    //        // �浹 ������ ���� ��ǥ�� �ͷ��� �������� ���� ��ǥ�� ��ȯ�մϴ�.
    //        Vector3 terrainPosition = terrain.transform.position;
    //        int x = Mathf.FloorToInt((collisionPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
    //        int z = Mathf.FloorToInt((collisionPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);
    
    //        // ���̸� ���̱� ���� ���� ���̸� �����ɴϴ�.
    //        float[,] heights = terrainData.GetHeights(x, z, 1, 1);
    //        heights[0, 0] = Mathf.Max(0, heights[0, 0] - 0.1f); // ���̸� 0.1��ŭ ���Դϴ�.
    
    //        // ���̸� ������Ʈ�մϴ�.
    //        terrainData.SetHeights(x, z, heights);
    //        RayTerrain();
    //        Debug.Log($"Collision Point: {collisionPoint}, Terrain Height Reduced at: ({x}, {z})");
    //    }
    //    else
    //    {
    //        Debug.Log("Terrain�� �ƴմϴ�.");
    //    }
    //}

    //void RayTerrain()
    //{
    //    Vector3 forward = transform.TransformDirection(Vector3.forward) * digDistance;
    //    Ray ray = new Ray(transform.position, forward);
    //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out RaycastHit hit))
    //    {
    //        // ��Ʈ�� ������Ʈ�� �ͷ������� Ȯ���մϴ�.
    //        Terrain terrain = hit.collider.GetComponent<Terrain>();
    //        if (terrain != null)
    //        {
    //            // �ͷ��� �����Ϳ� �����Ͽ� ������ ����ϴ�.
    //            TerrainData terrainData = terrain.terrainData;
    //            Vector3 hitPoint = hit.point;

    //            // ��Ʈ�� ������ ���� ��ǥ�� �ͷ��� �������� ���� ��ǥ�� ��ȯ�մϴ�.
    //            Vector3 terrainPosition = terrain.transform.position;
    //            int x = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
    //            int z = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

    //            // ���̸� ���̱� ���� ���� ���̸� �����ɴϴ�.
    //            float[,] heights = terrainData.GetHeights(x, z, 1, 1);
    //            heights[0, 0] = Mathf.Max(0, heights[0, 0] - 0.1f); // ���̸� 0.1��ŭ ���Դϴ�.

    //            // ���̸� ������Ʈ�մϴ�.
    //            terrainData.SetHeights(x, z, heights);

    //            Debug.Log($"Hit Point: {hitPoint}, Terrain Height Reduced at: ({x}, {z})");
    //        }
    //        else
    //        {
    //            Debug.Log("Terrain�� �ƴմϴ�.");
    //        }
    //    }
    //}
}