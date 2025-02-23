using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    //일단은 구현해 봤는대 작동 잘하는지 모르겠음
    [SerializeField] public GameObject[] enemyPrefabs; // 다수의 적 프리팹 배열
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // 프리팹별 객체 풀

    private void Awake()
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        // 각 프리팹에 대한 객체 풀을 생성
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
        var enemyName = enemy.name.Replace("(Clone)", "").Trim(); // "(Clone)" 제거

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
        // 적 프리팹을 무작위로 선택
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        // 적 생성
        var enemyInstance = Instantiate(enemyPrefab, position, rotation);
        // 적 오브젝트 활성화
        enemyInstance.SetActive(true);
    }
}