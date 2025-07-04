using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] public GameObject[] enemyPrefabs; // 적 프리팹 배열
    [SerializeField] private GameObject healthBarPrefab; // 체력바 프리팹
    private Dictionary<string, ObjectPool<GameObject>> poolDictionary; // 오브젝트 풀 딕셔너리

    // 활성화된 적 리스트 추가
    private List<GameObject> activeEnemies = new List<GameObject>();

    public int ActiveEnemies
    {
        get { return activeEnemies.Count; }
    }

    private void Awake()
    {
        poolDictionary = new Dictionary<string, ObjectPool<GameObject>>();

        // 모든 적 프리팹에 대해 오브젝트 풀 생성
        foreach (var prefab in enemyPrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    var obj = Instantiate(prefab);
                    obj.SetActive(false); // 초기 생성 시 비활성화
                    return obj;
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                    activeEnemies.Add(obj); // 활성화된 적 리스트에 추가
                    Debug.Log($"적 활성화됨: {obj.name}");
                },
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                    activeEnemies.Remove(obj); // 활성화된 적 리스트에서 제거
                    Debug.Log($"적 비활성화됨: {obj.name}");
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
            Debug.LogError($"해당 이름의 적 {enemyName}(을)를 찾을 수 없습니다.");
            return null;
        }
    }

    public void ReturnEnemy(GameObject enemyInstance)
    {
        // 체력바 삭제
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
            Debug.Log($"적 반환됨: {enemyName}");
        }
        else
        {
            Debug.LogError($"적 반환에 실패하였습니다: {enemyName}");
        }
    }


    public GameObject SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary.TryGetValue(enemyName, out var pool))
        {
            GameObject enemyInstance = pool.Get();

            if (enemyInstance == null)
            {
                Debug.LogError($"적 인스턴스를 생성할 수 없습니다: {enemyName}");
                return null;
            }

            enemyInstance.transform.position = position;
            enemyInstance.transform.rotation = rotation;

            NavMeshAgent agent = enemyInstance.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.Warp(position); // NavMeshAgent 위치 초기화
            }
            else
            {
                Debug.LogError($"해당 적에는 NavMeshAgent가 없습니다: {enemyName}");
            }

            EnemyBase enemyBase = enemyInstance.GetComponent<EnemyBase>();
            if (enemyBase != null)
            {
                enemyBase.InitializeHealth(); // 체력 초기화
            }

            return enemyInstance; // GameObject 인스턴스 반환
        }
        else
        {
            Debug.LogError($"적을 찾을 수 없습니다: {enemyName}");
            return null; // 적을 찾을 수 없는 경우 null 반환
        }
    }
}
