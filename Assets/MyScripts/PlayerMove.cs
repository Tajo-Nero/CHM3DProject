using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // �̵� �ӵ�
    [SerializeField] private float jumpPower = 4f; // ���� ��
    [SerializeField] private float camRotSpeed = 1f; // ī�޶� ȸ�� �ӵ�
    [SerializeField] private Transform camPos; // ī�޶� ��ġ
    private Animator animator; // �ִϸ�����
    private Rigidbody rb; // ������ٵ�
    private Transform playerTransform; // �÷��̾� Ʈ������
    public GameManager gameManager;
    private TowerGenerator towerGenerator;

    [Header("Camera Settings")]
    private Camera cam; // ī�޶�
    private float mouseX, mouseY; // ���콺 �Է� ��
    [SerializeField] private float minYRotation = -30f; // Y�� ȸ�� �ּҰ�
    [SerializeField] private float maxYRotation = 30f; // Y�� ȸ�� �ִ밪

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        gameManager = FindObjectOfType<GameManager>();
        towerGenerator = FindObjectOfType<TowerGenerator>(); // TowerGenerator �ν��Ͻ� ã��
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleJump();
        cam.transform.position = camPos.position;

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("IsHammer", true);
        }
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("IsHammer", false);
        }
        if (Input.GetKeyDown(KeyCode.G) && gameManager != null)
        {
            gameManager.AdvanceWave(); // AdvanceWave �޼��� ȣ��
        }

        HandleTowerDeletion();
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
        mouseY = Mathf.Clamp(mouseY, minYRotation, maxYRotation); // Y�� ȸ�� ���� ����

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

    private void HandleTowerDeletion()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3 rayOrigin = new Vector3(playerTransform.position.x, playerTransform.position.y + 1.0f, playerTransform.position.z);
            Ray ray = new Ray(rayOrigin, playerTransform.forward);
            RaycastHit hit;

            // ����ĳ��Ʈ�� Ȯ���ϱ� ���� ����� ���� �׸���.
            Debug.DrawRay(rayOrigin, playerTransform.forward * 4.0f, Color.red, 1.0f);

            if (Physics.Raycast(ray, out hit, 3.0f))
            {
                Debug.Log("�浹�� ��ü: " + hit.collider.name);
                if (hit.collider.CompareTag("Towers"))
                {
                    towerGenerator.RemoveTower(hit.collider.gameObject); // Tower ���� �޼��� ȣ��
                    Debug.Log("Ÿ�� ������: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("Ÿ���� �浹���� ����");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = new Vector3(playerTransform.position.x, playerTransform.position.y + 2.0f, playerTransform.position.z);
            Gizmos.DrawRay(rayOrigin, playerTransform.forward * 3.0f);
        }
    }

    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }
}
