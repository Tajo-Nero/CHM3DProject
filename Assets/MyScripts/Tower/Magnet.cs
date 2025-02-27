//using System.Collections.Generic;
//using UnityEngine;

//public class Magnet : MonoBehaviour
//{
//    [SerializeField] private LayerMask towerPowUpLayer;
//    [SerializeField] private LayerMask playerLayer;
//    [SerializeField] public float towerDetectionRadius = 3f;
//    [SerializeField] public float playerDetectionRadius = 5f;
//    public float stopDistance = 0.1f;
//    [SerializeField] private float magnetForce = 20f;

//    private bool isPlayerInRange = false;
//    private Collider[] towerColliders;
//    private List<Rigidbody> affectedRigidbodies = new List<Rigidbody>(); // ������ �޴� Rigidbody ���
//    private Dictionary<Rigidbody, Vector3> originalPositions = new Dictionary<Rigidbody, Vector3>(); // ���� ��ġ ����
//    private Dictionary<Rigidbody, Quaternion> originalRotations = new Dictionary<Rigidbody, Quaternion>(); // ���� ȸ�� ����
//    public TowerGenerator towerGenerator;
//    private void FixedUpdate()
//    {
//        ApplyMagnetForce();
//    }

//    public void ApplyMagnetForce()
//    {
//        CheckPlayerInRange();
//        DetectTowers();
//        // ���� ��ġ�� ȸ���� �����մϴ�.
//    }

//    public void CheckPlayerInRange()
//    {
//        Collider[] playerColliders = Physics.OverlapSphere(transform.position, playerDetectionRadius, playerLayer);
//        isPlayerInRange = playerColliders.Length > 0;

//        if (isPlayerInRange)
//        {
//            Debug.Log("�÷��̾� ���� ���� ����");
//            if (towerColliders != null)
//            {
//                ApplyGravity(towerColliders);
//            }
//        }
//        else
//        {
//            Debug.Log("�÷��̾� ���� �ܿ� ����");
//            if (towerColliders != null)
//            {
//                StopGravity(towerColliders);
//            }
//        }
//    }

//    public void DetectTowers()
//    {
//        if (isPlayerInRange)
//        {
//            // ���� ��ġ�� ȸ�� �ʱ�ȭ
//            originalPositions.Clear();
//            originalRotations.Clear();

//            towerColliders = Physics.OverlapSphere(transform.position, towerDetectionRadius, towerPowUpLayer);
//            Debug.Log("ž ������: " + towerColliders.Length + "��");
//            MoveTowersToCenter(towerColliders);
//            ApplyGravity(towerColliders);
//            foreach (Collider collider in towerColliders)
//            {
//                Rigidbody rb = collider.GetComponent<Rigidbody>();
//                if (rb != null && !originalPositions.ContainsKey(rb))
//                {
//                    originalPositions[rb] = rb.transform.position;
//                    originalRotations[rb] = rb.transform.rotation;

//                }
//            }

//        }
//        else
//        {
//            Debug.Log("ž �������� ����");
//        }
//    }

//    public void MoveTowersToCenter(Collider[] towerColliders)
//    {
//        if (towerColliders != null)
//        {
//            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y+1.9f, transform.position.z);
//            foreach (Collider collider in towerColliders)
//            {
//                Rigidbody rb = collider.GetComponent<Rigidbody>();
//                if (rb != null)
//                {
//                    float distance = Vector3.Distance(rb.transform.position, targetPosition);
//                    if (distance > stopDistance)
//                    {
//                        rb.transform.position = Vector3.MoveTowards(rb.transform.position, targetPosition, Time.deltaTime * magnetForce);
//                        Debug.Log("ž �̵� ��: " + rb.gameObject.name);
//                    }
//                    else
//                    {
//                        rb.transform.position = targetPosition;
//                        Debug.Log("ž �̵� �Ϸ�: " + rb.gameObject.name);
//                    }
//                }
//            }
//        }
//    }

//    public void ApplyGravity(Collider[] towerColliders)
//    {
//        foreach (Collider collider in towerColliders)
//        {
//            Rigidbody rb = collider.GetComponent<Rigidbody>();
//            if (rb != null && !affectedRigidbodies.Contains(rb))
//            {
//                rb.useGravity = true;
//                rb.isKinematic = false;
//                Debug.Log("�߷� �����: " + rb.gameObject.name);
//                affectedRigidbodies.Add(rb); // ������ �޴� Rigidbody ��Ͽ� �߰�
//            }
//        }
//    }

//    public void StopGravity(Collider[] towerColliders)
//    {
//        foreach (Collider collider in towerColliders)
//        {
//            if (collider != null)
//            {
//                Rigidbody rb = collider.GetComponent<Rigidbody>();
//                if (rb != null && affectedRigidbodies.Contains(rb))
//                {
//                    rb.useGravity = false;
//                    rb.isKinematic = true;
//                    Debug.Log("�߷� ������: " + rb.gameObject.name);

//                    // ���� ��ġ�� ȸ������ �ǵ����ϴ�.
//                    if (originalPositions.ContainsKey(rb) && originalRotations.ContainsKey(rb))
//                    {
//                        Vector3 originalPosition = originalPositions[rb];
//                        rb.transform.position = new Vector3(0,0,0); // X�� Y�� �ʱ�ȭ�ϰ� Z�� ����
//                        rb.transform.rotation = originalRotations[rb];

