using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // ���� ����
    [SerializeField] private float splashRadius = 10f; // ���÷��� �ݰ�
    [SerializeField] private float damageOverTime = 5f; // ���� ������
    [SerializeField] private float damageDuration = 5f; // ���� ������ �ð�
    [SerializeField] private Transform rocketLaunchPoint; // ���� �߻� ��ġ
    private bool isAttacking = false; // ���� ������ ����
    [SerializeField] private float attackConeAngle = 45f; // ���� ���� ����
    private ILineRendererStrategy lineRendererStrategy;

    void Awake()
    {
        // Ÿ�� �⺻ �Ӽ� �ʱ�ȭ
        towerAttackPower = 20;
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;
        SetRange(detectionRange); // ���� ���� �ʱ�ȭ

        // ���� ������ ���� ����
        lineRendererStrategy = new FanRendererStrategy();
    }

    private void Start()
    {
        // ���� ������ ���� �� ���� ����
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, rocketLaunchPoint, 20, attackConeAngle, detectionRange);
    }

    void Update()
    {
        DetectEnemiesInRange(); // �� ����
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
                        EnemyBase enemyHp = hitCollider.GetComponent<EnemyBase>();
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

    private IEnumerator ApplyDamageOverTime(EnemyBase enemyHp)
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

    //private void OnDrawGizmos()
    //{
    //    // ���� ���� �� ���� ������ �ð������� ǥ��
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
    //
    //    Vector3 forward = rocketLaunchPoint.forward * splashRadius;
    //    Vector3 right = Quaternion.Euler(0, attackConeAngle / 2, 0) * forward;
    //    Vector3 left = Quaternion.Euler(0, -attackConeAngle / 2, 0) * forward;
    //
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + right);
    //    Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + left);
    //}
}
