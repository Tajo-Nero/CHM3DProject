using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMove : MonoBehaviour
{
    //public Transform camPos;    
    //public Camera cam;
    //public Transform target;
    //public float rotationSpeed = 5f; // ȸ�� �ӵ� ���� ����
    //public float moveSpeed = 4f; // �̵� �ӵ� ���� ����

    //private Vector3 moveDir;

    //private void Start()
    //{
    //cam = Camera.main;
    //}

    //void Update()
    //{
    //// �Է� �� �ޱ�
    //float moveX = Input.GetAxis("Horizontal");
    //float moveY = Input.GetAxis("Vertical");

    //// �̵� ���� ����
    //moveDir = new Vector3(moveX, 0, moveY).normalized;

    //if (moveDir != Vector3.zero)
    //{
    //// ��ǥ ȸ�� ���
    //Quaternion targetRotation = Quaternion.LookRotation(moveDir);
    //// ȸ�� ����
    //transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    //// ��ġ �̵�
    //transform.localPosition += transform.forward * Time.deltaTime * moveSpeed;
    //}
    //float rotationInput = Input.GetAxis("Horizontal");

    //if (rotationInput != 0)
    //{
    //// ���� Y �� ȸ��
    //transform.Rotate(Vector3.up, rotationInput * rotationSpeed * Time.deltaTime, Space.Self);


    //// ī�޶��� ���� ȸ���� �÷��̾�� ��ġ��Ű��
    //if (cam != null)
    //{
    //cam.transform.localRotation = transform.localRotation;
    //}
    //}
    //}

    //private void LateUpdate()
    //{
    //// ī�޶��� ��ġ�� camPos�� ��ġ�� ����
    //if (cam != null && camPos != null)
    //{
    //cam.transform.position = camPos.position;
    //}
    //}
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
        if (Input.GetKeyUp(KeyCode.O))
        {
            _Obj.gameObject.SetActive(true);
            gameObject.SetActive(false);
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


