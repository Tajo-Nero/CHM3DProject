//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static UnityEngine.Rendering.HableCurve;

//public class RocketTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 10f; // 탐지 범위 (기본 설정)
//    [SerializeField] private float splashRadius = 10f; // 폭발 범위 (기본 설정)
//    [SerializeField] private float damageOverTime = 5f; // 지속 데미지
//    [SerializeField] private float damageDuration = 5f; // 지속 데미지 시간
//    [SerializeField] private Transform rocketLaunchPoint; // 로켓 발사 지점
//    private bool isAttacking = false;
//    [SerializeField] private float attackConeAngle = 45f; // 공격 각도

//    private LineRenderer lineRenderer;
//    public int segments = 20;    
//    void Awake()
//    {
//        towerAttackPower = 20; // 타워 공격력 설정
//        towerPenetrationPower = 5;
//        criticalHitRate = 0.05f;
//        attackSpeed = 1.5f;
//        installationCost = 10;

//        SetRange(detectionRange); // 탐지 범위 초기화

//    }
//    private void Start()
//    {
//        lineRenderer = GetComponent<LineRenderer>();

//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }

//        lineRenderer.positionCount = segments + 2; // 부채꼴 꼭짓점을 포함한 점 수 설정
//        lineRenderer.startWidth = 0.3f;
//        lineRenderer.endWidth = 0.3f;
//        lineRenderer.enabled = true;
//        GenerateFan(transform.position);

//    }
//    void Update()
//    {
//        DetectEnemiesInRange();
//    }

//    public override void DetectEnemiesInRange()
//    {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy") && !isAttacking)
//            {
//                StartCoroutine(AttackRoutine(hitCollider.transform));
//            }
//        }
//    }

//    private IEnumerator AttackRoutine(Transform target)
//    {
//        isAttacking = true;
//        RocketAttack(target); // 로켓 공격 실행
//        yield return new WaitForSeconds(attackSpeed);
//        isAttacking = false;
//    }

//    private void RocketAttack(Transform target)
//    {
//        Vector3 targetPosition = target.position;
//        Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

//        Debug.Log("로켓 발사! " + target.name + "이(가) 타겟되었습니다!");

//        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, splashRadius);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                Vector3 toCollider = (hitCollider.transform.position - rocketLaunchPoint.position).normalized;
//                float angle = Vector3.Angle(direction, toCollider);

//                if (angle <= attackConeAngle / 2)
//                {
//                    EnemyBase enemyHp = hitCollider.GetComponent<EnemyBase>();
//                    if (enemyHp != null)
//                    {
//                        enemyHp.TakeDamage(towerAttackPower); // 기본 데미지 적용
//                        StartCoroutine(ApplyDamageOverTime(enemyHp)); // 지속 데미지 적용
//                    }
//                }
//            }
//        }
//    }

//    private IEnumerator ApplyDamageOverTime(EnemyBase enemyHp)
//    {
//        float elapsed = 0f;
//        while (elapsed < damageDuration)
//        {
//            if (enemyHp != null && enemyHp.gameObject.activeSelf)
//            {
//                enemyHp.TakeDamage(damageOverTime); // 지속 데미지 적용
//                Debug.Log("지속 데미지 적용: " + enemyHp.name);
//            }
//            else
//            {
//                break; // enemyHp가 null이거나 비활성화된 경우 루프 종료
//            }
//            elapsed += 1f;
//            yield return new WaitForSeconds(1f);
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        Debug.Log("탐지 범위 설정: " + detectionRange);
//    }

//    private void GenerateFan(Vector3 playerPosition)
//    {
//        Vector3[] vertices = new Vector3[segments + 2];

//        vertices[0] = rocketLaunchPoint.position; // 중심점 설정

//        float angleStep = attackConeAngle / segments; // 부채꼴 각도 사용
//        float currentAngle = -attackConeAngle / 2.0f; // 부채꼴 각도의 반으로 설정

//        for (int i = 0; i <= segments; i++)
//        {
//            float rad = Mathf.Deg2Rad * currentAngle;
//            float x = Mathf.Sin(rad) * detectionRange; // 탐지 범위를 반지름으로 사용
//            float z = Mathf.Cos(rad) * detectionRange;
//            vertices[i + 1] = rocketLaunchPoint.position + rocketLaunchPoint.forward * z + rocketLaunchPoint.right * x; // 포워드 방향에서 그리기
//            currentAngle += angleStep;
//        }
//        vertices[segments + 1] = playerPosition; // 마지막 점을 플레이어의 위치로 설정

//        for (int i = 0;i < vertices.Length; i++)
//        {
//            lineRenderer.SetPosition(i, vertices[i]);
//        }
//    }




//    //void OnDrawGizmos()
//    //{
//    //    Gizmos.color = Color.yellow;
//    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
//    //
//    //    Vector3 forward = rocketLaunchPoint.forward * splashRadius;
//    //    Vector3 right = Quaternion.Euler(0, attackConeAngle / 2, 0) * forward;
//    //    Vector3 left = Quaternion.Euler(0, -attackConeAngle / 2, 0) * forward;
//    //
//    //    Gizmos.color = Color.yellow;
//    //    Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + right);
//    //    Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + left);
//    //}

//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // 탐지 범위
    [SerializeField] private float splashRadius = 10f; // 스플래시 반경
    [SerializeField] private float damageOverTime = 5f; // 도트 데미지
    [SerializeField] private float damageDuration = 5f; // 도트 데미지 지속 시간
    [SerializeField] private Transform rocketLaunchPoint; // 로켓 발사 지점
    private bool isAttacking = false;
    [SerializeField] private float attackConeAngle = 45f; // 공격 콘 각도
    private LineRenderer lineRenderer;
    public int segments = 20;
    private ILineRendererStrategy lineRendererStrategy;

    void Awake()
    {
        towerAttackPower = 20; // 타워 공격력 초기화
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;
        SetRange(detectionRange); // 탐지 범위 초기화
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // 원하는 전략을 초기화하고 설정
        lineRendererStrategy = new FanRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.enabled = true;
    }

    void Update()
    {
        DetectEnemiesInRange();
        // 패턴 생성
        {
            lineRendererStrategy.GeneratePattern(lineRenderer, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
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
        RocketAttack(target); // 로켓 공격 시작
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
    }

    private void RocketAttack(Transform target)
    {
        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

        Debug.Log("로켓 발사! " + target.name + "에게 타격을 가했습니다!");

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
                        enemyHp.TakeDamage(towerAttackPower); // 기본 공격력 피해
                        StartCoroutine(ApplyDamageOverTime(enemyHp)); // 도트 데미지 적용
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
                enemyHp.TakeDamage(damageOverTime); // 도트 데미지 적용
                Debug.Log("도트 데미지 적용: " + enemyHp.name);
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

    // 전략을 변경할 수 있는 메서드
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}
