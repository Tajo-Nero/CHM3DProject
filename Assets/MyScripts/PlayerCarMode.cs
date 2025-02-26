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
    private bool isSpeedBoostActive = false; // 이동 속도 부스트 플래그
    private Drill drillScript;

    public float cameraRotationX = 45f; // 카메라의 X축 회전 값을 조절할 입력값

    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // GameManager 인스턴스 찾기
    }

    void Update()
    {
        // 입력 값 받기
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 이동 처리
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;

            // 마우스 왼쪽 버튼 클릭 시 이동 속도 부스트
            if (Input.GetMouseButton(0))
            {
                isSpeedBoostActive = true;
                moveSpeed = 5f; // 이동 속도 부스트 3
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.5f; // Drill 스크립트의 digDepth 변경 0.3
                }
            }
            else
            {
                isSpeedBoostActive = false;
                moveSpeed = 3f; // 기본 이동 속도 2
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.3f; // Drill 스크립트의 digDepth 기본값으로 변경 0.2
                }
            }

            // 좌우 회전 처리
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        // 카메라 회전 및 위치 고정
        if (cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(cameraRotationX, transform.rotation.eulerAngles.y, 0); // 카메라 회전 설정
        }

        // F5 키 누르면 지형 초기화
        if (Input.GetKeyDown(KeyCode.F5))
        {
            gameManager.ResetTerrain();
        }
    }

    private void LateUpdate()
    {
        // 카메라 위치 업데이트
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
            Debug.Log("SpawnPlayer 함수가 호출되었습니다.");
            Destroy(gameObject);
            Debug.Log("플레이어가 파괴되었습니다.");
        }
    }
}
