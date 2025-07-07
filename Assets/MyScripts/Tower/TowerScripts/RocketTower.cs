using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float splashRadius = 10f; // 스플래시 반경
    [SerializeField] private float damageOverTime = 5f; // 지속 데미지
    [SerializeField] private float damageDuration = 5f; // 지속 데미지 시간
    [SerializeField] private Transform rocketLaunchPoint; // 로켓 발사 위치
    private bool isAttacking = false; // 공격 중인지 여부
    [SerializeField] private float attackConeAngle = 45f; // 공격 범위 각도

    void Awake()
    {
        // 타워 기본 속성 초기화
        towerAttackPower = 20;
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;
        SetRange(detectionRange); // 감지 범위 초기화

    }

    protected override void Start()
    {
        rangeColor = Color.blue; // 로켓 타워 - 주황
        detectionRange = 10f;
        rangeType = RangeType.Fan;

        base.Start(); // TowerBase의 SetupRangeDecal 호출

        SetRange(detectionRange);

        // 기존 LineRenderer 코드 제거
    }

    void Update()
    {
        DetectEnemiesInRange();

        // 범위가 표시 중이고 타워가 회전했다면 범위도 회전
        if (isRangeVisible && rangeDecal != null)
        {
            rangeDecal.transform.localRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
        }
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
        TowerAttack(new List<Transform> { target }); // 상속받은 TowerAttack 메서드 사용
        yield return new WaitForSeconds(attackSpeed); // 공격 주기 대기
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            Vector3 targetPosition = target.position;
            Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

            Debug.Log("로켓 발사! " + target.name + "에게 타격을 입혔습니다!");

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
                            enemyHp.TakeDamage(towerAttackPower); // 기본 데미지 적용
                            StartCoroutine(ApplyDamageOverTime(enemyHp)); // 지속 데미지 적용
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
                enemyHp.TakeDamage(damageOverTime); // 지속 데미지 적용
                Debug.Log("지속 데미지 적용: " + enemyHp.name);
            }
            else
            {
                break; // 적이 사라지거나 비활성화된 경우 중지
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("감지 범위 설정: " + detectionRange);
    }

    public override void TowerPowUp()
    {
        // 파워업 기능 추가
        towerAttackPower *= 2;
        Debug.Log("로켓 타워의 공격력이 강화되었습니다: " + towerAttackPower);
    }
}
