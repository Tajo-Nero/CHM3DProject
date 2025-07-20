using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    // ����ȭ�� Ǯ �׸�
    [System.Serializable]
    public class PoolItem
    {
        public EnemyType enemyType;
        public int poolSize = 10;
        [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
    }

    // ��ü Ǯ ���
    public List<PoolItem> poolItems = new List<PoolItem>();

    // EnemyData ���
    public List<EnemyData> enemyDataList;

    // �ʱ�ȭ �� ���� ����
    private Dictionary<EnemyType, EnemyData> enemyDataMap = new Dictionary<EnemyType, EnemyData>();

    private void Awake()
    {
        // ���� ����
        foreach (var data in enemyDataList)
        {
            enemyDataMap[data.enemyType] = data;
        }

        // Ǯ �ʱ�ȭ
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var item in poolItems)
        {
            // �ش� Ÿ���� EnemyData ã��
            if (enemyDataMap.TryGetValue(item.enemyType, out EnemyData data))
            {
                // Ǯ ����
                for (int i = 0; i < item.poolSize; i++)
                {
                    GameObject obj = Instantiate(data.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);

                    // Ǯ�� �߰�
                    item.pool.Enqueue(obj);
                }
            }
        }
    }

    // Ÿ������ �� �������� - �����ϰ� ������
    public GameObject GetEnemy(EnemyType type, Vector3 position)
    {
        foreach (var item in poolItems)
        {
            if (item.enemyType == type && item.pool.Count > 0)
            {
                GameObject enemy = item.pool.Dequeue();
                enemy.transform.position = position;
                enemy.SetActive(true);

                // EnemyData ����
                EnemyPathFollower follower = enemy.GetComponent<EnemyPathFollower>();
                if (follower != null && enemyDataMap.TryGetValue(type, out EnemyData data))
                {
                    follower.enemyData = data;
                }

                return enemy;
            }
        }

        // Ǯ�� ������� ���� ����
        return CreateNewEnemy(type, position);
    }

    // �� �� ����
    private GameObject CreateNewEnemy(EnemyType type, Vector3 position)
    {
        if (enemyDataMap.TryGetValue(type, out EnemyData data))
        {
            GameObject enemy = Instantiate(data.prefab, position, Quaternion.identity);

            EnemyPathFollower follower = enemy.GetComponent<EnemyPathFollower>();
            if (follower != null)
            {
                follower.enemyData = data;
            }

            return enemy;
        }

        Debug.LogError($"EnemyData not found for type: {type}");
        return null;
    }

    // �� ��ȯ (��Ȱ��ȭ)
    public void ReturnEnemy(GameObject enemy)
    {
        EnemyPathFollower follower = enemy.GetComponent<EnemyPathFollower>();
        if (follower != null && follower.enemyData != null)
        {
            EnemyType type = follower.enemyData.enemyType;

            foreach (var item in poolItems)
            {
                if (item.enemyType == type)
                {
                    enemy.SetActive(false);
                    enemy.transform.SetParent(transform);
                    item.pool.Enqueue(enemy);
                    return;
                }
            }
        }

        // ��Ī�Ǵ� Ǯ�� ������ �ı�
        Destroy(enemy);
    }

    // ȣȯ�� ����: ���ڿ��� ��������
    public GameObject GetEnemy(string enemyName, Vector3 position)
    {
        if (System.Enum.TryParse(enemyName, out EnemyType type))
        {
            return GetEnemy(type, position);
        }

        Debug.LogError($"Unknown enemy name: {enemyName}");
        return null;
    }
}