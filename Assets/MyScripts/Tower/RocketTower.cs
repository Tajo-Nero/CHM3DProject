//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static UnityEngine.Rendering.HableCurve;

//public class RocketTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 10f; // Ž�� ���� (�⺻ ����)
//    [SerializeField] private float splashRadius = 10f; // ���� ���� (�⺻ ����)
//    [SerializeField] private float damageOverTime = 5f; // ���� ������
//    [SerializeField] private float damageDuration = 5f; // ���� ������ �ð�
//    [SerializeField] private Transform rocketLaunchPoint; // ���� �߻� ����
//    private bool isAttacking = false;
//    [SerializeField] private float attackConeAngle = 45f; // ���� ����

//    private LineRenderer lineRenderer;
//    public int segments = 20;    
//    void Awake()
//    {
//        towerAttackPower = 20; // Ÿ�� ���ݷ� ����
//        towerPenetrationPower = 5;
//        criticalHitRate = 0.05f;
//        attackSpeed = 1.5f;
//        installationCost = 10;

//        SetRange(detectionRange); // Ž�� ���� �ʱ�ȭ

//    }
//    private void Start()
//    {
//        lineRenderer = GetComponent<LineRenderer>();

//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }

//        lineRenderer.positionCount = segments + 2; // ��ä�� �������� ������ �� �� ����
//        lineRenderer.startWidth = 0.3f;
//        lineRenderer.endWidth = 0.3f;
//        lineRenderer.enabled = true;
//        GenerateFan(transform.position);

//    }
//    void Update()
//    {
//        DetectEnemiesInRange();
//    }

//    public override void DetectEnemiesInRange()
//    {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy") && !isAttacking)
//            {
//                StartCoroutine(AttackRoutine(hitCollider.transform));
//            }
//        }
//    }

//    private IEnumerator AttackRoutine(Transform target)
//    {
//        isAttacking = true;
//        RocketAttack(target); // ���� ���� ����
//        yield return new WaitForSeconds(attackSpeed);
//        isAttacking = false;
//    }

//    private void RocketAttack(Transform target)
//    {
//        Vector3 targetPosition = target.position;
//        Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

//        Debug.Log("���� �߻�! " + target.name + "��(��) Ÿ�ٵǾ����ϴ�!");

//        Collider[] hitColliders = Physics.OverlapSphere(targetPosition, splashRadius);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                Vector3 toCollider = (hitCollider.transform.position - rocketLaunchPoint.position).normalized;
//                float angle = Vector3.Angle(direction, toCollider);

//                if (angle <= attackConeAngle / 2)
//                {
//                    EnemyBase enemyHp = hitCollider.GetComponent<EnemyBase>();
//                    if (enemyHp != null)
//                    {
//                        enemyHp.TakeDamage(towerAttackPower); // �⺻ ������ ����
//                        StartCoroutine(ApplyDamageOverTime(enemyHp)); // ���� ������ ����
//                    }
//                }
//            }
//        }
//    }

//    private IEnumerator ApplyDamageOverTime(EnemyBase enemyHp)
//    {
//        float elapsed = 0f;
//        while (elapsed < damageDuration)
//        {
//            if (enemyHp != null && enemyHp.gameObject.activeSelf)
//            {
//                enemyHp.TakeDamage(damageOverTime); // ���� ������ ����
//                Debug.Log("���� ������ ����: " + enemyHp.name);
//            }
//            else
//            {
//                break; // enemyHp�� null�̰ų� ��Ȱ��ȭ�� ��� ���� ����
//            }
//            elapsed += 1f;
//            yield return new WaitForSeconds(1f);
//        }
//    }

//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        Debug.Log("Ž�� ���� ����: " + detectionRange);
//    }

//    private void GenerateFan(Vector3 playerPosition)
//    {
//        Vector3[] vertices = new Vector3[segments + 2];

//        vertices[0] = rocketLaunchPoint.position; // �߽��� ����

//        float angleStep = attackConeAngle / segments; // ��ä�� ���� ���
//        float currentAngle = -attackConeAngle / 2.0f; // ��ä�� ������ ������ ����

