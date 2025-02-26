//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Magnet : MonoBehaviour
//{
//    [SerializeField] private LayerMask towerLayer; // 타워 레이어 설정
//    public float magnetForce = 10f; // 자석의 힘
//    public float detectionRadius = 5f; // 감지 반경
//    public float stopDistance = 1f; // 중앙에 멈추는 거리

//    private void FixedUpdate()
//    {
//        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
//        foreach (Collider collider in colliders)
//        {
//            Rigidbody rb = collider.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                Vector3 direction = transform.position - rb.transform.position;
//                float distance = direction.magnitude;

//                if (distance > stopDistance)
//                {
//                    rb.AddForce(direction.normalized * magnetForce);
//                }
//                else
//                {
//                    rb.velocity = Vector3.zero; // 속도를 0으로 설정하여 멈추기
//                    rb.angularVelocity = Vector3.zero; // 회전 속도도 0으로 설정
//                }
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        // 감지 반경을 Gizmo로 시각화
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, detectionRadius);
//        // 멈추는 거리를 Gizmo로 시각화
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, stopDistance);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer; // 타워 레이어 설정
    [SerializeField] private LayerMask playerLayer; // 타워 레이어 설정  
    public float detectionRadius = 5f; // 감지 반경
    public float stopDistance = 0.1f; // 고정될 때까지의 거리 오차
    private float magnetForce= 20f;

    private bool isPlayerInRange = false; // 플레이어 감지 여부

    private void FixedUpdate()
    {
        // 플레이어 감지
        Collider[] towerColliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
        isPlayerInRange = towerColliders.Length > 0;

        if (isPlayerInRange)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z); // 현재 위치에서 y값만 2로 설정

            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distance = Vector3.Distance(rb.transform.position, targetPosition);
                    if (distance > stopDistance)
                    {
                        rb.transform.position = Vector3.MoveTowards(rb.transform.position, targetPosition, Time.deltaTime * magnetForce); // 고정 속도 설정
                    }
                    else
                    {
                        rb.transform.position = targetPosition; // 타워를 원하는 위치로 고정
                    }
                }
            }
        }
        else
        {
            // 플레이어가 감지 범위를 벗어났을 때 타겟 포지션 변경
            Collider[] playerCollidersAfterDetection = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            if (playerCollidersAfterDetection.Length > 0)
            {
                // 플레이어 포지션의 포워드 방향으로 타겟 포지션 설정
                Vector3 playerPosition = playerCollidersAfterDetection[0].transform.position;
                Vector3 targetPositionForward = playerPosition + playerCollidersAfterDetection[0].transform.forward * detectionRadius;
                targetPositionForward.y = playerPosition.y + 2; // y값 조정

                // 타워 위치를 새로운 타겟 포지션으로 변경
                Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
                foreach (Collider collider in colliders)
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.transform.position = targetPositionForward; // 타워를 새로운 타겟 포지션으로 이동
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 감지 반경을 Gizmo로 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        // 고정될 위치를 Gizmo로 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 0.5f);
    }
}
