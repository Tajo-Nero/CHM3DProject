using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : TowerBase
{
    [SerializeField] private float buffRange = 10f; // ��ȭ ����
    [SerializeField] private float attackPowerMultiplier = 1.2f; // ���ݷ� ���� (20% ����)
    [SerializeField] private float buffDuration = 5f; // ��ȭ ���� �ð�
    [SerializeField] private float buffInterval = 6f; // ��ȭ �ֱ�
    private List<TowerBase> towersInRange = new List<TowerBase>(); // ���� �� Ÿ�� ���

    void Start()
    {
        // �ֱ������� ���� �� Ÿ���� �����ϰ� ��ȭ�ϴ� �ڷ�ƾ ����
        StartCoroutine(BuffTowersInRange());

        // ��ġ ��� ����
        installationCost = 15;
    }

    private IEnumerator BuffTowersInRange()
    {
        while (true)
        {
            DetectTowersInRange(); // ���� �� Ÿ�� ����
            ApplyBuffToTowers(); // ������ Ÿ�� ��ȭ
            yield return new WaitForSeconds(buffInterval); // ��ȭ �ֱ⸶�� ���� �� Ÿ���� �����ϰ� ��ȭ
        }
    }

    private void DetectTowersInRange()
    {
        towersInRange.Clear(); // ���� Ÿ�� ��� �ʱ�ȭ
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, buffRange); // ���� �� �浹ü ����
        foreach (var hitCollider in hitColliders)
        {
            TowerBase tower = hitCollider.GetComponent<TowerBase>();
            if (tower != null && !towersInRange.Contains(tower))
            {
                towersInRange.Add(tower); // ������ Ÿ���� ��Ͽ� �߰�
            }
        }
    }

    private void ApplyBuffToTowers()
    {
        StartCoroutine(ApplyBuffForDuration()); // ��ȭ ���� �ð��� �����ϴ� �ڷ�ƾ ����
    }

    private IEnumerator ApplyBuffForDuration()
    {
        List<TowerBase> buffedTowers = new List<TowerBase>(towersInRange); // ������ Ÿ�� ��� ����

        // Ÿ������ ���ݷ� ����
        foreach (var tower in buffedTowers)
        {
            tower.towerAttackPower *= attackPowerMultiplier; // ���ݷ� ����
        }

        yield return new WaitForSeconds(buffDuration); // ��ȭ ���� �ð� ���� ���

        // Ÿ������ ���ݷ� ������� ����
        foreach (var tower in buffedTowers)
        {
            if (tower != null) // Ÿ���� ���� �����ϴ��� Ȯ��
            {
                tower.towerAttackPower /= attackPowerMultiplier; // ���ݷ� ����
            }
        }
    }

    public override void TowerAttack(List<Transform> targets)
    {
        // ���� Ÿ���� ���� ����� �����Ƿ� ����Ӵϴ�.
    }

    public override void SetRange(float range)
    {
        buffRange = range; // ��ȭ ���� ����
        Debug.Log("��ȭ ���� ������: " + buffRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, buffRange); // ��ȭ ������ �ð������� ǥ��
    }
}
