using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] public GameObject[] enemyPrefabs; // �� ������ �迭
    [SerializeField] private GameObject healthBarPrefab; // ü�� �� ������
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // ��ü Ǯ ��ųʸ�

    // Ȱ��ȭ�� �� ����Ʈ
    private List<GameObject> activeEnemies = new List<GameObject>();

    public int ActiveEnemies
    {
        get { return activeEnemies.Count; }
    }

    private void Awake()
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        // ��� �� �����տ� ���� ��ü Ǯ ����
        foreach (var prefab in enemyPrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(prefab);
                    obj.SetActive(false); // �ʱ� ���� �� ��Ȱ��ȭ
                    return obj;
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                    activeEnemies.Add(obj); // Ȱ��ȭ�� �� ����Ʈ�� �߰�
                    Debug.Log($"�� Ȱ��ȭ��: {obj.name}");
                },
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                    activeEnemies.Remove(obj); // Ȱ��ȭ�� �� ����Ʈ���� ����
                    Debug.Log($"�� ��Ȱ��ȭ��: {obj.name}");
                },
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 20
            );

            poolDictionary.Add(prefab.name, pool);
        }
    }

    public GameObject GetEnemy(string enemyName)
    {
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            return pool.Get();
        }
        else
        {
            Debug.LogError($"�� �̸� {enemyName}(��)�� ã�� �� �����ϴ�.");
            return null;
        }
    }

    public void ReturnEnemy(GameObject enemyInstance)
    {
        string enemyName = enemyInstance.name.Replace("(Clone)", "").Trim();
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            pool.Release(enemyInstance);
            activeEnemies.Remove(enemyInstance);
            Debug.Log($"�� ��ȯ��: {enemyName}");
        }
        else
        {
            Debug.LogError($"�� ��ȯ�� �����߽��ϴ�: {enemyName}");
        }
    }

    public GameObject SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            GameObject enemyInstance = pool.Get();

            if (enemyInstance == null)
            {
                Debug.LogError($"���� ������ �� �����ϴ�: {enemyName}");
                return null;
            }

            enemyInstance.transform.position = position;
            enemyInstance.transform.rotation = rotation;

            // NavMeshAgent �ʱ�ȭ
            NavMeshAgent agent = enemyInstance.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(position);
            }
            else
            {
                Debug.LogError($"���� NavMeshAgent�� �����ϴ�: {enemyName}");
            }

            // ü�� �� �߰� �� �ʱ�ȭ
            if (enemyInstance.GetComponentInChildren<MyHealthBar>() == null)
            {
                GameObject healthBar = Instantiate(healthBarPrefab, enemyInstance.transform);
                MyHealthBar myHealthBar = healthBar.GetComponent<MyHealthBar>();
                EnemyBase enemyScript = enemyInstance.GetComponent<EnemyBase>();
                if (myHealthBar != null && enemyScript != null)
                {
                    myHealthBar.Initialize(enemyScript.enemy_health);
                    enemyScript.healthBar = myHealthBar;
                }
            }

            activeEnemies.Add(enemyInstance); // Ȱ��ȭ�� �� ����Ʈ�� �߰�
            return enemyInstance; // GameObject �ν��Ͻ� ��ȯ
        }
        else
        {
            Debug.LogError($"���� ã�� �� �����ϴ�: {enemyName}");
            return null; // ���� ã�� �� ������ null ��ȯ
        }
    }
}
