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
    private TowerGenerator towerGenerator;

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
        towerGenerator = FindObjectOfType<TowerGenerator>(); // TowerGenerator 인스턴스 찾기
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
            gameManager.AdvanceWave(); // AdvanceWave 메서드 호출
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
        mouseY = Mathf.Clamp(mouseY, minYRotation, maxYRotation); // Y축 회전 제한 설정

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

            // 레이캐스트를 확인하기 위해 디버그 라인 그리기.
            Debug.DrawRay(rayOrigin, playerTransform.forward * 4.0f, Color.red, 1.0f);

            if (Physics.Raycast(ray, out hit, 3.0f))
            {
                Debug.Log("충돌한 객체: " + hit.collider.name);
                if (hit.collider.CompareTag("Towers"))
                {
                    towerGenerator.RemoveTower(hit.collider.gameObject); // Tower 삭제 메서드 호출
                    Debug.Log("타워 삭제됨: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("타워와 충돌하지 않음");
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
