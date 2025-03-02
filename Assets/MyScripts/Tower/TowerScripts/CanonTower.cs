//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CanonTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 8f; // 감지 범위
//    private bool isAttacking = false;
//    [SerializeField] private Transform cannonTowerTransform;
//    private List<Transform> targets = new List<Transform>();
//    private Transform currentTarget;
//    private ILineRendererStrategy lineRendererStrategy;
//    public int segments = 50;

//    void Awake()
//    {
//        towerAttackPower = 40;
//        towerPenetrationPower = 25;
//        criticalHitRate = 0.05f;
//        attackSpeed = 0.7f;
//        installationCost = 8;
//        isAttackUp = false;

//        // 라인 렌더러 전략 초기화
//        lineRendererStrategy = new CircleRendererStrategy();
//    }

//    void Start()
//    {
//        // 라인 렌더러 전략을 사용하여 감지 범위 표시
//        lineRendererStrategy.Setup(gameObject);
//        lineRendererStrategy.GeneratePattern(gameObject, transform.position, cannonTowerTransform, segments, detectionRange, 0);

//        SetRange(detectionRange);
//    }

//    void Update()
//    {
//        DetectEnemiesInRange();
//        if (targets.Count > 0)
//        {
//            RotateTowardsTarget();
//        }
//        else
//        {
//            currentTarget = null; // 감지 범위 내에 적이 없으면 타겟 초기화
//        }
//    }

//    public override void TowerPowUp()
//    {
//        towerAttackPower *= 2;
//        Debug.Log("캐논 타워의 공격력이 상승하였습니다: " + towerAttackPower);
//    }

//    public override void TowerAttack(List<Transform> targets)
//    {
//        if (targets.Count > 0)
//        {
//            // 현재 타겟이 없는 경우 첫 번째 타겟을 설정
//            if (currentTarget == null)
//            {
//                currentTarget = targets[0];
//            }

//            if (currentTarget != null)
//            {
//                EnemyBase enemyHp = currentTarget.GetComponent<EnemyBase>();
//                if (enemyHp != null)
//                {
//                    enemyHp.TakeDamage(towerAttackPower);
//                    Debug.Log("캐논 타워가 " + currentTarget.name + "에게 피해를 입혔습니다! 공격력: " + towerAttackPower);
//                }
//            }
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//    }

//    public override void DetectEnemiesInRange()
//    {
//        targets.Clear();
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform);
//            }
//        }

//        // 마지막에 담긴 적을 타겟으로 설정
//        if (targets.Count > 0)
//        {
//            currentTarget = targets[targets.Count - 1];
//        }
//    }

//    private void RotateTowardsTarget()
//    {
//        if (currentTarget != null)
//        {
//            Vector3 direction = (currentTarget.position - cannonTowerTransform.position).normalized;
//            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
//            cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);

//            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
//            {
//                StartCoroutine(AttackRoutine(targets));
//            }
//        }
//    }

//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true;
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets);
//            yield return new WaitForSeconds(attackSpeed);

//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

//            if (targets.Count > 0)
//            {
//                RotateTowardsTarget();
//            }
//        }
//        isAttacking = false;
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 8f;
    private bool isAttacking = false;
    [SerializeField] private Transform cannonTowerTransform;
    [SerializeField] private ParticleSystem attackParticleSystem; // 파티클 시스템 참조
    private List<Transform> targets = new List<Transform>();
    private Transform currentTarget;
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 50;

    void Awake()
    {
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;
        isAttackUp = false;

        lineRendererStrategy = new CircleRendererStrategy();
    }

    void Start()
    {
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, cannonTowerTransform, segments, detectionRange, 0);

        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();
        if (targets.Count > 0)
        {
            RotateTowardsTarget();
        }
        else
        {
            currentTarget = null;
        }
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2;
        Debug.Log("캐논 타워의 공격력이 상승하였습니다: " + towerAttackPower);
    }

    public override void TowerAttack(List<Transform> targets)
    {
        if (targets.Count > 0)
        {
            if (currentTarget == null)
            {
                currentTarget = targets[0];
            }

            if (currentTarget != null)
            {
                EnemyBase enemyHp = currentTarget.GetComponent<EnemyBase>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("캐논 타워가 " + currentTarget.name + "에게 피해를 입혔습니다! 공격력: " + towerAttackPower);

                    // 파티클 효과 재생
                    if (attackParticleSystem != null)
                    {
                        attackParticleSystem.Play();
                    }
                }
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
    }

    public override void DetectEnemiesInRange()
    {
        targets.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
            }
        }

        if (targets.Count > 0)
        {
            currentTarget = targets[targets.Count - 1];
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - cannonTowerTransform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);

            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
            {
                StartCoroutine(AttackRoutine(targets));
            }
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets);
            yield return new WaitForSeconds(attackSpeed);

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            if (targets.Count > 0)
            {
                RotateTowardsTarget();
            }
        }
        isAttacking = false;
    }
}