//        for (int i = 0; i <= segments; i++)
//        {
//            float rad = Mathf.Deg2Rad * currentAngle;
//            float x = Mathf.Sin(rad) * detectionRange; // Ž�� ������ ���������� ���
//            float z = Mathf.Cos(rad) * detectionRange;
//            vertices[i + 1] = rocketLaunchPoint.position + rocketLaunchPoint.forward * z + rocketLaunchPoint.right * x; // ������ ���⿡�� �׸���
//            currentAngle += angleStep;
//        }
//        vertices[segments + 1] = playerPosition; // ������ ���� �÷��̾��� ��ġ�� ����

//        for (int i = 0;i < vertices.Length; i++)
//        {
//            lineRenderer.SetPosition(i, vertices[i]);
//        }
//    }




//    //void OnDrawGizmos()
//    //{
//    //    Gizmos.color = Color.yellow;
//    //    Gizmos.DrawWireSphere(transform.position, detectionRange);
//    //
//    //    Vector3 forward = rocketLaunchPoint.forward * splashRadius;
//    //    Vector3 right = Quaternion.Euler(0, attackConeAngle / 2, 0) * forward;
//    //    Vector3 left = Quaternion.Euler(0, -attackConeAngle / 2, 0) * forward;
//    //
//    //    Gizmos.color = Color.yellow;
//    //    Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + right);
//    //    Gizmos.DrawLine(rocketLaunchPoint.position, rocketLaunchPoint.position + left);
//    //}

//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTower : TowerBase
{
    [SerializeField] private float detectionRange = 10f; // Ž�� ����
    [SerializeField] private float splashRadius = 10f; // ���÷��� �ݰ�
    [SerializeField] private float damageOverTime = 5f; // ��Ʈ ������
    [SerializeField] private float damageDuration = 5f; // ��Ʈ ������ ���� �ð�
    [SerializeField] private Transform rocketLaunchPoint; // ���� �߻� ����
    private bool isAttacking = false;
    [SerializeField] private float attackConeAngle = 45f; // ���� �� ����
    private LineRenderer lineRenderer;
    public int segments = 20;
    private ILineRendererStrategy lineRendererStrategy;

    void Awake()
    {
        towerAttackPower = 20; // Ÿ�� ���ݷ� �ʱ�ȭ
        towerPenetrationPower = 5;
        criticalHitRate = 0.05f;
        attackSpeed = 1.5f;
        installationCost = 10;
        SetRange(detectionRange); // Ž�� ���� �ʱ�ȭ
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // ���ϴ� ������ �ʱ�ȭ�ϰ� ����
        lineRendererStrategy = new FanRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 2;
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.3f;
        lineRenderer.enabled = true;
    }

    void Update()
    {
        DetectEnemiesInRange();
        // ���� ����
        {
            lineRendererStrategy.GeneratePattern(lineRenderer, rocketLaunchPoint.position, transform, segments, attackConeAngle, detectionRange);
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
        RocketAttack(target); // ���� ���� ����
        yield return new WaitForSeconds(attackSpeed);
        isAttacking = false;
    }

    private void RocketAttack(Transform target)
    {
        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - rocketLaunchPoint.position).normalized;

        Debug.Log("���� �߻�! " + target.name + "���� Ÿ���� ���߽��ϴ�!");

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
                        enemyHp.TakeDamage(towerAttackPower); // �⺻ ���ݷ� ����
                        StartCoroutine(ApplyDamageOverTime(enemyHp)); // ��Ʈ ������ ����
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
                enemyHp.TakeDamage(damageOverTime); // ��Ʈ ������ ����
                Debug.Log("��Ʈ ������ ����: " + enemyHp.name);
            }
            else
            {
                break; // enemyHp�� null�̰ų� ��Ȱ��ȭ�� ��� ���� ����
            }
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        Debug.Log("Ž�� ���� ����: " + detectionRange);
    }

    // ������ ������ �� �ִ� �޼���
    public void SetStrategy(ILineRendererStrategy newStrategy)
    {
        lineRendererStrategy = newStrategy;
        lineRendererStrategy.Setup(lineRenderer);
    }
}
