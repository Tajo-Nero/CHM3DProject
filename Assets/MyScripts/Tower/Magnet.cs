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
    // 원래 포지션을 담을 딕셔너리
    private Dictionary<Rigidbody, Vector3> originalPositions = new Dictionary<Rigidbody, Vector3>();
    // 원래 로테이션을 담을 딕셔너리
    private Dictionary<Rigidbody, Quaternion> originalRotations = new Dictionary<Rigidbody, Quaternion>();

    private void Start()
    {
        StartCoroutine(DetectTowersPeriodically());
    }

    private void FixedUpdate()
    {
        // ⭐ 수정: 불필요한 중복 호출 제거
        CheckPlayerInRange();

        // 플레이어가 범위에 있을 때만 타워 이동 처리
        if (isPlayerInRange && towerColliders != null)
        {
            MoveTowersToCenter(towerColliders);
        }
    }

    // 플레이어가 감지에 들어오면 중력 적용 시키고, 벗어나면 중력을 멈춤
    public void CheckPlayerInRange()
    {
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, playerDetectionRadius, playerLayer);
        bool wasPlayerInRange = isPlayerInRange;
        isPlayerInRange = playerColliders.Length > 0;

        // 플레이어 상태가 변경된 경우에만 처리
        if (isPlayerInRange != wasPlayerInRange)
        {
            if (isPlayerInRange)
            {
                // 플레이어가 들어왔을 때
                DetectTowers(); // 타워 탐지 및 원본 위치 저장
                if (towerColliders != null)
                {
                    ApplyGravity(towerColliders);
                }
            }
            else
            {
                // 플레이어가 벗어났을 때
                if (towerColliders != null)
                {
                    StopGravity(towerColliders);
                }
                // 딕셔너리 초기화
                originalPositions.Clear();
                originalRotations.Clear();
                affectedRigidbodies.Clear();
                towerColliders = null; // 타워 배열도 초기화
            }
        }
    }

    // 중력 적용 시키고 타워태그를 가진 타워의 공격력 2배 증가
    public void DetectTowers()
    {
        // ⭐ 수정: 중복 초기화 방지
        if (!isPlayerInRange) return;

        towerColliders = Physics.OverlapSphere(transform.position, towerDetectionRadius, towerPowUpLayer);

        foreach (Collider collider in towerColliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null && !originalPositions.ContainsKey(rb))
            {
                // ⭐ 중요: 원본 위치와 회전 저장
                originalPositions[rb] = rb.transform.position;
                originalRotations[rb] = rb.transform.rotation;
            }

            // Towers 태그가 설정된 타워의 공격력을 2배로 증가시키기
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

    // 끌어오고자 하는 중심지 설정
    public void MoveTowersToCenter(Collider[] towerColliders)
    {
        if (towerColliders != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y + 1.9f, transform.position.z);
            foreach (Collider collider in towerColliders)
            {
                // null 체크 추가
                if (collider == null) continue;

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

    // 중력 설정
    public void ApplyGravity(Collider[] towerColliders)
    {
        foreach (Collider collider in towerColliders)
        {
            if (collider != null)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null && !affectedRigidbodies.Contains(rb))
                {
                    rb.useGravity = true; // 중력적용
                    rb.isKinematic = false; // 물리반응에 따라 움직이게 설정

                    affectedRigidbodies.Add(rb);
                }
            }
        }
    }

    // 중력 멈추기
    public void StopGravity(Collider[] towerColliders)
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        foreach (Collider collider in towerColliders)
        {
            if (collider != null)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null && affectedRigidbodies.Contains(rb))
                {
                    rb.useGravity = false;
                    rb.isKinematic = true;

                    // ⭐ 핵심 수정: 플레이어 forward 방향으로 이동
                    if (player != null)
                    {
                        // 플레이어 앞쪽 위치 계산 (forward 방향으로 약간 떨어진 곳)
                        Vector3 playerForwardPosition = player.transform.position + player.transform.forward * 2f;
                        playerForwardPosition.y = rb.transform.position.y; // Y 높이는 유지

                        rb.transform.position = playerForwardPosition;

                        // 오리지널 로테이션 Y 값만 초기화하기
                        if (originalRotations.ContainsKey(rb))
                        {
                            Vector3 eulerRotation = rb.transform.rotation.eulerAngles;
                            eulerRotation.y = 0; // Y 회전값 초기화
                            rb.transform.rotation = Quaternion.Euler(eulerRotation);
                        }
                    }
                    else
                    {
                        // 플레이어를 찾을 수 없는 경우 원본 위치로 복원
                        if (originalPositions.ContainsKey(rb))
                        {
                            rb.transform.position = originalPositions[rb];
                        }
                        Debug.LogWarning("플레이어를 찾을 수 없어 원본 위치로 복원합니다.");
                    }

                    affectedRigidbodies.Remove(rb);
                }
            }
        }
    }

    private IEnumerator DetectTowersPeriodically()
    {
        while (true)
        {
            // ⭐ 수정: 플레이어가 범위에 있을 때만 주기적 탐지
            if (isPlayerInRange)
            {
                DetectTowers();
            }
            yield return new WaitForSeconds(detectionInterval);
        }
    }

    // 디버그용 Gizmos (주석 해제하여 시각적 확인 가능)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, towerDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}