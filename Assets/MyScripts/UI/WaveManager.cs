using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public Wave[] waves; // ���̺� ������ �迭
    private int currentWaveIndex = 0;

    public Transform[] spawnPoints; // �� ���� ��ġ
    private EnemyPool enemyPool;

    public WaveProgressBar waveProgressBar; // WaveProgressBar ��ũ��Ʈ ����

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
            waveProgressBar.StartWave(); // ���̺� ���� ǥ��
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
                Debug.LogError("�� ������ �����߽��ϴ�: " + enemyData.enemyName);
                continue;
            }

            EnemyBase enemyComponent = enemyInstance.GetComponent<EnemyBase>();
            if (enemyComponent != null)
            {
                enemyComponent.enemy_health = enemyData.health; // ���� ü�� ����
                enemyComponent.enemy_attackDamage = enemyData.attackPower; // ���� ���ݷ� ����
            }

            yield return new WaitForSeconds(1f); // �� ���� ��� �ð�
        }

        // ��� ���� óġ�Ǿ����� Ȯ���ϴ� �ڷ�ƾ ȣ��
        StartCoroutine(CheckAllEnemiesDefeated());
    }

    private IEnumerator CheckAllEnemiesDefeated()
    {
        while (enemyPool.ActiveEnemies > 0)
        {
            yield return null;
        }

        waveProgressBar.EndWave(); // ���̺� ���� ǥ��

        currentWaveIndex++;
        UIManager.Instance.UpdateWaveProgress(currentWaveIndex, waves.Length); // UI ������Ʈ

        // ���̺� �Ϸ� �˸�
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
