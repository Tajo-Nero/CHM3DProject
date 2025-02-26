using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer; // 타워 레이어 설정
    public float magnetForce = 10f; // 자석의 힘
    public float detectionRadius = 5f; // 감지 반경

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = transform.position - rb.transform.position;
                rb.AddForce(direction.normalized * magnetForce);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 감지 반경을 Gizmo로 시각화
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
