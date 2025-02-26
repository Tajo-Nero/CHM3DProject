using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] public GameObject[] enemyPrefabs; // �� ������ �迭
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // ��ü Ǯ ����

    // Ȱ��ȭ�� �� ��� �߰�
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
                   var obj= Instantiate(prefab);
                    obj.SetActive(false); // �ʱ� ���� �� ��Ȱ��ȭ
                    return obj;
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                    activeEnemies.Add(obj); // �� Ȱ��ȭ �� ��Ͽ� �߰�
                    Debug.Log($"�� Ȱ��ȭ��: {obj.name}");
                },
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                    activeEnemies.Remove(obj); // �� ��ȯ �� ��Ͽ��� ����
                    Debug.Log($"�� ��Ȱ��ȭ��: {obj.name}");
                },
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 20
            );

            poolDictionary.Add(prefab.name, pool);
            Debug.Log($"�� ������ �߰���: {prefab.name}");
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
            Debug.LogError($"�� ������ �̸� {enemyName}��(��) ã�� �� �����ϴ�.");
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
            Debug.Log($"�� ��ȯ: {enemyName}");
        }
        else
        {
            Debug.LogError($"�� �������� ��ȯ�� �� �����ϴ�: {enemyName}");
        }
    }
   //�̻��Ѱ����� �� ������ 
    //public GameObject SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    //{
    //    if (poolDictionary.TryGetValue(enemyName, out var pool))
    //    {
    //        GameObject enemyInstance = pool.Get();
    //        enemyInstance.transform.position = position;
    //        enemyInstance.transform.rotation = rotation;
    //        activeEnemies.Add(enemyInstance);
    //        Debug.Log($"�� ���� ��ġ (EnemyPool): {position}, ������ ��: {enemyName}");
    //        return enemyInstance;
    //    }
    //    else
    //    {
    //        Debug.LogError($"�� �������� ã�� �� �����ϴ�: {enemyName}");
    //        return null;
    //    }
    //}




    //public void ReturnEnemy(GameObject enemy)
    //{
    //    var enemyName = enemy.name.Replace("(Clone)", "").Trim(); // "(Clone)" ����
    //
    //    if (poolDictionary.TryGetValue(enemyName, out var pool))
    //    {
    //        pool.Release(enemy);
    //    }
    //    else
    //    {
    //        Debug.LogError($"�� ������ �̸� {enemyName}��(��) ã�� �� �����ϴ�.");
    //    }
    //}
    //�̰� �ߵǴ°� 
    public GameObject SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            GameObject enemyInstance = pool.Get();
            enemyInstance.transform.position = position;
            enemyInstance.transform.rotation = rotation;
    
            NavMeshAgent agent = enemyInstance.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(position); // NavMeshAgent ��ġ �ʱ�ȭ
            }    
            
            return enemyInstance; // GameObject ������ ���� ��ȯ
        }
        else
        {
            Debug.LogError($"�� �������� ã�� �� �����ϴ�: {enemyName}");
            return null; // �� �������� ã�� �� ���� ��� null ��ȯ
        }
    }
    //public void SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    //{
    //    if (poolDictionary.TryGetValue(enemyName, out var pool))
    //    {
    //        GameObject enemyInstance = pool.Get();
    //        enemyInstance.transform.position = position;
    //        enemyInstance.transform.rotation = rotation;
    //        Debug.Log($"�� ������: {enemyName} at {position}");
    //    }
    //    else
    //    {
    //        Debug.LogError($"�� �������� ã�� �� �����ϴ�: {enemyName}");
    //    }
    //}
}
