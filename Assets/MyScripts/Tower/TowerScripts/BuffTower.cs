using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : TowerBase
{
    [SerializeField] private float buffRange = 10f; // ���� ����
    [SerializeField] private float attackPowerMultiplier = 1.2f; // ���ݷ� ��� (20% ����)
    [SerializeField] private float buffDuration = 5f; // ���� ���� �ð�
    [SerializeField] private float buffInterval = 6f; // ���� �ֱ�
    private List<TowerBase> towersInRange = new List<TowerBase>(); // ���� �� Ÿ�� ���
    private ILineRendererStrategy lineRendererStrategy;
    public int segments = 50; // ���� ������ ���׸�Ʈ ��

    void Start()
    {
        // ���� ������ ���� ����
        lineRendererStrategy = new CircleRendererStrategy();

        // ���� ������ ���� �� ���� ����
        lineRendererStrategy.Setup(gameObject);
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, buffRange, 0);

        StartCoroutine(BuffTowersInRange()); // ���� ��ƾ ����

        // ��ġ ��� ����
        installationCost = 15;
    }

    void Update()
    {
        // ���� �������� ����Ͽ� ���� ���� �ð�ȭ
        GenerateRangeVisualization();
    }

    private void GenerateRangeVisualization()
    {
        lineRendererStrategy.GeneratePattern(gameObject, transform.position, transform, segments, buffRange, 0);
    }

    private IEnumerator BuffTowersInRange()
    {
        while (true)
        {
            DetectTowersInRange(); // ���� �� Ÿ�� ����
            ApplyBuffToTowers(); // Ÿ���� ���� ����
            yield return new WaitForSeconds(buffInterval); // ���� �ֱ� ���
        }
    }

    private void DetectTowersInRange()
    {
        towersInRange.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange); // ���� �� �ݶ��̴� Ž��
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Towers"))
            {
                TowerBase tower = hitCollider.GetComponent<TowerBase>();
                if (tower != null && !towersInRange.Contains(tower))
                {
                    towersInRange.Add(tower); // ���� �� Ÿ�� ��Ͽ� �߰�
                }
            }
        }
    }

    private void ApplyBuffToTowers()
    {
        StartCoroutine(ApplyBuffForDuration()); // ���� ���� �ð� ���� ����
    }

    private IEnumerator ApplyBuffForDuration()
    {
        List<TowerBase> buffedTowers = new List<TowerBase>(towersInRange);

        // Ÿ������ ���ݷ� ����
        foreach (var tower in buffedTowers)
        {
            tower.towerAttackPower *= attackPowerMultiplier;
        }

        yield return new WaitForSeconds(buffDuration); // ���� ���� �ð� ���

        // Ÿ������ ���ݷ� ������� ����
        foreach (var tower in buffedTowers)
        {
            if (tower != null)
            {
                tower.towerAttackPower /= attackPowerMultiplier;
            }
        }
    }

    public override void TowerAttack(List<Transform> targets)
    {
        // ���� Ÿ���� ���� ����� �����Ƿ� �������� �ʽ��ϴ�.
    }

    public override void SetRange(float range)
    {
        buffRange = range; // ���� ���� ����
        Debug.Log("���� ���� ����: " + buffRange);
    }

    public override void TowerPowUp()
    {
        Debug.Log("���� Ÿ���� �Ŀ����Ǿ����ϴ�!");
    }

    public override void DetectEnemiesInRange()
    {
        // �� �޼���� ���� Ÿ���� �ʿ����� �ʽ��ϴ�.
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange); // ���� ���� �ð�ȭ
    }
}
