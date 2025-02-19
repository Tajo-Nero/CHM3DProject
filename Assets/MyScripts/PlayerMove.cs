using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    Vector3 moveDir;
    float moveSpeed = 5;
    float jumpPow = 4;

    public float mouseX;
    public float mouseY;
    public Transform camPos;
    public float camRotSpeed = 1;
    public Camera cam;
    public Transform target;
    Transform tr;
    
    
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private int resolution = 32;//�ͷ��� ũ��
    //[SerializeField] private float scale = 1f; //������ �ͷ��� ������,�̰� PerlinNoise ������= ���� �����ҋ� ���� 
    [SerializeField] private float heightScale = 1.5f; // �ͷ��� ����
    //public float digDistance = 1.0f;  // ���� �� �Ÿ�
    void SetTerrainHeights()//�ͷ��� ���� �Լ�
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(resolution, heightScale, resolution);
        float[,] heights = new float[resolution, resolution];

        //for ���̶� 0,0 �������� �����ؼ� x,y 0 ��ǥ�� ���� ���۵� 
        //0,0 ���� resolution�� �־��� �� ���� ���� 100,100 ���� ������ ���̱��� �ö������ 
        for (int x = 1; x < resolution; x++)
        {
            for (int y = 1; y < resolution; y++)
            {
                //���� �����Ҷ� ���̴� ������ �Լ� PerlinNoise(�ʺ�,�ʺ�) �־��ָ� 
                //heights[x, y] = perlin * heightScale; �ʺ� * ���� ������ ���� ����ŭ ���� �ö󰡸鼭 ���������Ǽ� ������ �ϼ���
                //�ݴ�� ���� �ʺ���� �������鼭 �������� ���� ���� ���� �׳� �������ϸ� ������ �Ƚᵵ �� 
                //float perlin = Mathf.PerlinNoise((float)x / resolution * scale, (float)y / resolution * scale);
                heights[x, y] = resolution * heightScale;
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
    public void LowerTerrainHeight(int x, int z)//�ͷ��� ���ߴ� �Լ� 
    {

        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);
        heights[x, z] -= 0.1f;

        terrainData.SetHeights(0, 0, heights);
    }
    

    void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();

    }
    void Update()
    {
        mouseX += Input.GetAxis("Mouse X");
        mouseY -= Input.GetAxis("Mouse Y");
        cam.transform.rotation = Quaternion.Euler(mouseY * camRotSpeed, mouseX * camRotSpeed, 0);
        transform.rotation = Quaternion.Euler(0, mouseX * camRotSpeed, 0);

        PlayermoveMent();

        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IsHammer", false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetTerrainHeights();
        }
       

    }
    void PlayermoveMent()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // �����¿� �̵� ���� ���� ���
        moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Translate(�̵� ���� * �ӷ� * Time.deltaTime)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        animator.SetFloat("Move", moveDir.magnitude);

    }

    void OnAttack()
    {
        animator.SetBool("IsHammer", true);
    }
    void OnJump()
    {
        rb.AddForce(Vector3.up * jumpPow, ForceMode.Impulse);
        animator.SetBool("IsJump", true);
    }

   
    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }
    
}