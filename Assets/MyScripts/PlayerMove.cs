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
    //[SerializeField] private int resolution;//�ͷ��� ũ��
    //[SerializeField] private float scale = 1f; //������ �ͷ��� ������,�̰� PerlinNoise ������= ���� �����ҋ� ���� 
    //[SerializeField] private float heightScale = 1.5f; // �ͷ��� ����
                                                       //public float digDistance = 1.0f;  // ���� �� �Ÿ�
                                                       //void SetTerrainHeights()//�ͷ��� ���� �Լ�
                                                       //{
                                                       //terrainData.heightmapResolution = resolution;
                                                       //terrainData.size = new Vector3(resolution, heightScale, resolution);
                                                       //float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];


    //// ���� �������� ���� �ػ󵵸� ��������
    ////int currentResolution = terrainData.heightmapResolution;

    //// ���ο� �ػ󵵷� ����
    ////if (currentResolution != resolution)
    ////{
    ////    terrainData.heightmapResolution = resolution;
    ////}

    //// ���� �������� ũ�⸦ ����
    ////terrainData.size = new Vector3(resolution, heightScale, resolution);

    //// ���ο� �ػ󵵿� �´� ���� �迭 ����
    ////float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

    ////for ���̶� 0,0 �������� �����ؼ� x,y 0 ��ǥ�� ���� ���۵� 
    ////0,0 ���� resolution�� �־��� �� ���� ���� 100,100 ���� ������ ���̱��� �ö������ 
    //for (int x = 1; x < terrainData.heightmapResolution-1; x++)
    //{
    //for (int y = 1; y < terrainData.heightmapResolution-1; y++)
    //{
    ////���� �����Ҷ� ���̴� ������ �Լ� PerlinNoise(�ʺ�,�ʺ�) �־��ָ� 
    ////heights[x, y] = perlin * heightScale; �ʺ� * ���� ������ ���� ����ŭ ���� �ö󰡸鼭 ���������Ǽ� ������ �ϼ���
    ////�ݴ�� ���� �ʺ���� �������鼭 �������� ���� ���� ���� �׳� �������ϸ� ������ �Ƚᵵ �� 
    ////float perlin = Mathf.PerlinNoise((float)x / resolution * scale, (float)y / resolution * scale);
    ////heights[x, y] = resolution * heightScale;
    //heights[y, x] = heightScale / terrainData.size.y;
    //}

    //terrainData.SetHeights(0, 0, heights);
    //}
    //// ���ο� �ػ󵵷� ����
    ////terrainData.heightmapResolution = resolution;

    //// ���� �������� ũ�⸦ ����
    ////terrainData.size = new Vector3(resolution, heightScale, resolution);

    //// ���ο� �ػ󵵿� �´� ���� �迭 ����
    ////int terrainWidth = terrainData.heightmapResolution;
    ////int terrainHeight = terrainData.heightmapResolution;
    ////float[,] heights = new float[terrainHeight, terrainWidth];
    ////
    ////// ���� �迭 ���� (��� ���� ���� ������ ���̷� ����)
    ////for (int y = 1; y < terrainHeight-1; y++)
    ////{
    ////    for (int x = 1; x < terrainWidth-1; x++)
    ////    {
    ////        heights[y, x] = heightScale / terrainData.size.y;
    ////    }
    ////}

    //// ���� ���� ����
    //terrainData.SetHeights(0, 0, heights);

    //}
    [SerializeField] GameObject _Obj;
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private int resolution = 65; // ��ȿ�� �ػ� ��
    [SerializeField] private float heightScale = 1.5f; // �ͷ��� ����
    [SerializeField] private float terrainWidth = 200f; // ���� �ʺ�
    [SerializeField] private float terrainLength = 200f; // ���� ����
    

    void SetTerrainHeights() // �ͷ��� ���� �Լ�
    {
        // ��ȿ�� �ػ󵵷� ����
        terrainData.heightmapResolution = resolution;

        // ���� �������� ũ�⸦ ���� (�ʺ�� ���� ����)
        terrainData.size = new Vector3(terrainWidth, heightScale, terrainLength);

        // �ػ󵵿� �´� ���� �迭 ����
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        // ���� �迭 ���� (��� ���� ���� ������ ���̷� ����)
        for (int y = 2; y < terrainData.heightmapResolution - 2; y++)
        {
            for (int x = 2; x < terrainData.heightmapResolution - 2; x++)
            {
                heights[y, x] = heightScale / terrainData.size.y;              
                
            }
        }
        // ���� ���� ������
        SmoothTerrain(heights);
        // ���� ���� ����
        terrainData.SetHeights(0, 0, heights);
        // Terrain ������Ʈ�� ������ Flush ȣ��
        Terrain terrain = _Obj.GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.Flush();  // ���� �����͸� ������ ������Ʈ�Ͽ� ��ȭ�� �����մϴ�.

        }

    }
    void SmoothTerrain(float[,] heights)
    {
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);

        for (int i = 0; i < 5; i++) // ������ �ݺ� Ƚ��
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    // �ֺ� ���� ������ ����� ����Ͽ� ���� ���� ���� ����
                    float avgHeight = (heights[y - 1, x] + heights[y + 1, x] + heights[y, x - 1] + heights[y, x + 1]) / 4f;
                    heights[y, x] = avgHeight;
                }
            }
        }
    }

    public void LowerTerrainHeight(int x, int z)//�ͷ��� ���ߴ� �Լ� 
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
        // NavMesh �ٽ� ����ũ
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh�� �ٽ� ����ũ�Ǿ����ϴ�.");
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


    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }

}