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

            // ���콺 ���� ��ư Ŭ�� �� �̵� �ӵ� �ν�Ʈ Ȱ��ȭ
            if (Input.GetMouseButton(0))
            {
                isSpeedBoostActive = true;
                moveSpeed = 3f; // �̵� �ӵ� 1.5�� ����
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.3f; // Drill ��ũ��Ʈ�� digDepth ����
                }
            }
            else
            {
                isSpeedBoostActive = false;
                moveSpeed = 2f; // ���� �ӵ��� ����
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.2f; // Drill ��ũ��Ʈ�� digDepth ���� ������ ����
                }
            }

            // �¿� ȸ�� ó��
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }

            // ī�޶� ȸ�� ó��
            if (cam != null)
            {
                cam.transform.localRotation = transform.localRotation;
            }

            // F5 Ű�� ������ ���� �Ŵ��� �ʱ�ȭ
            if (Input.GetKeyDown(KeyCode.F5))
            {
                gameManager.ResetTerrain();
            }
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

   //public IEnumerator OnTriggerEnter(Collider other)
   //{
   //    if (other.CompareTag("Nexus"))
   //    {
   //        Destroy(gameObject);
   //        Debug.Log("���� ������Ʈ�� �ı��Ǿ����ϴ�.");
   //        yield return new WaitForSeconds(1f);
   //        Debug.Log("1�ʰ� �������ϴ�.");
   //        gameManager.SpawnPlayer(_PlayerMode);
   //        Debug.Log("SpawnPlayer �Լ��� ȣ��Ǿ����ϴ�.");
   //        gameManager.BakeNavMesh();
   //        Debug.Log("BakeNavMesh �Լ��� ȣ��Ǿ����ϴ�.");
   //        
   //    }
   //}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Nexus"))
        {
            gameManager.SpawnPlayer(_PlayerMode);
            Debug.Log("SpawnPlayer �Լ��� ȣ��Ǿ����ϴ�.");
            Destroy(gameObject);
            Debug.Log("���� ������Ʈ�� �ı��Ǿ����ϴ�.");
            
           
        }
    }

}