using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] public GameObject[] enemyPrefabs; // 적 프리팹 배열
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // 객체 풀 사전

    // 활성화된 적 목록 추가
    private List<GameObject> activeEnemies = new List<GameObject>();

    public int ActiveEnemies
    {
        get { return activeEnemies.Count; }
    }

    private void Awake()
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        // 모든 적 프리팹에 대해 객체 풀 생성
        foreach (var prefab in enemyPrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                   var obj= Instantiate(prefab);
                    obj.SetActive(false); // 초기 생성 시 비활성화
                    return obj;
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                    activeEnemies.Add(obj); // 적 활성화 시 목록에 추가
                    Debug.Log($"적 활성화됨: {obj.name}");
                },
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                    activeEnemies.Remove(obj); // 적 반환 시 목록에서 제거
                    Debug.Log($"적 비활성화됨: {obj.name}");
                },
                actionOnDestroy: obj => Destroy(obj),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 20
            );

            poolDictionary.Add(prefab.name, pool);
            Debug.Log($"적 프리팹 추가됨: {prefab.name}");
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
            Debug.LogError($"적 프리팹 이름 {enemyName}을(를) 찾을 수 없습니다.");
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
            Debug.Log($"적 반환: {enemyName}");
        }
        else
        {
            Debug.LogError($"적 프리팹을 반환할 수 없습니다: {enemyName}");
        }
    }
   //이상한곳에서 적 생성됨 
    //public GameObject SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    //{
    //    if (poolDictionary.TryGetValue(enemyName, out var pool))
    //    {
    //        GameObject enemyInstance = pool.Get();
    //        enemyInstance.transform.position = position;
    //        enemyInstance.transform.rotation = rotation;
    //        activeEnemies.Add(enemyInstance);
    //        Debug.Log($"적 생성 위치 (EnemyPool): {position}, 생성된 적: {enemyName}");
    //        return enemyInstance;
    //    }
    //    else
    //    {
    //        Debug.LogError($"적 프리팹을 찾을 수 없습니다: {enemyName}");
    //        return null;
    //    }
    //}




    //public void ReturnEnemy(GameObject enemy)
    //{
    //    var enemyName = enemy.name.Replace("(Clone)", "").Trim(); // "(Clone)" 제거
    //
    //    if (poolDictionary.TryGetValue(enemyName, out var pool))
    //    {
    //        pool.Release(enemy);
    //    }
    //    else
    //    {
    //        Debug.LogError($"적 프리팹 이름 {enemyName}을(를) 찾을 수 없습니다.");
    //    }
    //}
    //이게 잘되는거 
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
                agent.Warp(position); // NavMeshAgent 위치 초기화
            }    
            
            return enemyInstance; // GameObject 형식의 값을 반환
        }
        else
        {
            Debug.LogError($"적 프리팹을 찾을 수 없습니다: {enemyName}");
            return null; // 적 프리팹을 찾을 수 없는 경우 null 반환
        }
    }
    //public void SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    //{
    //    if (poolDictionary.TryGetValue(enemyName, out var pool))
    //    {
    //        GameObject enemyInstance = pool.Get();
    //        enemyInstance.transform.position = position;
    //        enemyInstance.transform.rotation = rotation;
    //        Debug.Log($"적 생성됨: {enemyName} at {position}");
    //    }
    //    else
    //    {
    //        Debug.LogError($"적 프리팹을 찾을 수 없습니다: {enemyName}");
    //    }
    //}
}
