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
        // NavMesh �ٽ� ����ũ
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh�� �ٽ� ����ũ�Ǿ����ϴ�.");
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