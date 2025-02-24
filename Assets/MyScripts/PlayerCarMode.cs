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
    private bool isSpeedBoostActive = false; // 이동 속도 부스트 플래그
    private Drill drillScript;
    void Start()
    {
        cam = Camera.main;
        gameManager = FindObjectOfType<GameManager>(); // GameManager 인스턴스 찾기
    }

    void Update()
    {
        // 입력 값 받기
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 이동 처리
        if (verticalInput != 0)
        {
            moveDir = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
            transform.localPosition += moveDir;

            // 마우스 왼쪽 버튼 클릭 시 이동 속도 부스트 활성화
            if (Input.GetMouseButton(0))
            {
                isSpeedBoostActive = true;
                moveSpeed = 3f; // 이동 속도 1.5배 증가
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.3f; // Drill 스크립트의 digDepth 변경
                }
            }
            else
            {
                isSpeedBoostActive = false;
                moveSpeed = 2f; // 원래 속도로 복귀
                if (drillScript != null)
                {
                    drillScript.digDepth = 0.2f; // Drill 스크립트의 digDepth 원래 값으로 복귀
                }
            }

            // 좌우 회전 처리
            if (horizontalInput != 0)
            {
                transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime, Space.Self);
            }

            // 카메라 회전 처리
            if (cam != null)
            {
                cam.transform.localRotation = transform.localRotation;
            }

            // F5 키를 누르면 게임 매니저 초기화
            if (Input.GetKeyDown(KeyCode.F5))
            {
                gameManager.ResetTerrain();
            }
        }
    }

    private void LateUpdate()
    {
        // 카메라 위치 업데이트
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
   //        Debug.Log("게임 오브젝트가 파괴되었습니다.");
   //        yield return new WaitForSeconds(1f);
   //        Debug.Log("1초가 지났습니다.");
   //        gameManager.SpawnPlayer(_PlayerMode);
   //        Debug.Log("SpawnPlayer 함수가 호출되었습니다.");
   //        gameManager.BakeNavMesh();
   //        Debug.Log("BakeNavMesh 함수가 호출되었습니다.");
   //        
   //    }
   //}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Nexus"))
        {
            gameManager.SpawnPlayer(_PlayerMode);
            Debug.Log("SpawnPlayer 함수가 호출되었습니다.");
            Destroy(gameObject);
            Debug.Log("게임 오브젝트가 파괴되었습니다.");
            
           
        }
    }

}