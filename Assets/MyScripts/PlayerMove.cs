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
    [SerializeField]
    private Terrain terrain;
    void LowerTerrainHeight()
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = terrainData.GetHeights(0, 0, width, height);
    
        // 모든 높이를 50%로 낮추기
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] *= 0.5f;
            }
        }
    
        terrainData.SetHeights(0, 0, heights);
    }

    void Start()
    {
       terrain = FindObjectOfType<Terrain>();
       if (terrain == null)
       {
           Debug.LogError("Terrain 컴포넌트를 찾을 수 없습니다.");
           return;
       }
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

        if (Input.GetKeyDown(KeyCode.W))
        {
            LowerTerrainHeight();



        }
        
            PlayermoveMent();
        
            //if (moveDir != Vector3.zero)
            //{
            //    transform.rotation = Quaternion.LookRotation(moveDir);
            //    transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);            
            //}
            if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("IsHammer", false);
        }
    }
    void PlayermoveMent()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 전후좌우 이동 방향 벡터 계산
        moveDir = (Vector3.forward * v) + (Vector3.right * h);
        // Translate(이동 방향 * 속력 * Time.deltaTime)
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        animator.SetFloat("Move", moveDir.magnitude);

    }
    //void OnMove(InputValue val)
    //{
    //    Vector2 dir = val.Get<Vector2>();
    //    moveDir = new Vector3(dir.x ,0,dir.y);
    //    
    //    animator.SetFloat("Move", dir.magnitude);
    //}
    void OnAttack()
    {
        animator.SetBool("IsHammer",true);
    }
    void OnJump()
    {        
        rb.AddForce(Vector3.up * jumpPow ,ForceMode.Impulse);
        animator.SetBool("IsJump", true);
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {            
            animator.SetBool("IsJump",false); 
        }
    }
    private void LateUpdate()
    {
        //cam.transform.position = target.position + (-target.forward * 5) + (Vector3.up * 4);
        //camPos.LookAt(target.position);
        //camPos.transform.rotation = Quaternion.identity;
        cam.transform.position = camPos.position;
    }



}

