using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] public GameObject[] enemyPrefabs; // �� ������ �迭
    [SerializeField] private GameObject healthBarPrefab; // ü�¹� ������
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // ������Ʈ Ǯ ��ųʸ�
    private PathManager pathManager;
    // Ȱ��ȭ�� �� ����Ʈ �߰�
    private List<GameObject> activeEnemies = new List<GameObject>();

    public int ActiveEnemies
    {
        get { return activeEnemies.Count; }
    }

    private void Awake()
    {
        pathManager = FindObjectOfType<PathManager>();
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        // ��� �� �����տ� ���� ������Ʈ Ǯ ����
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
            Debug.LogError($"�ش� �̸��� �� {enemyName}(��)�� ã�� �� �����ϴ�.");
            return null;
        }
    }

    public void ReturnEnemy(GameObject enemyInstance)
    {
        // ü�¹� ����
        Transform healthBar = enemyInstance.transform.Find(healthBarPrefab.name + "(Clone)");
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }

        string enemyName = enemyInstance.name.Replace("(Clone)", "").Trim();
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            pool.Release(enemyInstance);
            activeEnemies.Remove(enemyInstance);
            Debug.Log($"�� ��ȯ��: {enemyName}");
        }
        else
        {
            Debug.LogError($"�� ��ȯ�� �����Ͽ����ϴ�: {enemyName}");
        }
    }


    public GameObject SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            GameObject enemyInstance = pool.Get();

            if (enemyInstance == null)
            {
                Debug.LogError($"�� �ν��Ͻ��� ������ �� �����ϴ�: {enemyName}");
                return null;
            }

            enemyInstance.transform.position = position;
            enemyInstance.transform.rotation = rotation;

            // ��� �Ҵ�
            EnemyPathFollower pathFollower = enemyInstance.GetComponent<EnemyPathFollower>();
            if (pathFollower != null && pathManager != null && pathManager.HasValidPath())
            {
                pathFollower.SetPath(pathManager.GetMainPath());
            }

            // ü�� �ʱ�ȭ
            EnemyPathFollower enemyBase = enemyInstance.GetComponent<EnemyPathFollower>();
            if (enemyBase != null)
            {
                enemyBase.InitializeHealth();
            }

            return enemyInstance;
        }
        else
        {
            Debug.LogError($"���� ã�� �� �����ϴ�: {enemyName}");
            return null;
        }
    }
}
