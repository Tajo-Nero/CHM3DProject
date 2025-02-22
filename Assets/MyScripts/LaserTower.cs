using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f;
    private Transform target;  // target ���� ����
    [SerializeField] private Transform laserStartPoint;  // �������� ���۵Ǵ� ��ġ
    private LineRenderer lineRenderer;  // �������� �׸��� ���� LineRenderer
    private bool isAttacking = false;
    private bool showGizmos = false;
    [SerializeField] private float laserLength = 10f;  // ������ ����

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

        // ����ĳ��Ʈ ���� ��ġ�� Ÿ�� ��ġ���� �߻�
        Vector3 laserStartPosition = laserStartPoint.position;

        hits = Physics.RaycastAll(laserStartPosition, laserStartPoint.forward, laserLength);
        foreach (var hit in hits)
        {
            Debug.Log("����ĳ��Ʈ �浹: " + hit.collider.name);  // �浹�� ������Ʈ ����� ���
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyHp enemyHp = hit.collider.GetComponent<EnemyHp>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("������ Ÿ���� " + hit.collider.name + "�� �������� �������ϴ�! ���ݷ�: " + towerAttackPower);
                }
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("Ž�� ���� ������: " + detectionRange);
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
