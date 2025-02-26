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
    private bool isSpeedBoostActive = false; // �̵� �ӵ� �ν�Ʈ �÷���
    private Drill drillScript;

    public float cameraRotationX = 45f; // ī�޶��� X�� ȸ�� ���� ������ �Է°�

    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // GameManager �ν��Ͻ� ã��
    }

    void Update()
    {
        // �Է� �� �ޱ�
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �̵� ó��
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;

            // ���콺 ���� ��ư Ŭ�� �� �̵� �ӵ� �ν�Ʈ
            if (Input.GetMouseButton(0))
            {
                isSpeedBoostActive = true;
                moveSpeed = 5f; // �̵� �ӵ� �ν�Ʈ 3
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.5f; // Drill ��ũ��Ʈ�� digDepth ���� 0.3
                }
            }
            else
            {
                isSpeedBoostActive = false;
                moveSpeed = 3f; // �⺻ �̵� �ӵ� 2
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.3f; // Drill ��ũ��Ʈ�� digDepth �⺻������ ���� 0.2
                }
            }

            // �¿� ȸ�� ó��
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        // ī�޶� ȸ�� �� ��ġ ����
        if (cam != null)
        {
            cam.transform.rotation = Quaternion.Euler(cameraRotationX, transform.rotation.eulerAngles.y, 0); // ī�޶� ȸ�� ����
        }

        // F5 Ű ������ ���� �ʱ�ȭ
        if (Input.GetKeyDown(KeyCode.F5))
        {
            gameManager.ResetTerrain();
        }
    }

    private void LateUpdate()
    {
        // ī�޶� ��ġ ������Ʈ
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
            Debug.Log("SpawnPlayer �Լ��� ȣ��Ǿ����ϴ�.");
            Destroy(gameObject);
            Debug.Log("�÷��̾ �ı��Ǿ����ϴ�.");
        }
    }
}
