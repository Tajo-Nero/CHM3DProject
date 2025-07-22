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

    public GameObject GetEnemy(EnemyType type, Vector3 position)
    {
        Debug.Log($"요청된 적 타입: {type}");

        foreach (var item in poolItems)
        {
            if (item.enemyType == type && item.pool.Count > 0)
            {
                GameObject enemy = item.pool.Dequeue();
                Debug.Log($"풀에서 가져옴: {enemy.name}");

                enemy.transform.position = position;
                enemy.transform.rotation = Quaternion.identity;
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
        Debug.Log($"풀이 비어서 새로 생성: {type}");
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

            // 경로와 상태 초기화
            follower.currentPath = null;
            follower.currentWaypointIndex = 0;
            follower.enemy_health = follower.enemyData.health; // 체력 리셋

            // 체력바 초기화
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
                    enemy.transform.position = Vector3.zero; // 위치 초기화
                    item.pool.Enqueue(enemy);
                    return;
                }
            }
        }

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
    [ContextMenu("Wave 기반 자동 설정")]
    private void SetupPoolBasedOnWaves()
    {
        // WaveManager에서 모든 Wave 가져오기
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager == null || waveManager.waves == null)
        {
            Debug.LogError("WaveManager를 찾을 수 없습니다!");
            return;
        }

        // 사용되는 모든 적 타입 수집
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

        // Pool Items 재구성
        poolItems.Clear();
        foreach (EnemyType type in usedEnemyTypes)
        {
            PoolItem item = new PoolItem();
            item.enemyType = type;
            item.poolSize = 10; // 기본값
            poolItems.Add(item);
        }

        Debug.Log($"Wave에서 사용하는 {usedEnemyTypes.Count}개 적 타입으로 Pool 설정 완료!");
    }
}