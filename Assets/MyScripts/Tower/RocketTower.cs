using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // 탐지 범위 (기본 설정)
    [SerializeField] private float splashRadius = 10f; // 폭발 범위 (기본 설정)
    [SerializeField] private float damageOverTime = 5f; // 지속 데미지
    [SerializeField] private float damageDuration = 5f; // 지속 데미지 시간
    [SerializeField] private Transform rocketLaunchPoint; // 로켓 발사 지점
    private bool isAttacking = false;
    [SerializeField] private float attackConeAngle = 45f; // 공격 각도

    void Awake()
    {
        towerAttackPower = 20; // 타워 공격력 설정
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;

        SetRange(detectionRange); // 탐지 범위 초기화
    }

    void Update()
    {
        DetectEnemiesInRange();
    }

    public override void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && !isAttacking)
            {
                StartCoroutine(AttackRoutine(hitCollider.transform));
            }
        }
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        isAttacking = true;
        RocketAttack(target); // 로켓 공격 실행
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
    }

    private void RocketAttack(Transform target)
    {
        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

        Debug.Log("로켓 발사! " + target.name + "이(가) 타겟되었습니다!");

        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, splashRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Vector3 toCollider = (hitCollider.transform.position - rocketLaunchPoint.position).normalized;
                float angle = Vector3.Angle(direction, toCollider);

                if (angle <= attackConeAngle / 2)
                {
                    EnemyBase enemyHp = hitCollider.GetComponent<EnemyBase>();
                    if (enemyHp != null)
                    {
                        enemyHp.TakeDamage(towerAttackPower); // 기본 데미지 적용
                        StartCoroutine(ApplyDamageOverTime(enemyHp)); // 지속 데미지 적용
                    }
                }
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(EnemyBase enemyHp)
    {
        float elapsed = 0f;
        while (elapsed < damageDuration)
        {
            if (enemyHp != null && enemyHp.gameObject.activeSelf)
            {
                enemyHp.TakeDamage(damageOverTime); // 지속 데미지 적용
                Debug.Log("지속 데미지 적용: " + enemyHp.name);
            }
            else
            {
                break; // enemyHp가 null이거나 비활성화된 경우 루프 종료
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("탐지 범위 설정: " + detectionRange);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Vector3 forward = rocketLaunchPoint.forward * splashRadius;
        Vector3 right = Quaternion.Euler(0, attackConeAngle / 2, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -attackConeAngle / 2, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + right);
        Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + left);
    }

}
