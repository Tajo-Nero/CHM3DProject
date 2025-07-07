using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    private bool isAttacking = false;
    [SerializeField] private Transform cannonTowerTransform;
    private List<Transform> targets = new List<Transform>();

    void Awake()
    {
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;
        isAttackUp = false;

    }

    protected override void Start()
    {
        rangeColor = Color.red; // 공격 타워 - 빨강
        detectionRange = 8f;
        rangeType = RangeType.Circle;

        // 기존 LineRenderer 코드 제거하고 Decal로 교체
        base.Start(); // TowerBase의 SetupRangeDecal 호출

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
        towerAttackPower *= 2;
        Debug.Log("캐논 타워의 공격력이 강화되었습니다: " + towerAttackPower);
    }

    public override void TowerAttack(List<Transform> targets)
    {
        if (targets.Count > 0)
        {
            Transform target = targets[0];
            if (target != null)
            {
                EnemyPathFollower enemyHp = target.GetComponent<EnemyPathFollower>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("캐논 타워가 " + target.name + "에게 데미지를 입혔습니다! 공격력: " + towerAttackPower);
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
    }

    private void RotateTowardsTarget()
    {
        if (targets.Count > 0)
        {
            Vector3 direction = (targets[0].position - cannonTowerTransform.position).normalized;
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
