using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;
    private Transform target;  // target 변수 선언
    [SerializeField] private Transform laserStartPoint;  // 레이저가 시작되는 위치
    private LineRenderer lineRenderer;  // 레이저를 그리기 위한 LineRenderer
    private bool isAttacking = false;
    private bool showGizmos = false;
    [SerializeField] private float laserLength = 10f;  // 레이저 길이

    void Start()
    {
        towerAttackPower = 100;
        attackSpeed = 1f;
        installationCost = 15;

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
        if (target != null && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        showGizmos = true;
        while (target != null)
        {
            TowerAttack();
            yield return new WaitForSeconds(attackSpeed);
        }
        lineRenderer.enabled = false;
        showGizmos = false;
        isAttacking = false;
    }

    public override void TowerAttack()
    {
        lineRenderer.enabled = true;
        RaycastHit[] hits;
        lineRenderer.SetPosition(0, laserStartPoint.position);
        lineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserLength);

        // 레이캐스트 시작 위치를 타워 위치에서 발사
        Vector3 laserStartPosition = laserStartPoint.position;

        hits = Physics.RaycastAll(laserStartPosition, laserStartPoint.forward, laserLength);
        foreach (var hit in hits)
        {
            Debug.Log("레이캐스트 충돌: " + hit.collider.name);  // 충돌한 오브젝트 디버그 출력
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHp enemyHp = hit.collider.GetComponent<EnemyHp>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("레이저 타워가 " + hit.collider.name + "에 데미지를 입혔습니다! 공격력: " + towerAttackPower);
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                target = hitCollider.transform;
                
                break;
            }
        }

        if (hitColliders.Length == 0)
        {
            target = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(laserStartPoint.position, laserStartPoint.position + laserStartPoint.forward * laserLength);
        }
    }
}
