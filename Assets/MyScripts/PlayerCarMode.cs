using System.Collections;
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

    public float cameraRotationX = 45f; // ī�޶� X�� ȸ�� ����
    private GameObject drillSphere;
    [SerializeField]
    private float digDistance = 3f; // �帱 �Ÿ�
    public float digRadius = 3f; // �帱 ���� �ݰ�
    public float digDepth = 0.2f; // �帱 ����

    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // GameManager �ν��Ͻ� ã��
    }

    void Update()
    {
        // �Է� �� �ޱ�
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �̵� ó��
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;

            // ���콺 ���� ��ư Ŭ�� �� �̵� �ӵ� ����
            if (Input.GetMouseButton(0))
            {
                moveSpeed = 5f; // �̵� �ӵ� ����
                digDepth = 0.4f; // �帱 ���� ����
            }
            else
            {
                moveSpeed = 3f; // �⺻ �̵� �ӵ�
                digDepth = 0.3f; // �⺻ �帱 ����
            }

            // �¿� ȸ�� ó��
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        // ī�޶� ȸ�� �� ��ġ ������Ʈ
        if (cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(cameraRotationX, transform.rotation.eulerAngles.y, 0); // ī�޶� ȸ�� ó��
        }

        // F5 Ű �Է� �� ������Ʈ �ı� �� Terrain ����
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Destroy(gameObject);
            TerrainManager terrainManager = FindObjectOfType<TerrainManager>();
            if (terrainManager != null)
            {
                terrainManager.ResetTerrain();
            }
        }

        PerformDig(); // �帱�� �� ���� �ı� ó��
    }

    private void LateUpdate()
    {
        // ī�޶� ��ġ ������Ʈ
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Nexus"))
        {
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
                drillSphere.SetActive(true); // ������Ʈ Ȱ��ȭ
            }
        }
        else
        {
            if (drillSphere != null)
            {
                drillSphere.SetActive(false); // ������Ʈ ��Ȱ��ȭ
            }
        }
    }

    private void Dig(TerrainCollider terrainCollider, Vector3 hitPoint)
    {
        Terrain terrain = terrainCollider.GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        // ��Ʈ ����Ʈ�� Heightmap ��ǥ ���
        int xBase = Mathf.FloorToInt((hitPoint.x - terrainPosition.x) / terrainData.size.x * terrainData.heightmapResolution);
        int yBase = Mathf.FloorToInt((hitPoint.z - terrainPosition.z) / terrainData.size.z * terrainData.heightmapResolution);

        // �ݰ� ���� ���� �ı�
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
                        float depthFactor = Mathf.Lerp(1, 0, distance / radius); // �Ÿ� ��� ����
                        heights[0, 0] = Mathf.Max(0, heights[0, 0] - digDepth * depthFactor); // ���� �ı�

                        // ������Ʈ�� Heightmap�� TerrainData�� ����
                        terrainData.SetHeights(xPos, yPos, heights);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // �帱 �������� �ð������� ǥ��
        Gizmos.color = Color.red;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * digDistance;
        Gizmos.DrawLine(startPosition, endPosition);

        // ��Ʈ ����Ʈ�� �߽����� �帱 ������ �ð������� ǥ��
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(endPosition, digRadius);
    }
}
