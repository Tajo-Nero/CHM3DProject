using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // 탐지 범위
    private bool isAttacking = false; // 공격 중 여부
    [SerializeField] private Transform cannonTowerTransform; // 캐논 타워 위치
    private List<Transform> targets = new List<Transform>(); // 감지된 적들의 리스트
    private LineRenderer lineRenderer; // 레이저 라인 렌더러

    void Start()
    {
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;

        SetRange(detectionRange);

        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }

    void Update()
    {
        DetectEnemiesInRange();
        if (targets.Count > 0 && !isAttacking)
        {
            RotateTowardsTarget();
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets); // 리스트의 첫 번째 타겟을 공격
            yield return new WaitForSeconds(attackSpeed);

            // 타겟 리스트 갱신
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            // 타겟 리스트가 남아있다면 다시 회전
            if (targets.Count > 0)
            {
                RotateTowardsTarget();
            }
        }
        lineRenderer.enabled = false;
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        if (targets.Count > 0)
        {
            Transform target = targets[0];
            if (target != null)
            {
                EnemyAI enemyHp = target.GetComponent<EnemyAI>();
                if (enemyHp != null)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, cannonTowerTransform.position);
                    lineRenderer.SetPosition(1, target.position);

                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("캐논 타워가 " + target.name + "에 타격되었습니다! 피해량: " + towerAttackPower);
                }
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
        targets.Clear(); // 기존 리스트 초기화
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform); // 감지된 적을 리스트에 추가
                Debug.Log("적 감지됨: " + hitCollider.name);
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

            // 회전 완료 후 공격 시작
            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
            {
                StartCoroutine(AttackRoutine(targets));
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
