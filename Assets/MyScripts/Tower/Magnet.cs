
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
    [SerializeField] private float detectionInterval = 1f; // 타워 감지 간격

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
            // 플레이어가 범위를 벗어났을 때 딕셔너리 초기화
            originalPositions.Clear();
            originalRotations.Clear();
            towerColliders = null; // 타워 배열도 초기화
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
                //// Towers 태그가 설정된 타워의 공격력을 2배로 증가시키기
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

                        // 오리지널 로테이션 Y 값만 초기화합니다.
                        Vector3 eulerRotation = rb.transform.rotation.eulerAngles;
                        eulerRotation.y = 0; // 또는 초기화할 값으로 설정
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
