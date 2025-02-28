
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CanonTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 8f; // 탐지 범위
//    private bool isAttacking = false; // 공격 중인지 여부
//    [SerializeField] private Transform cannonTowerTransform; // 캐논 타워 위치
//    private List<Transform> targets = new List<Transform>(); // 타겟 리스트
//    private LineRenderer lineRenderer; // 라인 렌더러
//    public int segments = 50; // 세그먼트 수

//    void Awake()
//    {
//        //탑의 기본 능력치
//        towerAttackPower = 40;
//        towerPenetrationPower = 25;
//        criticalHitRate = 0.05f;
//        attackSpeed = 0.7f;
//        installationCost = 8;
//        isAttackUp = false;
//    }

//    void Start()
//    {
//        lineRenderer = GetComponent<LineRenderer>();

//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }

//        lineRenderer.positionCount = segments + 1; // 추가된 세그먼트 수 설정
//        lineRenderer.loop = true;
//        lineRenderer.useWorldSpace = false;
//        lineRenderer.startWidth = 1f;
//        lineRenderer.endWidth = 1f;


//        // 탐지 범위 설정 후 원형 생성
//        SetRange(detectionRange);
//    }

//    void Update()
//    {
//        DetectEnemiesInRange();
//        if (targets.Count > 0)
//        {
//            RotateTowardsTarget();
//        }
//    }

//    public override void TowerPowUp()
//    {
//        base.TowerPowUp();
//    }

//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true;
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets); // 타겟 리스트 첫 번째 타겟 공격
//            yield return new WaitForSeconds(attackSpeed);

//            // 타겟 리스트 갱신
//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

//            // 타겟 리스트에 타겟이 남아있으면 다시 회전
//            if (targets.Count > 0)
//            {
//                RotateTowardsTarget();
//            }
//        }
//        lineRenderer.enabled = false;
//        isAttacking = false;
//    }

//    public override void TowerAttack(List<Transform> targets)
//    {
//        if (targets.Count > 0)
//        {
//            Transform target = targets[0];
//            if (target != null)
//            {
//                EnemyBase enemyHp = target.GetComponent<EnemyBase>();
//                if (enemyHp != null)
//                {
//                    enemyHp.TakeDamage(towerAttackPower);
//                    Debug.Log("캐논 타워가 " + target.name + "에게 공격을 가했습니다! 공격력: " + towerAttackPower);
//                }
//            }
//        }
//    }

//    void OnDrawGizmos()
//    {
//        if (targets.Count > 0)
//        {
//            Gizmos.color = Color.yellow;
//            Gizmos.DrawLine(transform.position, targets[0].position);
//        }
//    }


//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        GenerateCircle(detectionRange); // 탐지 범위 설정 후 원형 생성
//    }

//    public override void DetectEnemiesInRange()
//    {
//        targets.Clear(); // 타겟 리스트 초기화
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform); // 탐지된 적을 타겟 리스트에 추가
//            }
//        }
//    }

//    private void RotateTowardsTarget()
//    {
//        if (targets.Count > 0)
//        {
//            Vector3 direction = (targets[0].position - cannonTowerTransform.position).normalized;
//            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
//            cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);

//            // 회전 완료 시 공격 시작
//            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
//            {
//                StartCoroutine(AttackRoutine(targets));
//            }
//        }
//    }

//    private void GenerateCircle(float radius)
//    {
//        lineRenderer.positionCount = segments + 1; // 세그먼트 수 설정

//        float angleStep = 360f / segments;
//        for (int i = 0; i <= segments; i++)
//        {
//            float angle = i * angleStep * Mathf.Deg2Rad;
//            float x = Mathf.Cos(angle) * radius/2;
//            float z = Mathf.Sin(angle) * radius/2;
//            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 8f; // 탐지 범위
    private bool isAttacking = false; // 공격 중인지 여부
    [SerializeField] private Transform cannonTowerTransform; // 캐논 타워 위치
    private List<Transform> targets = new List<Transform>(); // 타겟 목록
    private LineRenderer lineRenderer; // 라인 렌더러
    public int segments = 50; // 세그먼트 수
    private ILineRendererStrategy lineRendererStrategy; // 라인 렌더러 전략

    void Awake()
    {
        // 타워 기본 능력치
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;
        isAttackUp = false;
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 전략 초기화 및 설정
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 1; // 세그먼트 수 설정
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        // 탐지 범위 설정
        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();
        if (targets.Count > 0)
        {
            RotateTowardsTarget();
        }
    }

    public override void TowerPowUp()
    {
        base.TowerPowUp();
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets); // 타겟 목록 첫 번째 타겟 공격
            yield return new WaitForSeconds(attackSpeed);

            // 타겟 목록 갱신
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            // 타겟 목록에 타겟이 남아 있으면 다시 회전
            if (targets.Count > 0)
            {
                RotateTowardsTarget();
            }
        }
        lineRenderer.enabled = false;
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        if (targets.Count > 0)
        {
            Transform target = targets[0];
            if (target != null)
            {
                EnemyBase enemyHp = target.GetComponent<EnemyBase>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("캐논 타워가 " + target.name + "에게 데미지를 입혔습니다! 데미지: " + towerAttackPower);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (targets.Count > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targets[0].position);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        lineRendererStrategy.GeneratePattern(lineRenderer, transform.position, transform, segments, detectionRange, detectionRange); // 탐지 범위 설정
    }

    public override void DetectEnemiesInRange()
    {
        targets.Clear(); // 타겟 목록 초기화
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform); // 탐지된 적을 타겟 목록에 추가
            }
        }
    }

    private void RotateTowardsTarget()
    {
        if (targets.Count > 0)
        {
            Vector3 direction = (targets[0].position - cannonTowerTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);

            // 회전 완료 후 공격 시작
            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
            {
                StartCoroutine(AttackRoutine(targets));
            }
        }
    }
}

