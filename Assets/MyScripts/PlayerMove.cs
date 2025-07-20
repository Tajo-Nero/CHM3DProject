using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // �̵� �ӵ�
    [SerializeField] private float jumpPower = 4f; // ���� ��
    [SerializeField] private float camRotSpeed = 1f; // ī�޶� ȸ�� �ӵ�
    [SerializeField] private Transform camPos; // ī�޶� ��ġ

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // �� üũ ����Ʈ
    [SerializeField] private float groundDistance = 0.4f; // �� ���� �Ÿ�
    [SerializeField] private LayerMask groundMask; // �� ���̾�

    // ������Ʈ��
    private Animator animator; // �ִϸ�����
    private Rigidbody rb; // ������ٵ�
    private Transform playerTransform; // �÷��̾� Ʈ������
    private Camera cam; // ī�޶�

    // �Ŵ�����
    public GameManager gameManager;
    private TowerGenerator towerGenerator;

    // ���� ������
    private bool isGrounded; // ���� ����ִ���
    private bool isHammering = false; // �ظ� ��� ������
    private bool isMoving = false; // �̵� ������

    [Header("Camera Settings")]
    private float mouseX, mouseY; // ���콺 �Է� ��
    [SerializeField] private float minYRotation = -30f; // Y�� ȸ�� �ּڰ�
    [SerializeField] private float maxYRotation = 30f; // Y�� ȸ�� �ִ�

    void Start()
    {
        InitializeComponents();
        SetupGroundCheck();
    }

    private void InitializeComponents()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        gameManager = FindObjectOfType<GameManager>();
        towerGenerator = FindObjectOfType<TowerGenerator>();
    }

    private void SetupGroundCheck()
    {
        // GroundCheck�� ������ �� ��ġ�� ����
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, 0.1f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        CheckGroundStatus();
        HandleMovement();
        HandleRotation();
        HandleJump();
        HandleHammer();
        HandleGameInput();
        HandleTowerDeletion();
        UpdateCameraPosition();
    }

    private void CheckGroundStatus()
    {
        // ���� ����ִ��� üũ
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // ���� �ִϸ��̼� ����
        if (isGrounded && animator.GetBool("IsJump"))
        {
            animator.SetBool("IsJump", false);
        }

        // �ִϸ����Ϳ� Ground ���� ����
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        // �̵� ó��
        playerTransform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // �̵� ���� ������Ʈ
        isMoving = moveDir.magnitude > 0.1f;

        // �ִϸ����Ϳ� �̵� �ӵ� ����
        animator.SetFloat("Move", moveDir.magnitude);
        animator.SetBool("IsMoving", isMoving);
    }

    private void HandleRotation()
    {
        mouseX += Input.GetAxis("Mouse X") * camRotSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * camRotSpeed;
        mouseY = Mathf.Clamp(mouseY, minYRotation, maxYRotation);

        cam.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        playerTransform.rotation = Quaternion.Euler(0, mouseX, 0);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("IsJump", true);
            animator.SetTrigger("JumpTrigger"); // Ʈ���� ��뵵 ����
        }
    }

    private void HandleHammer()
    {
        // ���콺 ��ư ���¿� ���� �ظ� �ִϸ��̼�
        bool hammerInput = Input.GetMouseButton(0);

        // ���°� ����� ���� �ִϸ����� ������Ʈ
        if (hammerInput != isHammering)
        {
            isHammering = hammerInput;
            animator.SetBool("IsHammer", isHammering);

            if (isHammering)
            {
                animator.SetTrigger("HammerAttack"); // Ʈ���ŷε� ��� ����
            }
        }
    }

    private void HandleGameInput()
    {
        if (Input.GetKeyDown(KeyCode.G) && gameManager != null)
        {
            gameManager.AdvanceWave();
        }
    }

    private void HandleTowerDeletion()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Vector3 rayOrigin = new Vector3(playerTransform.position.x, playerTransform.position.y + 1.0f, playerTransform.position.z);
            Ray ray = new Ray(rayOrigin, playerTransform.forward);
            RaycastHit hit;

            Debug.DrawRay(rayOrigin, playerTransform.forward * 4.0f, Color.red, 1.0f);

            if (Physics.Raycast(ray, out hit, 3.0f))
            {
                Debug.Log("�浹�� ��ü: " + hit.collider.name);
                if (hit.collider.CompareTag("Towers"))
                {
                    if (towerGenerator != null)
                    {
                        towerGenerator.RemoveTower(hit.collider.gameObject);
                        Debug.Log("Ÿ�� ������: " + hit.collider.name);
                    }
                }
            }
            else
            {
                Debug.Log("Ÿ���� �浹���� ����");
            }
        }
    }

    private void UpdateCameraPosition()
    {
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
        }
    }

    private void LateUpdate()
    {
        UpdateCameraPosition();
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ�� ������ �޼����
    public void OnHammerHit()
    {
        // �ظ� Ÿ�� ������ ȣ�� (Animation Event)
        Debug.Log("�ظ� Ÿ��!");
    }

    public void OnJumpLand()
    {
        // ���� ������ ȣ�� (Animation Event)
        Debug.Log("����!");
    }

    // ����׿� �����
    private void OnDrawGizmosSelected()
    {
        // Ÿ�� ���� ���� ǥ��
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = new Vector3(playerTransform.position.x, playerTransform.position.y + 2.0f, playerTransform.position.z);
            Gizmos.DrawRay(rayOrigin, playerTransform.forward * 3.0f);
        }

        // �� üũ ���� ǥ��
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}