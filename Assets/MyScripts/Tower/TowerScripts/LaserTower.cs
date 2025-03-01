using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : TowerBase
{
    [SerializeField] private float detectionRange = 20f; // Ž�� ����
    [SerializeField] private Transform laserStartPoint; // ������ ������
    private ILineRendererStrategy lineRendererStrategy;
    private bool isAttacking = false; // ���� ������ ����

    // �ʱⰪ�� �����մϴ�.
    void Awake()
    {
        towerAttackPower = 50; // ���ݷ� ����
        attackSpeed = 1f; // ���� �ӵ� ����
        installationCost = 15; // ��ġ ��� ����
    }

    void Start()
    {
        SetRange(detectionRange); // Ž�� ���� ����

        // ���η����� ���� �ʱ�ȭ
        lineRendererStrategy = new LaserRendererStrategy();
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, laserStartPoint, 4, detectionRange, 20f);
    }

    void Update()
    {
        DetectEnemiesInRange(); // �� Ž��
    }

    public override void DetectEnemiesInRange()
    {
        List<Transform> targets = new List<Transform>();

        // Ž�� ���� ����
        Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
        Vector3 boxHalfExtents = new Vector3(2f, 2f, detectionRange / 2);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, laserStartPoint.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                targets.Add(hitCollider.transform);
                Debug.Log("Detected Enemy: " + hitCollider.transform.name);
            }
        }

        if (targets.Count > 0 && !isAttacking)
        {
            StartCoroutine(AttackRoutine(targets)); // ���� ��ƾ ����
        }
    }

    private IEnumerator AttackRoutine(List<Transform> targets)
    {
        isAttacking = true;
        while (targets.Count > 0)
        {
            TowerAttack(targets); // Ÿ�� ����
            yield return new WaitForSeconds(attackSpeed); // ���� ��� �ð�

            targets.RemoveAll(t => t == null || !t.gameObject.activeSelf); // ��Ȱ��ȭ�� �� ����
        }
        isAttacking = false;
    }

    public override void TowerAttack(List<Transform> targets)
    {
        foreach (var target in targets)
        {
            EnemyBase enemyHp = target.GetComponent<EnemyBase>();
            if (enemyHp != null)
            {
                enemyHp.TakeDamage(towerAttackPower); // ������ ������ �ֱ�
                Debug.Log("Laser hit " + target.name + " for " + towerAttackPower + " damage!");
            }
        }
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // Ž�� ���� ����
    }

    public override void TowerPowUp()
    {
        towerAttackPower *= 2; // ���ݷ� �� ��� ����
        Debug.Log("������ Ÿ���� ���ݷ��� ��ȭ�Ǿ����ϴ�: " + towerAttackPower);
    }

    private void OnDrawGizmos()
    {
        if (laserStartPoint != null)
        {
            // Ž�� ���� �ð�ȭ
            Gizmos.color = Color.red;
            Vector3 boxCenter = laserStartPoint.position + laserStartPoint.forward * (detectionRange / 2);
            Vector3 boxSize = new Vector3(2f, 2f, detectionRange);
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, laserStartPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }
}
