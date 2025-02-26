//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Magnet : MonoBehaviour
//{
//    [SerializeField] private LayerMask towerLayer; // Ÿ�� ���̾� ����
//    public float magnetForce = 10f; // �ڼ��� ��
//    public float detectionRadius = 5f; // ���� �ݰ�
//    public float stopDistance = 1f; // �߾ӿ� ���ߴ� �Ÿ�

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
//                    rb.velocity = Vector3.zero; // �ӵ��� 0���� �����Ͽ� ���߱�
//                    rb.angularVelocity = Vector3.zero; // ȸ�� �ӵ��� 0���� ����
//                }
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        // ���� �ݰ��� Gizmo�� �ð�ȭ
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, detectionRadius);
//        // ���ߴ� �Ÿ��� Gizmo�� �ð�ȭ
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, stopDistance);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private LayerMask towerLayer; // Ÿ�� ���̾� ����
    [SerializeField] private LayerMask playerLayer; // Ÿ�� ���̾� ����  
    public float detectionRadius = 5f; // ���� �ݰ�
    public float stopDistance = 0.1f; // ������ �������� �Ÿ� ����
    private float magnetForce= 20f;

    private bool isPlayerInRange = false; // �÷��̾� ���� ����

    private void FixedUpdate()
    {
        // �÷��̾� ����
        Collider[] towerColliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
        isPlayerInRange = towerColliders.Length > 0;

        if (isPlayerInRange)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z); // ���� ��ġ���� y���� 2�� ����

            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distance = Vector3.Distance(rb.transform.position, targetPosition);
                    if (distance > stopDistance)
                    {
                        rb.transform.position = Vector3.MoveTowards(rb.transform.position, targetPosition, Time.deltaTime * magnetForce); // ���� �ӵ� ����
                    }
                    else
                    {
                        rb.transform.position = targetPosition; // Ÿ���� ���ϴ� ��ġ�� ����
                    }
                }
            }
        }
        else
        {
            // �÷��̾ ���� ������ ����� �� Ÿ�� ������ ����
            Collider[] playerCollidersAfterDetection = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
            if (playerCollidersAfterDetection.Length > 0)
            {
                // �÷��̾� �������� ������ �������� Ÿ�� ������ ����
                Vector3 playerPosition = playerCollidersAfterDetection[0].transform.position;
                Vector3 targetPositionForward = playerPosition + playerCollidersAfterDetection[0].transform.forward * detectionRadius;
                targetPositionForward.y = playerPosition.y + 2; // y�� ����

                // Ÿ�� ��ġ�� ���ο� Ÿ�� ���������� ����
                Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, towerLayer);
                foreach (Collider collider in colliders)
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.transform.position = targetPositionForward; // Ÿ���� ���ο� Ÿ�� ���������� �̵�
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        // ���� �ݰ��� Gizmo�� �ð�ȭ
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        // ������ ��ġ�� Gizmo�� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), 0.5f);
    }
}
