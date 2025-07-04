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
        else
        {
            Debug.Log("모든 웨이브 완료!");
        }
    }

    private IEnumerator StartWave(Wave waveData)
    {
        int enemyCount = waveData.wave_enemyCount;
        Debug.Log($"웨이브 {currentWaveIndex + 1} 시작! {enemyCount}마리의 적을 순서대로 스폰합니다.");

        for (int i = 0; i < enemyCount; i++)
        {
            // 스폰 포인트는 순환해서 사용 (여러 스폰 포인트가 있을 때)
            int spawnIndex = i % spawnPoints.Length;
            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            Quaternion spawnRotation = Quaternion.identity;

            // Wave에서 지정한 순서대로 적 선택
            int enemyDataIndex = i % waveData.wave_enemyData.Length;
            EnemyData enemyData = waveData.wave_enemyData[enemyDataIndex];

            Debug.Log($"적 스폰 {i + 1}/{enemyCount}: {enemyData.enemyName} at 스폰포인트 {spawnIndex}");

            GameObject enemyInstance = enemyPool.SpawnEnemy(enemyData.enemyName, spawnPosition, spawnRotation);
            if (enemyInstance == null)
            {
                Debug.LogError($"적 스폰에 실패했습니다: {enemyData.enemyName}");
                continue;
            }

            // 적 컴포넌트 설정
            EnemyPathFollower enemyComponent = enemyInstance.GetComponent<EnemyPathFollower>();
            if (enemyComponent != null)
            {
                // EnemyData에서 정확한 스탯 적용
                enemyComponent.enemy_health = enemyData.health;
                enemyComponent.enemy_attackDamage = enemyData.attackPower;
                enemyComponent.enemy_attackSpeed = enemyData.attackSpeed;
                enemyComponent.moveSpeed = enemyData.moveSpeed;

                Debug.Log($"적 스탯 설정: {enemyData.enemyName} - 체력={enemyData.health}, 속도={enemyData.moveSpeed}");
            }

            // 1초 간격으로 스폰
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"웨이브 {currentWaveIndex + 1} 스폰 완료!");

        // 모든 적이 처치되었는지 확인하는 코루틴 호출
        StartCoroutine(CheckAllEnemiesDefeated());
    }

    private IEnumerator CheckAllEnemiesDefeated()
    {
        // 스폰이 완료된 후 잠시 대기
        yield return new WaitForSeconds(1f);

        while (enemyPool.ActiveEnemies > 0)
        {
            Debug.Log($"남은 적: {enemyPool.ActiveEnemies}마리");
            yield return new WaitForSeconds(1f); // 1초마다 체크
        }

        Debug.Log($"웨이브 {currentWaveIndex + 1} 완료!");
        waveProgressBar.EndWave(); // 웨이브 종료 표시

        currentWaveIndex++;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWaveProgress(currentWaveIndex, waves.Length);
        }

        // 웨이브 완료 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnWaveCompleted();
        }
    }

    public int GetCurrentWaveIndex()
    {
        return currentWaveIndex;
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }

    // 현재 웨이브 정보 출력 (디버그용)
    public void DebugCurrentWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log($"=== 웨이브 {currentWaveIndex + 1} 정보 ===");
            Debug.Log($"총 적 수: {currentWave.wave_enemyCount}");
            Debug.Log($"적 종류 배열 길이: {currentWave.wave_enemyData.Length}");

            for (int i = 0; i < currentWave.wave_enemyData.Length; i++)
            {
                var enemy = currentWave.wave_enemyData[i];
                Debug.Log($"  {i}: {enemy.enemyName} (체력: {enemy.health}, 속도: {enemy.moveSpeed})");
            }
        }
    }
}