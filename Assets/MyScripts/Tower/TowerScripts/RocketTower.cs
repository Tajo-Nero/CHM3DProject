using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float splashRadius = 10f; // ���÷��� �ݰ�
    [SerializeField] private float damageOverTime = 5f; // ���� ������
    [SerializeField] private float damageDuration = 5f; // ���� ������ �ð�
    [SerializeField] private Transform rocketLaunchPoint; // ���� �߻� ��ġ
    private bool isAttacking = false; // ���� ������ ����
    [SerializeField] private float attackConeAngle = 45f; // ���� ���� ����

    void Awake()
    {
        // Ÿ�� �⺻ �Ӽ� �ʱ�ȭ
        towerAttackPower = 20;
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;
        SetRange(detectionRange); // ���� ���� �ʱ�ȭ

    }

    protected override void Start()
    {
        rangeColor = Color.blue; // ���� Ÿ�� - ��Ȳ
        detectionRange = 10f;
        rangeType = RangeType.Fan;

        base.Start(); // TowerBase�� SetupRangeDecal ȣ��

        SetRange(detectionRange);

        // ���� LineRenderer �ڵ� ����
    }

    void Update()
    {
        DetectEnemiesInRange();

        // ������ ǥ�� ���̰� Ÿ���� ȸ���ߴٸ� ������ ȸ��
        if (isRangeVisible && rangeDecal != null)
        {
            rangeDecal.transform.localRotation = Quaternion.Euler(90, transform.eulerAngles.y, 0);
        }
    }

    public override void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && !isAttacking)
            {
                StartCoroutine(AttackRoutine(hitCollider.transform));
            }
        }
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        isAttacking = true;
        TowerAttack(new List<Transform> { target }); // ��ӹ��� TowerAttack �޼��� ���
        yield return new WaitForSeconds(attackSpeed); // ���� �ֱ� ���
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            Vector3 targetPosition = target.position;
            Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

            Debug.Log("���� �߻�! " + target.name + "���� Ÿ���� �������ϴ�!");

            Collider[] hitColliders = Physics.OverlapSphere(targetPosition, splashRadius);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    Vector3 toCollider = (hitCollider.transform.position - rocketLaunchPoint.position).normalized;
                    float angle = Vector3.Angle(direction, toCollider);

                    if (angle <= attackConeAngle / 2)
                    {
                        EnemyPathFollower enemyHp = hitCollider.GetComponent<EnemyPathFollower>();
                        if (enemyHp != null)
                        {
                            enemyHp.TakeDamage(towerAttackPower); // �⺻ ������ ����
                            StartCoroutine(ApplyDamageOverTime(enemyHp)); // ���� ������ ����
                        }
                    }
                }
            }
        }
    }

    private IEnumerator ApplyDamageOverTime(EnemyPathFollower enemyHp)
    {
        float elapsed = 0f;
        while (elapsed < damageDuration)
        {
            if (enemyHp != null && enemyHp.gameObject.activeSelf)
            {
                enemyHp.TakeDamage(damageOverTime); // ���� ������ ����
                Debug.Log("���� ������ ����: " + enemyHp.name);
            }
            else
            {
                break; // ���� ������ų� ��Ȱ��ȭ�� ��� ����
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("���� ���� ����: " + detectionRange);
    }

    public override void TowerPowUp()
    {
        // �Ŀ��� ��� �߰�
        towerAttackPower *= 2;
        Debug.Log("���� Ÿ���� ���ݷ��� ��ȭ�Ǿ����ϴ�: " + towerAttackPower);
    }
}
