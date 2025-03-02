//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CanonTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 8f; // ���� ����
//    private bool isAttacking = false;
//    [SerializeField] private Transform cannonTowerTransform;
//    private List<Transform> targets = new List<Transform>();
//    private Transform currentTarget;
//    private ILineRendererStrategy lineRendererStrategy;
//    public int segments = 50;

//    void Awake()
//    {
//        towerAttackPower = 40;
//        towerPenetrationPower = 25;
//        criticalHitRate = 0.05f;
//        attackSpeed = 0.7f;
//        installationCost = 8;
//        isAttackUp = false;

//        // ���� ������ ���� �ʱ�ȭ
//        lineRendererStrategy = new CircleRendererStrategy();
//    }

//    void Start()
//    {
//        // ���� ������ ������ ����Ͽ� ���� ���� ǥ��
//        lineRendererStrategy.Setup(gameObject);
//        lineRendererStrategy.GeneratePattern(gameObject, transform.position, cannonTowerTransform, segments, detectionRange, 0);

//        SetRange(detectionRange);
//    }

//    void Update()
//    {
//        DetectEnemiesInRange();
//        if (targets.Count > 0)
//        {
//            RotateTowardsTarget();
//        }
//        else
//        {
//            currentTarget = null; // ���� ���� ���� ���� ������ Ÿ�� �ʱ�ȭ
//        }
//    }

//    public override void TowerPowUp()
//    {
//        towerAttackPower *= 2;
//        Debug.Log("ĳ�� Ÿ���� ���ݷ��� ����Ͽ����ϴ�: " + towerAttackPower);
//    }

//    public override void TowerAttack(List<Transform> targets)
//    {
//        if (targets.Count > 0)
//        {
//            // ���� Ÿ���� ���� ��� ù ��° Ÿ���� ����
//            if (currentTarget == null)
//            {
//                currentTarget = targets[0];
//            }

//            if (currentTarget != null)
//            {
//                EnemyBase enemyHp = currentTarget.GetComponent<EnemyBase>();
//                if (enemyHp != null)
//                {
//                    enemyHp.TakeDamage(towerAttackPower);
//                    Debug.Log("ĳ�� Ÿ���� " + currentTarget.name + "���� ���ظ� �������ϴ�! ���ݷ�: " + towerAttackPower);
//                }
//            }
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//    }

//    public override void DetectEnemiesInRange()
//    {
//        targets.Clear();
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform);
//            }
//        }

//        // �������� ��� ���� Ÿ������ ����
//        if (targets.Count > 0)
//        {
//            currentTarget = targets[targets.Count - 1];
//        }
//    }

//    private void RotateTowardsTarget()
//    {
//        if (currentTarget != null)
//        {
//            Vector3 direction = (currentTarget.position - cannonTowerTransform.position).normalized;
//            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
//            cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);

//            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
//            {
//                StartCoroutine(AttackRoutine(targets));
//            }
//        }
//    }

//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true;
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets);
//            yield return new WaitForSeconds(attackSpeed);

//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

//            if (targets.Count > 0)
//            {
//                RotateTowardsTarget();
//            }
//        }
//        isAttacking = false;
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 8f;
    private bool isAttacking = false;
    [SerializeField] private Transform cannonTowerTransform;
    [SerializeField] private ParticleSystem attackParticleSystem; // ��ƼŬ �ý��� ����
    private List<Transform> targets = new List<Transform>();
    private Transform currentTarget;
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

        lineRendererStrategy = new CircleRendererStrategy();
    }

    void Start()
    {
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
        else
        {
            currentTarget = null;
        }
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2;
        Debug.Log("ĳ�� Ÿ���� ���ݷ��� ����Ͽ����ϴ�: " + towerAttackPower);
    }

    public override void TowerAttack(List<Transform> targets)
    {
        if (targets.Count > 0)
        {
            if (currentTarget == null)
            {
                currentTarget = targets[0];
            }

            if (currentTarget != null)
            {
                EnemyBase enemyHp = currentTarget.GetComponent<EnemyBase>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("ĳ�� Ÿ���� " + currentTarget.name + "���� ���ظ� �������ϴ�! ���ݷ�: " + towerAttackPower);

                    // ��ƼŬ ȿ�� ���
                    if (attackParticleSystem != null)
                    {
                        attackParticleSystem.Play();
                    }
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

        if (targets.Count > 0)
        {
            currentTarget = targets[targets.Count - 1];
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget != null)
        {
            Vector3 direction = (currentTarget.position - cannonTowerTransform.position).normalized;
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
