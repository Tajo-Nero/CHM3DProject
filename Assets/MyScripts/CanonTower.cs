using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // Ž�� ����
    private bool isAttacking = false; // ���� �� ����
    [SerializeField] private Transform cannonTowerTransform; // ĳ�� Ÿ�� ��ġ
    private List<Transform> targets = new List<Transform>(); // ������ ������ ����Ʈ
    private LineRenderer lineRenderer; // ������ ���� ������

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
            TowerAttack(targets); // ����Ʈ�� ù ��° Ÿ���� ����
            yield return new WaitForSeconds(attackSpeed);

            // Ÿ�� ����Ʈ ����
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            // Ÿ�� ����Ʈ�� �����ִٸ� �ٽ� ȸ��
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
                    Debug.Log("ĳ�� Ÿ���� " + target.name + "�� Ÿ�ݵǾ����ϴ�! ���ط�: " + towerAttackPower);
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
        targets.Clear(); // ���� ����Ʈ �ʱ�ȭ
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform); // ������ ���� ����Ʈ�� �߰�
                Debug.Log("�� ������: " + hitCollider.name);
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

            // ȸ�� �Ϸ� �� ���� ����
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
