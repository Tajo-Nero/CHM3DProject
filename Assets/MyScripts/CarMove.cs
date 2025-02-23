using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMove : MonoBehaviour
{   
    // 카플레이어 모드 흠 이걸 상태 패턴으로 구현해아하나 여튼 이상태일때 터레인 땅파는 상태
    public Transform camPos;  
    public Camera cam;
    public Transform target;
    public float rotationSpeed = 100f; // 회전 속도 조절 변수
    public float moveSpeed = 4f; // 이동 속도 조절 변수
    public GameObject _Obj;
    private Vector3 moveDir;

    private void Start()
    {
        cam = Camera.main;
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
       
    }

    private void LateUpdate()
    {
        // 카메라의 위치를 camPos의 위치로 설정
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
        }
    }
}


