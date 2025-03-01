using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 20f; // 탐지 범위
    [SerializeField] private Transform laserStartPoint; // 레이저 시작점
    private ILineRendererStrategy lineRendererStrategy;
    private bool isAttacking = false; // 공격 중인지 여부

    // 초기값을 설정합니다.
    void Awake()
    {
        towerAttackPower = 50; // 공격력 설정
        attackSpeed = 1f; // 공격 속도 설정
        installationCost = 15; // 설치 비용 설정
    }

    void Start()
    {
        SetRange(detectionRange); // 탐지 범위 설정

        // 라인렌더러 전략 초기화
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, laserStartPoint, 4, detectionRange, 20f);
    }

    void Update()
    {
        DetectEnemiesInRange(); // 적 탐지
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        // 탐지 범위 설정
        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(2f, 2f, detectionRange / 2);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
                Debug.Log("Detected Enemy: " + hitCollider.transform.name);
            }
        }

        if (targets.Count > 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine(targets)); // 공격 루틴 시작
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets); // 타워 공격
            yield return new WaitForSeconds(attackSpeed); // 공격 대기 시간

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf); // 비활성화된 적 제거
        }
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower); // 적에게 데미지 주기
                Debug.Log("Laser hit " + target.name + " for " + towerAttackPower + " damage!");
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // 탐지 범위 설정
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2; // 공격력 두 배로 증가
        Debug.Log("레이저 타워의 공격력이 강화되었습니다: " + towerAttackPower);
    }

    private void OnDrawGizmos()
    {
        if (laserStartPoint != null)
        {
            // 탐지 범위 시각화
            Gizmos.color = Color.red;
            Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
            Vector3 boxSize = new Vector3(2f, 2f, detectionRange);
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, laserStartPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}
