using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // 탐지 범위
    [SerializeField] private Transform laserStartPoint; // 레이저가 시작되는 위치
    private LineRenderer lineRenderer; // 레이저를 그릴 LineRenderer
    private bool isAttacking = false; // 공격 중 여부
    private bool showGizmos = false; // Gizmos 표시 여부
    [SerializeField] private float laserLength = 10f; // 레이저 길이

    void Start()
    {
        towerAttackPower = 100; // 타워의 공격력 설정
        attackSpeed = 1f; // 공격 속도 설정
        installationCost = 15; // 설치 비용 설정

        SetRange(detectionRange); // 탐지 범위 설정
        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer 컴포넌트 참조

        // LineRenderer가 없으면 추가
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }

    void Update()
    {
        DetectEnemiesInRange(); // 범위 내 적 탐지
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>(); // 타겟들을 저장할 리스트

        // 탐지 범위 내의 충돌체 탐지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform); // 적을 리스트에 추가
            }
        }

        if (targets.Count > 0)
        {
            // 타겟들이 있는 경우 공격 루틴 시작
            StartCoroutine(AttackRoutine(targets));
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true; // 공격 중 상태로 설정
        showGizmos = true; // Gizmos 표시
        while (targets.Count > 0)
        {
            TowerAttack(targets); // 모든 타겟을 공격
            yield return new WaitForSeconds(attackSpeed); // 공격 속도만큼 대기

            // 타겟 리스트 갱신 (제거된 타겟 제거)
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
        }
        lineRenderer.enabled = false; // LineRenderer 비활성화
        showGizmos = false; // Gizmos 숨김
        isAttacking = false; // 공격 중 상태 해제
    }

    public override void TowerAttack(List<Transform> targets)
    {
        lineRenderer.enabled = true; // LineRenderer 활성화
        RaycastHit[] hits;
        lineRenderer.SetPosition(0, laserStartPoint.position); // 레이저 시작 위치 설정
        lineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserLength); // 레이저 끝 위치 설정

        // 레이캐스트 시작 위치
        Vector3 laserStartPosition = laserStartPoint.position;

        // 레이캐스트 실행
        hits = Physics.RaycastAll(laserStartPosition, laserStartPoint.forward, laserLength);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyAI enemyHp = hit.collider.GetComponent<EnemyAI>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower); // 적에게 데미지 적용
                    Debug.Log("레이저 타워가 " + hit.collider.name + "에 타격되었습니다! 피해량: " + towerAttackPower);
                }
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // 탐지 범위 설정
        Debug.Log("탐지 범위 설정됨: " + detectionRange);
    }

    void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(laserStartPoint.position, laserStartPoint.position + laserStartPoint.forward * laserLength); // 레이저 방향 Gizmo 그리기
        }
    }
}
