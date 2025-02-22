using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;//일단은 10으로 설정하고 나중에 스타트에 넣자
    private Transform target;//타겟을 바라보게 할
    private bool isAttacking = false;//공격판단
    [SerializeField] private Transform cannonTowerTransform;//캐논타워의 회전할 뚜껑 적을 바라보게 할거임

    void Start()
    {
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;

        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();        
        if (target != null)
        {
            RotateTowardsTarget();
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        while (target != null)
        {
            TowerAttack();
            yield return new WaitForSeconds(attackSpeed);
        }
        isAttacking = false;
    }

    public override void TowerAttack()
    {
        if (target != null)
        {
            EnemyHp enemyHp = target.GetComponent<EnemyHp>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower);
                Debug.Log("캐논 타워가 " + target.name + "을(를) 공격합니다! 공격력: " + towerAttackPower);
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("탐지 범위 설정됨: " + detectionRange);
    }

    public override void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                target = hitCollider.transform;
                Debug.Log("적 발견: " + hitCollider.name);
                break;
            }
        }

        if (hitColliders.Length == 0)
        {
            target = null;
        }
    }
    private void RotateTowardsTarget()
    {
        Vector3 direction = (target.position - cannonTowerTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);
        // 적을 바라보았을 때 공격 시작
        if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