//                        // �������� �����̼� Y ���� �ʱ�ȭ�մϴ�.
//                        Vector3 eulerRotation = rb.transform.rotation.eulerAngles;
//                        eulerRotation.y = 0; // �Ǵ� �ʱ�ȭ�� ������ ����
//                        rb.transform.rotation = Quaternion.Euler(eulerRotation);

//                        Debug.Log("���� ��ġ�� ȸ������ �ǵ���: " + rb.gameObject.name);
//                    }
//                }
//            }
//        }
//    }

//    private void OnDrawGizmos()
//    {
//        // �÷��̾� ���� ���� �׸���
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

//        // Ÿ�� ���� ���� �׸���
//        Gizmos.color = Color.blue;
//        Gizmos.DrawWireSphere(transform.position, towerDetectionRadius);

//        // ���� �Ÿ� �׸���
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, stopDistance);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] private LayerMask towerPowUpLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public float towerDetectionRadius = 3f;
    [SerializeField] public float playerDetectionRadius = 5f;
    public float stopDistance = 0.1f;
    [SerializeField] private float magnetForce = 20f;
    [SerializeField] private float detectionInterval = 1f; // Ÿ�� ���� ����

    private bool isPlayerInRange = false;
    private Collider[] towerColliders;
    private List<Rigidbody> affectedRigidbodies = new List<Rigidbody>();
    private Dictionary<Rigidbody, Vector3> originalPositions = new Dictionary<Rigidbody, Vector3>();
    private Dictionary<Rigidbody, Quaternion> originalRotations = new Dictionary<Rigidbody, Quaternion>();


    private void Start()
    {
        StartCoroutine(DetectTowersPeriodically());
    }

    private void FixedUpdate()
    {
        ApplyMagnetForce();
    }

    public void ApplyMagnetForce()
    {
        CheckPlayerInRange();
        DetectTowers();
    }

    public void CheckPlayerInRange()
    {
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, playerDetectionRadius, playerLayer);
        isPlayerInRange = playerColliders.Length > 0;

        if (isPlayerInRange)
        {
            
            if (towerColliders != null)
            {
                ApplyGravity(towerColliders);
            }
        }
        else
        {
            
            if (towerColliders != null)
            {
                StopGravity(towerColliders);
            }
            // �÷��̾ ������ ����� �� ��ųʸ� �ʱ�ȭ
            originalPositions.Clear();
            originalRotations.Clear();
            towerColliders = null; // Ÿ�� �迭�� �ʱ�ȭ
        }
    }

    public void DetectTowers()
    {
        if (isPlayerInRange)
        {
            originalPositions.Clear();
            originalRotations.Clear();

            towerColliders = Physics.OverlapSphere(transform.position, towerDetectionRadius, towerPowUpLayer);

            MoveTowersToCenter(towerColliders);
            ApplyGravity(towerColliders);

            foreach (Collider collider in towerColliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null && !originalPositions.ContainsKey(rb))
                {
                    originalPositions[rb] = rb.transform.position;
                    originalRotations[rb] = rb.transform.rotation;
                }
                //// Towers �±װ� ������ Ÿ���� ���ݷ��� 2��� ������Ű��
                if (collider.CompareTag("Towers"))
                {
                    TowerBase tower = collider.GetComponent<TowerBase>();
                    if (tower != null)
                    {
                        tower.isAttackUp = true;
                    }
                
                }
            }
        }

    }

    public void MoveTowersToCenter(Collider[] towerColliders)
    {
        if (towerColliders != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 1.9f, transform.position.z);
            foreach (Collider collider in towerColliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float distance = Vector3.Distance(rb.transform.position, targetPosition);
                    if (distance > stopDistance)
                    {
                        rb.transform.position = Vector3.MoveTowards(rb.transform.position, targetPosition, Time.deltaTime * magnetForce);

                    }
                    else
                    {
                        rb.transform.position = targetPosition;

                    }
                }
            }
        }
    }

    public void ApplyGravity(Collider[] towerColliders)
    {
        foreach (Collider collider in towerColliders)
        {
            if (collider != null)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null && !affectedRigidbodies.Contains(rb))
                {
                    rb.useGravity = true;
                    rb.isKinematic = false;

                    affectedRigidbodies.Add(rb);
                }
            }
        }
    }


    public void StopGravity(Collider[] towerColliders)
    {
        foreach (Collider collider in towerColliders)
        {
            if (collider != null)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null && affectedRigidbodies.Contains(rb))
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;


                    if (originalPositions.ContainsKey(rb) && originalRotations.ContainsKey(rb))
                    {
                        rb.transform.position = new Vector3(0, 0, 0);
                        rb.transform.rotation = originalRotations[rb];

                        // �������� �����̼� Y ���� �ʱ�ȭ�մϴ�.
                        Vector3 eulerRotation = rb.transform.rotation.eulerAngles;
                        eulerRotation.y = 0; // �Ǵ� �ʱ�ȭ�� ������ ����
                        rb.transform.rotation = Quaternion.Euler(eulerRotation);

                    }
                }
            }
        }
    }


    private IEnumerator DetectTowersPeriodically()
    {
        while (true)
        {
            DetectTowers();
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, towerDetectionRadius);

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, stopDistance);
    //}
}
