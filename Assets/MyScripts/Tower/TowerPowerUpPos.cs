using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer; // Ÿ�� ���̾� ����
    public float magnetForce = 10f; // �ڼ��� ��
    public float detectionRadius = 5f; // ���� �ݰ�

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
        // ���� �ݰ��� Gizmo�� �ð�ȭ
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
