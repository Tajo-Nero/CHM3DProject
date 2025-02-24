using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMove : MonoBehaviour
{
    //2025- 02 -23
    //ī�޶� �� ������ �ɵ���
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // �̵� �ӵ�
    [SerializeField] private float jumpPower = 4f; // ���� ��
    [SerializeField] private float camRotSpeed = 1f; // ī�޶� ȸ�� �ӵ�
    [SerializeField] private Transform camPos; // ī�޶� ��ġ
    private Animator animator; // �ִϸ�����
    private Rigidbody rb; // ������ٵ�
    private Transform playerTransform; // �÷��̾� Ʈ������
    public GameManager gameManager;

   [Header("Camera Settings")]
    private Camera cam; // ī�޶�
    private float mouseX, mouseY; // ���콺 �Է� ��

    void Start() // ������ �� ȣ��Ǵ� �Լ�
    {
        cam = Camera.main; // ���� ī�޶� �Ҵ�
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ �Ҵ�
        rb = GetComponent<Rigidbody>(); // ������ٵ� ������Ʈ �Ҵ�
        playerTransform = GetComponent<Transform>(); // �÷��̾� Ʈ������ �Ҵ�
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update() // �� �����Ӹ��� ȣ��Ǵ� �Լ�
    {
        HandleMovement(); // �̵� ó��
        HandleRotation(); // ȸ�� ó��
        HandleJump(); // ���� ó��
        cam.transform.position = camPos.position; // ī�޶� ��ġ ����

        if (Input.GetMouseButton(0))
        {
            animator.SetBool("IsHammer", true); // ���콺 ���� ��ư Ŭ�� �� �ִϸ��̼� Ʈ���� ����
        }
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("IsHammer", false); // ���콺 ������ ��ư Ŭ�� �� �ִϸ��̼� Ʈ���� ����
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (gameManager != null)
            {
                StartCoroutine(gameManager.SpawnEnemies()); // �񵿱�� ���� �����մϴ�.
            }
            else
            {
                Debug.LogError("GameManager �ν��Ͻ��� ã�� �� �����ϴ�.");

            }
        }
    }
    private void HandleMovement() // �̵� ó�� �Լ�
    {
        float h = Input.GetAxis("Horizontal"); // ���� �Է� ��
        float v = Input.GetAxis("Vertical"); // ���� �Է� ��
        Vector3 moveDir = new Vector3(h, 0, v).normalized; // �̵� ���� ����

        playerTransform.Translate(moveDir * moveSpeed * Time.deltaTime); // �÷��̾� �̵�
        animator.SetFloat("Move", moveDir.magnitude); // �ִϸ��̼� �Ķ���� ����
    }

    private void HandleRotation() // ȸ�� ó�� �Լ�
    {
        mouseX += Input.GetAxis("Mouse X") * camRotSpeed; // ���콺 X�� �Է� �� ó��
        mouseY -= Input.GetAxis("Mouse Y") * camRotSpeed; // ���콺 Y�� �Է� �� ó��
        cam.transform.rotation = Quaternion.Euler(mouseY, mouseX, 0); // ī�޶� ȸ��
        playerTransform.rotation = Quaternion.Euler(0, mouseX, 0); // �÷��̾� ȸ��
    }

    private void HandleJump() // ���� ó�� �Լ�
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse); // ���� �� �߰�
            animator.SetBool("IsJump", true); // ���� �ִϸ��̼� Ʈ���� ����
        }
    }

    private void LateUpdate() // ��� ������Ʈ�� ���� �� ȣ��Ǵ� �Լ�
    {
        cam.transform.position = camPos.position; // ī�޶� ��ġ ����
    }
}
