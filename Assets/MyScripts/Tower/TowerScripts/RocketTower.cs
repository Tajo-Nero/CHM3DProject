using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float splashRadius = 10f;
    [SerializeField] private float damageOverTime = 5f;
    [SerializeField] private float damageDuration = 5f;
    [SerializeField] private Transform rocketLaunchPoint;
    private bool isAttacking = false;
    [SerializeField] private float attackConeAngle = 45f;

    void Awake()
    {
        towerAttackPower = 20;
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;
    }

    protected override void Start()
    {
        detectionRange = 10f;

        base.Start(); // TowerBase의 Start 호출

        // 로켓 타워는 부채꼴 범위
        if (rangeDisplay != null)
        {
            rangeDisplay.shape = TowerRangeDisplay.RangeShape.Fan;
            rangeDisplay.fanAngle = attackConeAngle;
            rangeDisplay.UpdateRangeMesh();
        }

        SetRange(detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange();
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> detectedEnemies = new List<Transform>();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // 부채꼴 범위 체크
                Vector3 directionToTarget = (hitCollider.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToTarget);

                // fanAngle의 절반 이내에 있는지 체크
                float halfAngle = rangeDisplay != null ? rangeDisplay.fanAngle / 2f : attackConeAngle / 2f;

                if (angle <= halfAngle)
                {
                    detectedEnemies.Add(hitCollider.transform);
                }
            }
        }

        if (detectedEnemies.Count > 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine(detectedEnemies[0])); // 첫 번째 적만 공격
        }
    }

    // 디버그용 Gizmo
    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.grey;
        float halfAngle = rangeDisplay != null ? rangeDisplay.fanAngle / 2f : attackConeAngle / 2f;

        // 부채꼴 시각화
        Vector3 forward = transform.forward * detectionRange;
        Vector3 right = Quaternion.Euler(0, halfAngle, 0) * forward;
        Vector3 left = Quaternion.Euler(0, -halfAngle, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawLine(transform.position, transform.position + right);
        Gizmos.DrawLine(transform.position, transform.position + left);
    }
    private IEnumerator AttackRoutine(Transform target)
    {
        isAttacking = true;
        TowerAttack(new List<Transform> { target });
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            Vector3 targetPosition = target.position;
            Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

            Debug.Log("로켓 발사! " + target.name + "에게 공격을 입혔습니다!");

            Collider[] hitColliders = Physics.OverlapSphere(targetPosition, splashRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    Vector3 toCollider = (hitCollider.transform.position - rocketLaunchPoint.position).normalized;
                    float angle = Vector3.Angle(direction, toCollider);

                    if (angle <= attackConeAngle / 2)
                    {
                        EnemyPathFollower enemyHp = hitCollider.GetComponent<EnemyPathFollower>();
                        if (enemyHp != null)
                        {
                            enemyHp.TakeDamage(towerAttackPower);
                            StartCoroutine(ApplyDamageOverTime(enemyHp));
                        }
                    }
                }
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(EnemyPathFollower enemyHp)
    {
        float elapsed = 0f;
        while (elapsed < damageDuration)
        {
            if (enemyHp != null && enemyHp.gameObject.activeSelf)
            {
                enemyHp.TakeDamage(damageOverTime);
                Debug.Log("지속 데미지 적용: " + enemyHp.name);
            }
            else
            {
                break;
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        SetRangeSize(range); // TowerBase의 메서드 호출
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2;
        Debug.Log("로켓 타워의 공격력이 강화되었습니다: " + towerAttackPower);
    }
}