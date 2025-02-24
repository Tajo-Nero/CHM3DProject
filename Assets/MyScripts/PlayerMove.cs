using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    //2025- 02 -23
    //카메라만 잘 만지면 될듯함
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

    void Start() // 시작할 때 호출되는 함수
    {
        cam = Camera.main; // 메인 카메라 할당
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 할당
        rb = GetComponent<Rigidbody>(); // 리지드바디 컴포넌트 할당
        playerTransform = GetComponent<Transform>(); // 플레이어 트랜스폼 할당
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update() // 매 프레임마다 호출되는 함수
    {
        HandleMovement(); // 이동 처리
        HandleRotation(); // 회전 처리
        HandleJump(); // 점프 처리
        cam.transform.position = camPos.position; // 카메라 위치 설정

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("IsHammer", true); // 마우스 왼쪽 버튼 클릭 시 애니메이션 트리거 설정
        }
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("IsHammer", false); // 마우스 오른쪽 버튼 클릭 시 애니메이션 트리거 해제
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (gameManager != null)
            {
                StartCoroutine(gameManager.SpawnEnemies()); // 비동기로 적을 생성합니다.
            }
            else
            {
                Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다.");

            }
        }
    }
    private void HandleMovement() // 이동 처리 함수
    {
        float h = Input.GetAxis("Horizontal"); // 수평 입력 값
        float v = Input.GetAxis("Vertical"); // 수직 입력 값
        Vector3 moveDir = new Vector3(h, 0, v).normalized; // 이동 방향 벡터

        playerTransform.Translate(moveDir * moveSpeed * Time.deltaTime); // 플레이어 이동
        animator.SetFloat("Move", moveDir.magnitude); // 애니메이션 파라미터 설정
    }

    private void HandleRotation() // 회전 처리 함수
    {
        mouseX += Input.GetAxis("Mouse X") * camRotSpeed; // 마우스 X축 입력 값 처리
        mouseY -= Input.GetAxis("Mouse Y") * camRotSpeed; // 마우스 Y축 입력 값 처리
        cam.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0); // 카메라 회전
        playerTransform.rotation = Quaternion.Euler(0, mouseX, 0); // 플레이어 회전
    }

    private void HandleJump() // 점프 처리 함수
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse); // 점프 힘 추가
            animator.SetBool("IsJump", true); // 점프 애니메이션 트리거 설정
        }
    }

    private void LateUpdate() // 모든 업데이트가 끝난 후 호출되는 함수
    {
        cam.transform.position = camPos.position; // 카메라 위치 설정
    }
}
