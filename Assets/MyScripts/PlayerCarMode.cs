using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarMode : MonoBehaviour
{
    // ī�÷��̾� ��� �� �̰� ���� �������� �����ؾ��ϳ� ��ư �̻����϶� �ͷ��� ���Ĵ� ����
    public Transform camPos;
    public Camera cam;
    public Transform target;
    public float rotationSpeed = 100f; // ȸ�� �ӵ� ���� ����
    public float moveSpeed = 4f; // �̵� �ӵ� ���� ����    
    private Vector3 moveDir;
    public GameObject _PlayerMode;
    private GameManager gameManager;
    private bool isSpeedBoostActive = false; //�̵��ӵ� 

    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>();  // GameManager �ν��Ͻ� ã��
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
            if (isSpeedBoostActive)
            {
                moveSpeed *= 1.5f;  // �̵� �ӵ� 1.5�� ����

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

            if (Input.GetMouseButton(0))
            {
                isSpeedBoostActive = true;
            }

            else
            {
                isSpeedBoostActive = false;
            }
            // F5�� ������ �� �ͷ��� ����
            if (Input.GetKeyDown(KeyCode.F5))
            {
                gameManager.ResetTerrain();
            }
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

    //�浹 üũ�� ī�÷��̾� �϶� �ؼ���(Ÿ��)�̶� ������ �׺�Ž� ���� ����ũ�ϰ� �÷��̾� ������ ���� ���ڸ��� ����
    private void OnTriggerEnter(Collider other)
    {
        // �ؼ����� ���� �� �÷��̾� ���������� ��ȯ
        if (other.CompareTag("Nexus"))
        {
            //�ڷ�ƾ�ؼ� ����� 1�ʵڿ� ����ũ�Լ� �����غ���
            Destroy(gameObject);
            gameManager.SpawnPlayer(_PlayerMode);
            gameManager.BakeNavMesh();
        }

    }
}