using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private Transform laserStartPoint; // 레이저 시작점
    [SerializeField] private ParticleSystem laserImpactEffectPrefab; // 레이저 충돌 파티클 효과 프리팹
    private bool isAttacking = false; // 공격 중인지 여부

    void Awake()
    {
        towerAttackPower = 50; // 공격력 설정
        attackSpeed = 1f; // 공격 속도 설정
        installationCost = 15; // 설치 비용 설정
    }

    protected override void Start()
    {
        rangeColor = Color.blue; // 레이저 타워 - 파랑
        detectionRange = 20f;

        base.Start(); // TowerBase의 SetupRangeDecal 호출

        SetRange(detectionRange);
        rangeType = RangeType.Rectangle;
        // 기존 LineRenderer 코드 제거
    }

    void Update()
    {
        DetectEnemiesInRange(); // 적 탐지
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        // 자기 자신의 위치에서 탐지 박스 설정
        Vector3 boxCenter = transform.position + transform.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(2f, 2f, detectionRange / 2);
        Quaternion boxOrientation = Quaternion.Euler(0, transform.eulerAngles.y, 0);

        // 박스 캐스트로 적 탐지
        RaycastHit[] hits = Physics.BoxCastAll(boxCenter, boxHalfExtents, transform.forward, boxOrientation, detectionRange);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                targets.Add(hit.transform);
                Debug.Log("Detected Enemy: " + hit.transform.name);
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

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf); // 비활성화된 타겟 제거
        }
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            EnemyPathFollower enemyHp = target.GetComponent<EnemyPathFollower>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower); // 적에게 데미지 입힘
                Debug.Log("Laser hit " + target.name + " for " + towerAttackPower + " damage!");

                // 레이저 충돌 파티클 효과 재생
                if (laserImpactEffectPrefab != null)
                {
                    ParticleSystem laserImpactEffect = Instantiate(laserImpactEffectPrefab, target.position, Quaternion.LookRotation(target.position - laserStartPoint.position));
                    StartCoroutine(PlayParticleEffect(laserImpactEffect));
                }
            }
        }
    }

    private IEnumerator PlayParticleEffect(ParticleSystem laserImpactEffect)
    {
        laserImpactEffect.Play();
        yield return new WaitForSeconds(0.5f); // 파티클 효과 재생 시간
        laserImpactEffect.Stop();
        Destroy(laserImpactEffect.gameObject); // 파티클 오브젝트 파괴
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // 탐지 거리 설정
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2; // 공격력 두 배로 증가
        Debug.Log("파워업! 타워의 공격력이 두 배로 증가했습니다: " + towerAttackPower);
    }

}
