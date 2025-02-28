
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CanonTower : TowerBase
//{
//    [SerializeField] private float detectionRange = 8f; // Ž�� ����
//    private bool isAttacking = false; // ���� ������ ����
//    [SerializeField] private Transform cannonTowerTransform; // ĳ�� Ÿ�� ��ġ
//    private List<Transform> targets = new List<Transform>(); // Ÿ�� ����Ʈ
//    private LineRenderer lineRenderer; // ���� ������
//    public int segments = 50; // ���׸�Ʈ ��

//    void Awake()
//    {
//        //ž�� �⺻ �ɷ�ġ
//        towerAttackPower = 40;
//        towerPenetrationPower = 25;
//        criticalHitRate = 0.05f;
//        attackSpeed = 0.7f;
//        installationCost = 8;
//        isAttackUp = false;
//    }

//    void Start()
//    {
//        lineRenderer = GetComponent<LineRenderer>();

//        if (lineRenderer == null)
//        {
//            lineRenderer = gameObject.AddComponent<LineRenderer>();
//        }

//        lineRenderer.positionCount = segments + 1; // �߰��� ���׸�Ʈ �� ����
//        lineRenderer.loop = true;
//        lineRenderer.useWorldSpace = false;
//        lineRenderer.startWidth = 1f;
//        lineRenderer.endWidth = 1f;


//        // Ž�� ���� ���� �� ���� ����
//        SetRange(detectionRange);
//    }

//    void Update()
//    {
//        DetectEnemiesInRange();
//        if (targets.Count > 0)
//        {
//            RotateTowardsTarget();
//        }
//    }

//    public override void TowerPowUp()
//    {
//        base.TowerPowUp();
//    }

//    private IEnumerator AttackRoutine(List<Transform> targets)
//    {
//        isAttacking = true;
//        while (targets.Count > 0)
//        {
//            TowerAttack(targets); // Ÿ�� ����Ʈ ù ��° Ÿ�� ����
//            yield return new WaitForSeconds(attackSpeed);

//            // Ÿ�� ����Ʈ ����
//            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

//            // Ÿ�� ����Ʈ�� Ÿ���� ���������� �ٽ� ȸ��
//            if (targets.Count > 0)
//            {
//                RotateTowardsTarget();
//            }
//        }
//        lineRenderer.enabled = false;
//        isAttacking = false;
//    }

//    public override void TowerAttack(List<Transform> targets)
//    {
//        if (targets.Count > 0)
//        {
//            Transform target = targets[0];
//            if (target != null)
//            {
//                EnemyBase enemyHp = target.GetComponent<EnemyBase>();
//                if (enemyHp != null)
//                {
//                    enemyHp.TakeDamage(towerAttackPower);
//                    Debug.Log("ĳ�� Ÿ���� " + target.name + "���� ������ ���߽��ϴ�! ���ݷ�: " + towerAttackPower);
//                }
//            }
//        }
//    }

//    void OnDrawGizmos()
//    {
//        if (targets.Count > 0)
//        {
//            Gizmos.color = Color.yellow;
//            Gizmos.DrawLine(transform.position, targets[0].position);
//        }
//    }


//    public override void SetRange(float range)
//    {
//        detectionRange = range;
//        GenerateCircle(detectionRange); // Ž�� ���� ���� �� ���� ����
//    }

//    public override void DetectEnemiesInRange()
//    {
//        targets.Clear(); // Ÿ�� ����Ʈ �ʱ�ȭ
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
//        foreach (var hitCollider in hitColliders)
//        {
//            if (hitCollider.CompareTag("Enemy"))
//            {
//                targets.Add(hitCollider.transform); // Ž���� ���� Ÿ�� ����Ʈ�� �߰�
//            }
//        }
//    }

//    private void RotateTowardsTarget()
//    {
//        if (targets.Count > 0)
//        {
//            Vector3 direction = (targets[0].position - cannonTowerTransform.position).normalized;
//            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
//            cannonTowerTransform.rotation = Quaternion.Slerp(cannonTowerTransform.rotation, lookRotation, Time.deltaTime * 5f);

//            // ȸ�� �Ϸ� �� ���� ����
//            if (Quaternion.Angle(cannonTowerTransform.rotation, lookRotation) < 5f && !isAttacking)
//            {
//                StartCoroutine(AttackRoutine(targets));
//            }
//        }
//    }

//    private void GenerateCircle(float radius)
//    {
//        lineRenderer.positionCount = segments + 1; // ���׸�Ʈ �� ����

//        float angleStep = 360f / segments;
//        for (int i = 0; i <= segments; i++)
//        {
//            float angle = i * angleStep * Mathf.Deg2Rad;
//            float x = Mathf.Cos(angle) * radius/2;
//            float z = Mathf.Sin(angle) * radius/2;
//            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
//        }
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonTower : TowerBase
{
    [SerializeField] private float detectionRange = 8f; // Ž�� ����
    private bool isAttacking = false; // ���� ������ ����
    [SerializeField] private Transform cannonTowerTransform; // ĳ�� Ÿ�� ��ġ
    private List<Transform> targets = new List<Transform>(); // Ÿ�� ���
    private LineRenderer lineRenderer; // ���� ������
    public int segments = 50; // ���׸�Ʈ ��
    private ILineRendererStrategy lineRendererStrategy; // ���� ������ ����

    void Awake()
    {
        // Ÿ�� �⺻ �ɷ�ġ
        towerAttackPower = 40;
        towerPenetrationPower = 25;
        criticalHitRate = 0.05f;
        attackSpeed = 0.7f;
        installationCost = 8;
        isAttackUp = false;
    }

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // ���� �ʱ�ȭ �� ����
        lineRendererStrategy = new CircleRendererStrategy();
        lineRendererStrategy.Setup(lineRenderer);

        lineRenderer.positionCount = segments + 1; // ���׸�Ʈ �� ����
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;

        // Ž�� ���� ����
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
        base.TowerPowUp();
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets); // Ÿ�� ��� ù ��° Ÿ�� ����
            yield return new WaitForSeconds(attackSpeed);

            // Ÿ�� ��� ����
            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf);

            // Ÿ�� ��Ͽ� Ÿ���� ���� ������ �ٽ� ȸ��
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
                EnemyBase enemyHp = target.GetComponent<EnemyBase>();
                if (enemyHp != null)
                {
                    enemyHp.TakeDamage(towerAttackPower);
                    Debug.Log("ĳ�� Ÿ���� " + target.name + "���� �������� �������ϴ�! ������: " + towerAttackPower);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (targets.Count > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targets[0].position);
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range;
        lineRendererStrategy.GeneratePattern(lineRenderer, transform.position, transform, segments, detectionRange, detectionRange); // Ž�� ���� ����
    }

    public override void DetectEnemiesInRange()
    {
        targets.Clear(); // Ÿ�� ��� �ʱ�ȭ
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform); // Ž���� ���� Ÿ�� ��Ͽ� �߰�
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
}

