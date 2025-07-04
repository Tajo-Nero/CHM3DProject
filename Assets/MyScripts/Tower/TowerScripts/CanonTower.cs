using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 8f; // ���� ����
    private bool isAttacking = false;
    [SerializeField] private Transform cannonTowerTransform;
    private List<Transform> targets = new List<Transform>();
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 50;

    void Awake()
    {
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;
        isAttackUp = false;

        // ���� ������ ���� ����
        lineRendererStrategy = new CircleRendererStrategy();
    }

    void Start()
    {
        // ���� ������ ���� �� ���� ����
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, cannonTowerTransform, segments, detectionRange, 0);

        SetRange(detectionRange);       

    }

    void Update()
    {
        DetectEnemiesInRange();
        if (targets.Count > 0)
        {
            RotateTowardsTarget();
        }
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2;
        Debug.Log("ĳ�� Ÿ���� ���ݷ��� ��ȭ�Ǿ����ϴ�: " + towerAttackPower);
    }

    public override void TowerAttack(List<Transform> targets)
    {
        if (targets.Count > 0)
        {
            Transform target = targets[0];
            if (target != null)
            {
                EnemyPathFollower enemyHp = target.GetComponent<EnemyPathFollower>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("ĳ�� Ÿ���� " + target.name + "���� �������� �������ϴ�! ���ݷ�: " + towerAttackPower);
                }
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
    }

    public override void DetectEnemiesInRange()
    {
        targets.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
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

            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
            {
                StartCoroutine(AttackRoutine(targets));
            }
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets);
            yield return new WaitForSeconds(attackSpeed);

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            if (targets.Count > 0)
            {
                RotateTowardsTarget();
            }
        }
        isAttacking = false;
    }
}
