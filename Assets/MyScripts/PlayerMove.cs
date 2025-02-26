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
        if (Input.GetKeyDown(KeyCode.G) && !gameManager.IsWaveActive)
        {
            if (gameManager != null)
            {
                StartCoroutine(gameManager.StartWave());
            }
            else
            {
                Debug.LogError("GameManager �ν��Ͻ��� ã�� �� �����ϴ�.");
            }

        }
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
        mouseY = Mathf.Clamp(mouseY, minYRotation, maxYRotation); // Y�� ȸ�� �� ����

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
