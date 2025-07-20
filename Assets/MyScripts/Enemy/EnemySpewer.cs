using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpewer : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public string[] enemyNames; // 스폰할 적의 이름들 (EnemyPool에 등록된 이름)
    public int enemiesPerWave = 10; // 웨이브당 적 수
    public float spawnInterval = 1f; // 스폰 간격

    private bool isSpawning = false; // 생성 중 여부
    private EnemyPool enemyPool;
    private PathManager pathManager;

    void Start()
    {
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool을 찾을 수 없습니다!");
        }

        if (pathManager == null)
        {
            Debug.LogError("PathManager를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        // G 키를 눌러도 경로가 생성되지 않았으면 스폰하지 않음
        if (Input.GetKeyDown(KeyCode.G) && !isSpawning)
        {
            if (pathManager.GetMainPath() != null && pathManager.GetMainPath().Count >= 2)
            {
                StartCoroutine(SpawnEnemies());
            }
            else
            {
                Debug.LogWarning("경로가 설정되지 않았습니다! 먼저 차량으로 길을 만드세요.");
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool이 없어서 적을 스폰할 수 없습니다!");
            yield break;
        }

        if (enemyNames == null || enemyNames.Length == 0)
        {
            Debug.LogError("스폰할 적의 이름이 설정되지 않았습니다!");
            yield break;
        }

        isSpawning = true;
        Debug.Log($"적 스폰 시작! {enemiesPerWave}마리의 적을 순서대로 생성합니다.");

        for (int i = 0; i < enemiesPerWave; i++)
        {
            // 순서대로 적 선택 (배열을 순환)
            int enemyIndex = i % enemyNames.Length;
            string enemyName = enemyNames[enemyIndex];

            // EnemyPool을 통해 적 스폰
            GameObject spawnedEnemy = enemyPool.GetEnemy(enemyName, transform.position);

            if (spawnedEnemy != null)
            {
                spawnedEnemy.transform.rotation = transform.rotation;
                Debug.Log($"적 스폰 성공: {enemyName} ({i + 1}/{enemiesPerWave}) - 순서: {enemyIndex}");
            }
            else
            {
                Debug.LogError($"적 스폰 실패: {enemyName}");
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
        Debug.Log("적 스폰 완료!");
    }

    // 외부에서 호출할 수 있는 스폰 메서드
    public void SpawnWave(int waveSize = -1)
    {
        if (isSpawning)
        {
            Debug.LogWarning("이미 스폰 중입니다!");
            return;
        }

        if (waveSize > 0)
        {
            enemiesPerWave = waveSize;
        }

        StartCoroutine(SpawnEnemies());
    }

    // 특정 적만 스폰
    public void SpawnSpecificEnemy(string enemyName, int count = 1)
    {
        if (isSpawning)
        {
            Debug.LogWarning("이미 스폰 중입니다!");
            return;
        }

        StartCoroutine(SpawnSpecificEnemyCoroutine(enemyName, count));
    }

    private IEnumerator SpawnSpecificEnemyCoroutine(string enemyName, int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            GameObject spawnedEnemy = enemyPool.GetEnemy(enemyName, transform.position);

            if (spawnedEnemy != null)
            {
                spawnedEnemy.transform.rotation = transform.rotation;
                Debug.Log($"특정 적 스폰: {enemyName} ({i + 1}/{count})");
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    void OnDrawGizmos()
    {
        // 스폰 지점 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // 스폰 방향 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}