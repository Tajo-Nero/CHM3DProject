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
        else
        {
            Debug.Log("��� ���̺� �Ϸ�!");
        }
    }

    private IEnumerator StartWave(Wave waveData)
    {
        int enemyCount = waveData.wave_enemyCount;
        Debug.Log($"���̺� {currentWaveIndex + 1} ����! {enemyCount}������ ���� ������� �����մϴ�.");

        for (int i = 0; i < enemyCount; i++)
        {
            // ���� ����Ʈ�� ��ȯ�ؼ� ��� (���� ���� ����Ʈ�� ���� ��)
            int spawnIndex = i % spawnPoints.Length;
            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            Quaternion spawnRotation = Quaternion.identity;

            // Wave���� ������ ������� �� ����
            int enemyDataIndex = i % waveData.wave_enemyData.Length;
            EnemyData enemyData = waveData.wave_enemyData[enemyDataIndex];

            Debug.Log($"�� ���� {i + 1}/{enemyCount}: {enemyData.enemyName} at ��������Ʈ {spawnIndex}");

            GameObject enemyInstance = enemyPool.SpawnEnemy(enemyData.enemyName, spawnPosition, spawnRotation);
            if (enemyInstance == null)
            {
                Debug.LogError($"�� ������ �����߽��ϴ�: {enemyData.enemyName}");
                continue;
            }

            // �� ������Ʈ ����
            EnemyPathFollower enemyComponent = enemyInstance.GetComponent<EnemyPathFollower>();
            if (enemyComponent != null)
            {
                // EnemyData���� ��Ȯ�� ���� ����
                enemyComponent.enemy_health = enemyData.health;
                enemyComponent.enemy_attackDamage = enemyData.attackPower;
                enemyComponent.enemy_attackSpeed = enemyData.attackSpeed;
                enemyComponent.moveSpeed = enemyData.moveSpeed;

                Debug.Log($"�� ���� ����: {enemyData.enemyName} - ü��={enemyData.health}, �ӵ�={enemyData.moveSpeed}");
            }

            // 1�� �������� ����
            yield return new WaitForSeconds(1f);
        }

        Debug.Log($"���̺� {currentWaveIndex + 1} ���� �Ϸ�!");

        // ��� ���� óġ�Ǿ����� Ȯ���ϴ� �ڷ�ƾ ȣ��
        StartCoroutine(CheckAllEnemiesDefeated());
    }

    private IEnumerator CheckAllEnemiesDefeated()
    {
        // ������ �Ϸ�� �� ��� ���
        yield return new WaitForSeconds(1f);

        while (enemyPool.ActiveEnemies > 0)
        {
            Debug.Log($"���� ��: {enemyPool.ActiveEnemies}����");
            yield return new WaitForSeconds(1f); // 1�ʸ��� üũ
        }

        Debug.Log($"���̺� {currentWaveIndex + 1} �Ϸ�!");
        waveProgressBar.EndWave(); // ���̺� ���� ǥ��

        currentWaveIndex++;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWaveProgress(currentWaveIndex, waves.Length);
        }

        // ���̺� �Ϸ� �˸�
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

    // ���� ���̺� ���� ��� (����׿�)
    public void DebugCurrentWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            Wave currentWave = waves[currentWaveIndex];
            Debug.Log($"=== ���̺� {currentWaveIndex + 1} ���� ===");
            Debug.Log($"�� �� ��: {currentWave.wave_enemyCount}");
            Debug.Log($"�� ���� �迭 ����: {currentWave.wave_enemyData.Length}");

            for (int i = 0; i < currentWave.wave_enemyData.Length; i++)
            {
                var enemy = currentWave.wave_enemyData[i];
                Debug.Log($"  {i}: {enemy.enemyName} (ü��: {enemy.health}, �ӵ�: {enemy.moveSpeed})");
            }
        }
    }
}