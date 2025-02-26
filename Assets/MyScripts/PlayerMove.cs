using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // 이동 속도
    [SerializeField] private float jumpPower = 4f; // 점프 힘
    [SerializeField] private float camRotSpeed = 1f; // 카메라 회전 속도
    [SerializeField] private Transform camPos; // 카메라 위치
    private Animator animator; // 애니메이터
    private Rigidbody rb; // 리지드바디
    private Transform playerTransform; // 플레이어 트랜스폼
    public GameManager gameManager;

    [Header("Camera Settings")]
    private Camera cam; // 카메라
    private float mouseX, mouseY; // 마우스 입력 값
    [SerializeField] private float minYRotation = -30f; // Y축 회전 최소값
    [SerializeField] private float maxYRotation = 30f; // Y축 회전 최대값

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
                Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다.");
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
        mouseY = Mathf.Clamp(mouseY, minYRotation, maxYRotation); // Y축 회전 값 제한

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
