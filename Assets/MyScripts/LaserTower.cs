using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // Ž�� ����
    [SerializeField] private Transform laserStartPoint; // �������� ���۵Ǵ� ��ġ
    private LineRenderer lineRenderer; // �������� �׸� LineRenderer
    private bool isAttacking = false; // ���� �� ����
    private bool showGizmos = false; // Gizmos ǥ�� ����
    [SerializeField] private float laserLength = 10f; // ������ ����

    void Start()
    {
        towerAttackPower = 100; // Ÿ���� ���ݷ� ����
        attackSpeed = 1f; // ���� �ӵ� ����
        installationCost = 15; // ��ġ ��� ����

        SetRange(detectionRange); // Ž�� ���� ����
        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer ������Ʈ ����

        // LineRenderer�� ������ �߰�
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
    }

    void Update()
    {
        DetectEnemiesInRange(); // ���� �� �� Ž��
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>(); // Ÿ�ٵ��� ������ ����Ʈ

        // Ž�� ���� ���� �浹ü Ž��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform); // ���� ����Ʈ�� �߰�
            }
        }

        if (targets.Count > 0)
        {
            // Ÿ�ٵ��� �ִ� ��� ���� ��ƾ ����
            StartCoroutine(AttackRoutine(targets));
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true; // ���� �� ���·� ����
        showGizmos = true; // Gizmos ǥ��
        while (targets.Count > 0)
        {
            TowerAttack(targets); // ��� Ÿ���� ����
            yield return new WaitForSeconds(attackSpeed); // ���� �ӵ���ŭ ���

            // Ÿ�� ����Ʈ ���� (���ŵ� Ÿ�� ����)
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);
        }
        lineRenderer.enabled = false; // LineRenderer ��Ȱ��ȭ
        showGizmos = false; // Gizmos ����
        isAttacking = false; // ���� �� ���� ����
    }

    public override void TowerAttack(List<Transform> targets)
    {
        lineRenderer.enabled = true; // LineRenderer Ȱ��ȭ
        RaycastHit[] hits;
        lineRenderer.SetPosition(0, laserStartPoint.position); // ������ ���� ��ġ ����
        lineRenderer.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserLength); // ������ �� ��ġ ����

        // ����ĳ��Ʈ ���� ��ġ
        Vector3 laserStartPosition = laserStartPoint.position;

        // ����ĳ��Ʈ ����
        hits = Physics.RaycastAll(laserStartPosition, laserStartPoint.forward, laserLength);
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyAI enemyHp = hit.collider.GetComponent<EnemyAI>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower); // ������ ������ ����
                    Debug.Log("������ Ÿ���� " + hit.collider.name + "�� Ÿ�ݵǾ����ϴ�! ���ط�: " + towerAttackPower);
                }
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // Ž�� ���� ����
        Debug.Log("Ž�� ���� ������: " + detectionRange);
    }

    void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(laserStartPoint.position, laserStartPoint.position + laserStartPoint.forward * laserLength); // ������ ���� Gizmo �׸���
        }
    }
}
