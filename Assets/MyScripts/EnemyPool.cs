using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    //�ϴ��� ������ �ô´� �۵� ���ϴ��� �𸣰���
    [SerializeField] public GameObject[] enemyPrefabs; // �ټ��� �� ������ �迭
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // �����պ� ��ü Ǯ

    private void Awake()
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        // �� �����տ� ���� ��ü Ǯ�� ����
        foreach (var prefab in enemyPrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefab),
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
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
            Debug.LogError($"Enemy prefab with name {enemyName} not found.");
            return null;
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        var enemyName = enemy.name.Replace("(Clone)", "").Trim(); // "(Clone)" ����

        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            pool.Release(enemy);
        }
        else
        {
            Debug.LogError($"Enemy prefab with name {enemyName} not found.");
        }
    }
    public void SpawnEnemy(Vector3 position, Quaternion rotation)
    {
        // �� �������� �������� ����
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        // �� ����
        var enemyInstance = Instantiate(enemyPrefab, position, rotation);
        // �� ������Ʈ Ȱ��ȭ
        enemyInstance.SetActive(true);
    }
}