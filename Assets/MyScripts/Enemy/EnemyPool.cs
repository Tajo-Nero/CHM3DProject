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

    public GameObject GetEnemy(EnemyType type, Vector3 position)
    {
        Debug.Log($"��û�� �� Ÿ��: {type}");

        foreach (var item in poolItems)
        {
            if (item.enemyType == type && item.pool.Count > 0)
            {
                GameObject enemy = item.pool.Dequeue();
                Debug.Log($"Ǯ���� ������: {enemy.name}");

                enemy.transform.position = position;
                enemy.transform.rotation = Quaternion.identity;
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
        Debug.Log($"Ǯ�� �� ���� ����: {type}");
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

            // ��ο� ���� �ʱ�ȭ
            follower.currentPath = null;
            follower.currentWaypointIndex = 0;
            follower.enemy_health = follower.enemyData.health; // ü�� ����

            // ü�¹� �ʱ�ȭ
            MyHealthBar healthBar = enemy.GetComponentInChildren<MyHealthBar>();
            if (healthBar != null)
            {
                healthBar.Initialize(follower.enemyData.health);
            }

            foreach (var item in poolItems)
            {
                if (item.enemyType == type)
                {
                    enemy.SetActive(false);
                    enemy.transform.SetParent(transform);
                    enemy.transform.position = Vector3.zero; // ��ġ �ʱ�ȭ
                    item.pool.Enqueue(enemy);
                    return;
                }
            }
        }

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
    [ContextMenu("Wave ��� �ڵ� ����")]
    private void SetupPoolBasedOnWaves()
    {
        // WaveManager���� ��� Wave ��������
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager == null || waveManager.waves == null)
        {
            Debug.LogError("WaveManager�� ã�� �� �����ϴ�!");
            return;
        }

        // ���Ǵ� ��� �� Ÿ�� ����
        HashSet<EnemyType> usedEnemyTypes = new HashSet<EnemyType>();

        foreach (Wave wave in waveManager.waves)
        {
            if (wave.wave_enemyData != null)
            {
                foreach (EnemyData data in wave.wave_enemyData)
                {
                    if (data != null)
                    {
                        usedEnemyTypes.Add(data.enemyType);
                    }
                }
            }
        }

        // Pool Items �籸��
        poolItems.Clear();
        foreach (EnemyType type in usedEnemyTypes)
        {
            PoolItem item = new PoolItem();
            item.enemyType = type;
            item.poolSize = 10; // �⺻��
            poolItems.Add(item);
        }

        Debug.Log($"Wave���� ����ϴ� {usedEnemyTypes.Count}�� �� Ÿ������ Pool ���� �Ϸ�!");
    }
}