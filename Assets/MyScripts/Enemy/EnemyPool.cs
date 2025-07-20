using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    // 간소화된 풀 항목
    [System.Serializable]
    public class PoolItem
    {
        public EnemyType enemyType;
        public int poolSize = 10;
        [HideInInspector] public Queue<GameObject> pool = new Queue<GameObject>();
    }

    // 전체 풀 목록
    public List<PoolItem> poolItems = new List<PoolItem>();

    // EnemyData 목록
    public List<EnemyData> enemyDataList;

    // 초기화 시 맵핑 생성
    private Dictionary<EnemyType, EnemyData> enemyDataMap = new Dictionary<EnemyType, EnemyData>();

    private void Awake()
    {
        // 맵핑 생성
        foreach (var data in enemyDataList)
        {
            enemyDataMap[data.enemyType] = data;
        }

        // 풀 초기화
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var item in poolItems)
        {
            // 해당 타입의 EnemyData 찾기
            if (enemyDataMap.TryGetValue(item.enemyType, out EnemyData data))
            {
                // 풀 생성
                for (int i = 0; i < item.poolSize; i++)
                {
                    GameObject obj = Instantiate(data.prefab);
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);

                    // 풀에 추가
                    item.pool.Enqueue(obj);
                }
            }
        }
    }

    // 타입으로 적 가져오기 - 간단하고 직관적
    public GameObject GetEnemy(EnemyType type, Vector3 position)
    {
        foreach (var item in poolItems)
        {
            if (item.enemyType == type && item.pool.Count > 0)
            {
                GameObject enemy = item.pool.Dequeue();
                enemy.transform.position = position;
                enemy.SetActive(true);

                // EnemyData 설정
                EnemyPathFollower follower = enemy.GetComponent<EnemyPathFollower>();
                if (follower != null && enemyDataMap.TryGetValue(type, out EnemyData data))
                {
                    follower.enemyData = data;
                }

                return enemy;
            }
        }

        // 풀이 비었으면 새로 생성
        return CreateNewEnemy(type, position);
    }

    // 새 적 생성
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

    // 적 반환 (비활성화)
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

        // 매칭되는 풀이 없으면 파괴
        Destroy(enemy);
    }

    // 호환성 유지: 문자열로 가져오기
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