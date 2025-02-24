using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarMode : MonoBehaviour
{
    // 카플레이어 모드 흠 이걸 상태 패턴으로 구현해아하나 여튼 이상태일때 터레인 땅파는 상태
    public Transform camPos;
    public Camera cam;
    public Transform target;
    public float rotationSpeed = 100f; // 회전 속도 조절 변수
    public float moveSpeed = 4f; // 이동 속도 조절 변수    
    private Vector3 moveDir;
    public GameObject _PlayerMode;
    private GameManager gameManager;
    private bool isSpeedBoostActive = false; //이동속도 

    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>();  // GameManager 인스턴스 찾기
    }

    void Update()
    {
        // 입력 값 받기
        float horizontalInput = Input.GetAxis("Horizontal"); // 좌우 이동 입력
        float verticalInput = Input.GetAxis("Vertical"); // 전후 이동 입력

        // 이동 처리
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;
            if (isSpeedBoostActive)
            {
                moveSpeed *= 1.5f;  // 이동 속도 1.5배 증가

            }

            // 좌우 회전 처리
            if (horizontalInput != 0)
            {
                // 로컬 Y 축 회전
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }

            // 카메라의 로컬 회전을 플레이어와 일치시키기
            if (cam != null)
            {
                cam.transform.localRotation = transform.localRotation;
            }

            if (Input.GetMouseButton(0))
            {
                isSpeedBoostActive = true;
            }

            else
            {
                isSpeedBoostActive = false;
            }
            // F5를 눌렀을 때 터레인 리셋
            if (Input.GetKeyDown(KeyCode.F5))
            {
                gameManager.ResetTerrain();
            }
        }
    }
    private void LateUpdate()
    {
        // 카메라의 위치를 camPos의 위치로 설정
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
        }
    }

    //충돌 체크중 카플레이어 일때 넥서스(타겟)이랑 만나면 네비매쉬 새로 베이크하고 플레이어 프리팹 생성 그자리에 생성
    private void OnTriggerEnter(Collider other)
    {
        // 넥서스와 접촉 시 플레이어 프리팹으로 전환
        if (other.CompareTag("Nexus"))
        {
            //코루틴해서 지우고 1초뒤에 베이크함수 실행해보기
            Destroy(gameObject);
            gameManager.SpawnPlayer(_PlayerMode);
            gameManager.BakeNavMesh();
        }

    }
}