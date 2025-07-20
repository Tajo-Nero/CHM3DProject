using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // 이동 속도
    [SerializeField] private float jumpPower = 4f; // 점프 힘
    [SerializeField] private float camRotSpeed = 1f; // 카메라 회전 속도
    [SerializeField] private Transform camPos; // 카메라 위치

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // 땅 체크 포인트
    [SerializeField] private float groundDistance = 0.4f; // 땅 감지 거리
    [SerializeField] private LayerMask groundMask; // 땅 레이어

    // 컴포넌트들
    private Animator animator; // 애니메이터
    private Rigidbody rb; // 리지드바디
    private Transform playerTransform; // 플레이어 트랜스폼
    private Camera cam; // 카메라

    // 매니저들
    public GameManager gameManager;
    private TowerGenerator towerGenerator;

    // 상태 변수들
    private bool isGrounded; // 땅에 닿아있는지
    private bool isHammering = false; // 해머 사용 중인지
    private bool isMoving = false; // 이동 중인지

    [Header("Camera Settings")]
    private float mouseX, mouseY; // 마우스 입력 값
    [SerializeField] private float minYRotation = -30f; // Y축 회전 최솟값
    [SerializeField] private float maxYRotation = 30f; // Y축 회전 최댓값

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
        // GroundCheck가 없으면 발 위치에 생성
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
        // 땅에 닿아있는지 체크
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // 점프 애니메이션 리셋
        if (isGrounded && animator.GetBool("IsJump"))
        {
            animator.SetBool("IsJump", false);
        }

        // 애니메이터에 Ground 상태 전달
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        // 이동 처리
        playerTransform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // 이동 상태 업데이트
        isMoving = moveDir.magnitude > 0.1f;

        // 애니메이터에 이동 속도 전달
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
            animator.SetTrigger("JumpTrigger"); // 트리거 사용도 가능
        }
    }

    private void HandleHammer()
    {
        // 마우스 버튼 상태에 따른 해머 애니메이션
        bool hammerInput = Input.GetMouseButton(0);

        // 상태가 변경될 때만 애니메이터 업데이트
        if (hammerInput != isHammering)
        {
            isHammering = hammerInput;
            animator.SetBool("IsHammer", isHammering);

            if (isHammering)
            {
                animator.SetTrigger("HammerAttack"); // 트리거로도 사용 가능
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
                Debug.Log("충돌한 객체: " + hit.collider.name);
                if (hit.collider.CompareTag("Towers"))
                {
                    if (towerGenerator != null)
                    {
                        towerGenerator.RemoveTower(hit.collider.gameObject);
                        Debug.Log("타워 삭제됨: " + hit.collider.name);
                    }
                }
            }
            else
            {
                Debug.Log("타워와 충돌하지 않음");
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

    // 애니메이션 이벤트에서 호출 가능한 메서드들
    public void OnHammerHit()
    {
        // 해머 타격 시점에 호출 (Animation Event)
        Debug.Log("해머 타격!");
    }

    public void OnJumpLand()
    {
        // 착지 시점에 호출 (Animation Event)
        Debug.Log("착지!");
    }

    // 디버그용 기즈모
    private void OnDrawGizmosSelected()
    {
        // 타워 삭제 레이 표시
        if (playerTransform != null)
        {
            Gizmos.color = Color.red;
            Vector3 rayOrigin = new Vector3(playerTransform.position.x, playerTransform.position.y + 2.0f, playerTransform.position.z);
            Gizmos.DrawRay(rayOrigin, playerTransform.forward * 3.0f);
        }

        // 땅 체크 범위 표시
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}