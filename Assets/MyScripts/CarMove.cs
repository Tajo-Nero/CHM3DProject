using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMove : MonoBehaviour
{   
    // ī�÷��̾� ��� �� �̰� ���� �������� �����ؾ��ϳ� ��ư �̻����϶� �ͷ��� ���Ĵ� ����
    public Transform camPos;  
    public Camera cam;
    public Transform target;
    public float rotationSpeed = 100f; // ȸ�� �ӵ� ���� ����
    public float moveSpeed = 4f; // �̵� �ӵ� ���� ����
    public GameObject _Obj;
    private Vector3 moveDir;

    private void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // �Է� �� �ޱ�
        float horizontalInput = Input.GetAxis("Horizontal"); // �¿� �̵� �Է�
        float verticalInput = Input.GetAxis("Vertical"); // ���� �̵� �Է�

        // �̵� ó��
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;
        }

        // �¿� ȸ�� ó��
        if (horizontalInput != 0)
        {
            // ���� Y �� ȸ��
            transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
        }

        // ī�޶��� ���� ȸ���� �÷��̾�� ��ġ��Ű��
        if (cam != null)
        {
            cam.transform.localRotation = transform.localRotation;
        }
       
    }

    private void LateUpdate()
    {
        // ī�޶��� ��ġ�� camPos�� ��ġ�� ����
        if (cam != null && camPos != null)
        {
            cam.transform.position = camPos.position;
        }
    }
}


