using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // 탐지 범위 (부채꼴 길이)
    [SerializeField] private float splashRadius = 10f; // 스플래쉬 범위 (부채꼴 길이)
    [SerializeField] private float damageOverTime = 5f; // 지속 데미지
    [SerializeField] private float damageDuration = 5f; // 지속 데미지 시간
    [SerializeField] private Transform rocketLaunchPoint; // 로켓 발사 지점
    private bool isAttacking = false;
    [SerializeField] private float attackConeAngle = 45f; // 부채꼴 각도
    [SerializeField] private Transform towerTransform; // 타워 트랜스폼

    void Start()
    {
        towerAttackPower = 20; // 어택 파워 설정
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;

        SetRange(detectionRange); // 탐지 범위를 부채꼴 길이와 같게 설정
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
        // 로켓 발사 위치에서 목표 지점까지의 거리 계산
        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

        // 목표 지점에 로켓 발사
        Debug.Log("로켓 발사! " + target.name + "에 타격되었습니다!");

        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, splashRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                Vector3 toCollider = (hitCollider.transform.position - rocketLaunchPoint.position).normalized;
                float angle = Vector3.Angle(direction, toCollider);

                // 적이 원뿔 범위 내에 있는지 확인
                if (angle <= attackConeAngle / 2)
                {
                    EnemyAI enemyHp = hitCollider.GetComponent<EnemyAI>();
                    if (enemyHp != null)
                    {
                        enemyHp.TakeDamage(towerAttackPower); // 기본 데미지 적용
                        StartCoroutine(ApplyDamageOverTime(enemyHp)); // 지속 데미지 적용
                    }
                }
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(EnemyAI enemyHp)
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
                break; // enemyHp가 null이거나 비활성화된 경우 코루틴 종료
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("탐지 범위 설정됨: " + detectionRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 왼쪽 끝과 오른쪽 끝 표시
        Vector3 forward = rocketLaunchPoint.forward * splashRadius;
        Vector3 right = Quaternion.Euler(0, attackConeAngle / 2, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -attackConeAngle / 2, 0) * forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + right);
        Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + left);

    }
}
