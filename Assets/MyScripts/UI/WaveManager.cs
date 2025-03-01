using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Wave[] waves; // 웨이브 데이터 배열
    private int currentWaveIndex = 0;

    public Transform[] spawnPoints; // 적 스폰 위치
    private EnemyPool enemyPool;

    public WaveProgressBar waveProgressBar; // WaveProgressBar 스크립트 참조

    private void Start()
    {
        enemyPool = FindObjectOfType<EnemyPool>();
        if (waveProgressBar == null)
        {
            waveProgressBar = FindObjectOfType<WaveProgressBar>();
        }
    }

    public void StartNextWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            waveProgressBar.StartWave(); // 웨이브 시작 표시
            StartCoroutine(StartWave(waves[currentWaveIndex]));
        }
    }

    private IEnumerator StartWave(Wave waveData)
    {
        int enemyCount = waveData.wave_enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            int spawnIndex = i % spawnPoints.Length;
            EnemyData enemyData = waveData.wave_enemyData[i % waveData.wave_enemyData.Length];

            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            Quaternion spawnRotation = Quaternion.identity;

            GameObject enemyInstance = enemyPool.SpawnEnemy(enemyData.enemyName, spawnPosition, spawnRotation);
            if (enemyInstance == null)
            {
                Debug.LogError("적 스폰에 실패했습니다: " + enemyData.enemyName);
                continue;
            }

            EnemyBase enemyComponent = enemyInstance.GetComponent<EnemyBase>();
            if (enemyComponent != null)
            {
                enemyComponent.enemy_health = enemyData.health; // 적의 체력 설정
                enemyComponent.enemy_attackDamage = enemyData.attackPower; // 적의 공격력 설정
            }

            yield return new WaitForSeconds(1f); // 적 생성 대기 시간
        }

        // 모든 적이 처치되었는지 확인하는 코루틴 호출
        StartCoroutine(CheckAllEnemiesDefeated());
    }

    private IEnumerator CheckAllEnemiesDefeated()
    {
        while (enemyPool.ActiveEnemies > 0)
        {
            yield return null;
        }

        waveProgressBar.EndWave(); // 웨이브 종료 표시

        currentWaveIndex++;
        UIManager.Instance.UpdateWaveProgress(currentWaveIndex, waves.Length); // UI 업데이트

        // 웨이브 완료 알림
        GameManager.Instance.OnWaveCompleted();
    }

    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }
}
