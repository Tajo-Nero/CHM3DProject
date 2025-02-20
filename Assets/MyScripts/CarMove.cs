using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMove : MonoBehaviour
{

    public Transform rt;
    public Transform camPos;
    public float camRotSpeed = 1;
    public Camera cam;
    public Transform target;
    public float rotationSpeed = 5f; // 회전 속도 조절 변수
    public float moveSpeed = 4f; // 이동 속도 조절 변수

    private Vector3 moveDir;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // 입력 값 받기
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        // 이동 방향 설정
        moveDir = new Vector3(moveX, 0, moveY).normalized;

        if (moveDir != Vector3.zero)
        {
            // 목표 회전 계산
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            // 회전 보간
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            // 위치 이동
            transform.localPosition += transform.forward * Time.deltaTime * moveSpeed;
        }
        float rotationInput = Input.GetAxis("Horizontal");

        if (rotationInput != 0)
        {
            // 로컬 Y 축 회전
            transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime, Space.World);


            // 카메라의 로컬 회전을 플레이어와 일치시키기
            if (cam != null)
            {
                cam.transform.localRotation = transform.localRotation;
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
}


