using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("���̺� ����")]
    public Wave[] waves;
    public int currentWaveIndex = -1;
    public bool isWaveActive = false;
    public bool waitingForNextWave = false;

    [Header("���� ����")]
    public Transform[] spawnPoints;
    public float timeBetweenSpawns = 1.5f;

    [Header("UI ����")]
    public Text waveText;
    public WaveProgressBar waveProgressBar;

    // ����
    private GameManager gameManager;
    private EnemyPool enemyPool;
    private PathManager pathManager;

    // ���� ����
    private int remainingEnemies = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // ���̺� �Ϸ� �̺�Ʈ
    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveCompleted;
    private bool isPlayerReady = false;  // �÷��̾� �غ� ����

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        EnemyPathFollower.OnAnyEnemyDeath += HandleEnemyDeath;

        currentWaveIndex = -1;

        // �ʱ⿡�� ���̺� �ؽ�Ʈ ����
        if (waveText != null)
        {
            waveText.text = "";
        }

        UpdateWaveUI();
    }

    void OnDestroy()
    {
        EnemyPathFollower.OnAnyEnemyDeath -= HandleEnemyDeath;
    }

    void Update()
    {
        // �÷��̾ �غ�Ǿ���, ���̺갡 Ȱ��ȭ���� �ʾҰ�, Ȱ�� ���� ���� ����
        if (Input.GetKeyDown(KeyCode.G) && isPlayerReady && !isWaveActive && activeEnemies.Count == 0)
        {
            StartNextWave();
        }
    }

    // �÷��̾ �غ�Ǿ��� �� ȣ�� (GameManager���� ȣ��)
    public void OnPlayerReady()
    {
        isPlayerReady = true;

        // WaveProgressBar���� �˸�
        if (waveProgressBar != null)
        {
            waveProgressBar.OnPlayerReady();
        }

        // ���̺� �ؽ�Ʈ�� ǥ������ ���� (WaveProgressBar�� statusText�� ��ü)
        if (waveText != null)
        {
            waveText.text = "";
        }
    }

    public void StartNextWave()
    {
        if (isWaveActive || activeEnemies.Count > 0)
        {
            Debug.Log($"���̺� ���� �Ұ� - Ȱ��: {isWaveActive}, ���� ��: {activeEnemies.Count}");
            return;
        }

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            GameWon();
            return;
        }

        Debug.Log($"���̺� {currentWaveIndex + 1} ����!");
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        UpdateWaveUI();
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveActive = true;
        waitingForNextWave = false;

        int totalEnemies = wave.wave_enemyCount;
        remainingEnemies = totalEnemies;

        if (waveProgressBar != null)
        {
            waveProgressBar.StartWave();
        }

        // ��� Ȯ��
        List<Vector3> mainPath = pathManager != null ? pathManager.GetMainPath() : null;
        if (mainPath == null || mainPath.Count == 0)
        {
            Debug.LogWarning("��ΰ� �������� �ʾҽ��ϴ�. �⺻ ��θ� ����մϴ�.");
            mainPath = GenerateDefaultPath();
        }

        // ��Ȯ�� wave_enemyCount��ŭ�� ����
        for (int i = 0; i < totalEnemies; i++)
        {
            EnemyData enemyData = GetEnemyDataFromWave(wave, i);

            if (enemyData != null)
            {
                GameObject enemy = SpawnEnemy(enemyData, mainPath);

                if (enemy == null)
                {
                    Debug.LogError($"�� ���� ����: {i + 1}/{totalEnemies}");
                    remainingEnemies--;
                }
            }
            else
            {
                Debug.LogError($"EnemyData�� null�Դϴ�! �ε���: {i}");
                remainingEnemies--;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        Debug.Log($"���̺� {currentWaveIndex + 1} ���� �Ϸ� - Ȱ�� ��: {activeEnemies.Count}");
    }

    private EnemyData GetEnemyDataFromWave(Wave wave, int index)
    {
        if (wave.wave_enemyData == null || wave.wave_enemyData.Length == 0)
        {
            Debug.LogError($"���̺� {currentWaveIndex + 1}�� �� �����Ͱ� �����ϴ�!");
            return null;
        }

        if (wave.wave_enemyData.Length == 1)
        {
            return wave.wave_enemyData[0];
        }
        else
        {
            int dataIndex = index % wave.wave_enemyData.Length;
            return wave.wave_enemyData[dataIndex];
        }
    }

    private List<Vector3> GenerateDefaultPath()
    {
        List<Vector3> defaultPath = new List<Vector3>();

        if (spawnPoints.Length > 0)
        {
            defaultPath.Add(spawnPoints[0].position);

            Nexus nexus = FindObjectOfType<Nexus>();
            if (nexus != null)
            {
                defaultPath.Add(nexus.transform.position);
            }
            else
            {
                defaultPath.Add(new Vector3(0, 0, 0));
            }
        }

        return defaultPath;
    }

    private GameObject SpawnEnemy(EnemyData enemyData, List<Vector3> path)
    {
        if (spawnPoints.Length == 0 || enemyPool == null)
        {
            Debug.LogError("���� ����Ʈ �Ǵ� �� Ǯ�� �������� �ʾҽ��ϴ�!");
            return null;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = enemyPool.GetEnemy(enemyData.enemyType, spawnPoint.position);

        if (enemy != null)
        {
            activeEnemies.Add(enemy);

            EnemyPathFollower pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
            }
        }

        return enemy;
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            remainingEnemies--;

            UpdateWaveProgress();

            if (remainingEnemies <= 0 && isWaveActive)
            {
                WaveCompleted();
            }
        }
    }

    private void UpdateWaveProgress()
    {
        if (waveProgressBar != null && currentWaveIndex < waves.Length)
        {
            int totalEnemies = waves[currentWaveIndex].wave_enemyCount;
            int killedEnemies = totalEnemies - remainingEnemies;

            float progress = (float)killedEnemies / totalEnemies;
            waveProgressBar.UpdateProgress(progress);
        }
    }

    private void WaveCompleted()
    {
        isWaveActive = false;
        waitingForNextWave = false;

        Debug.Log($"���̺� {currentWaveIndex + 1} �Ϸ�!");

        // ���� ����
        if (gameManager != null)
        {
            int reward = 50 + (currentWaveIndex * 10);
            gameManager.AddResources(reward);
            Debug.Log($"���̺� ����: {reward} �ڿ�");
        }

        // OnWaveCompleted �̺�Ʈ �߻� (WaveProgressBar�� ���� ��)
        if (OnWaveCompleted != null)
        {
            OnWaveCompleted.Invoke(currentWaveIndex + 1);
        }

        // EndWave�� ȣ������ ���� (OnWaveCompleted���� ó����)

        // ���̺� �Ϸ� �� waveText�� ������� ����
        if (waveText != null)
        {
            waveText.text = "";
        }
    }

    private void GameWon()
    {
        Debug.Log("���� �¸�! ��� ���̺긦 Ŭ�����߽��ϴ�.");

        if (waveText != null)
        {
            waveText.text = "�¸�! ��� ���̺� Ŭ����";
        }

        if (gameManager != null)
        {
            gameManager.GameWon();
        }
    }

    private void UpdateWaveUI()
    {
        // waveText�� ���� �¸� �ÿ��� ���
        if (waveText != null && currentWaveIndex >= 0 && currentWaveIndex < waves.Length)
        {
            waveText.text = "";  // ���ÿ��� �� �ؽ�Ʈ
        }
    }

    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }

    public float GetWaveProgress()
    {
        if (currentWaveIndex < 0 || currentWaveIndex >= waves.Length)
            return 0;

        int totalEnemies = waves[currentWaveIndex].wave_enemyCount;
        if (totalEnemies <= 0)
            return 1;

        return (float)(totalEnemies - remainingEnemies) / totalEnemies;
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies.ToArray())
        {
            if (enemy != null)
            {
                enemyPool.ReturnEnemy(enemy);
            }
        }

        activeEnemies.Clear();
        remainingEnemies = 0;

        if (isWaveActive)
        {
            WaveCompleted();
        }
    }

    public void ResetWaves()
    {
        StopAllCoroutines();
        ClearAllEnemies();

        currentWaveIndex = -1;
        isWaveActive = false;
        waitingForNextWave = false;
        isPlayerReady = false;  // �÷��̾� �غ� ���µ� ����

        UpdateWaveUI();

        if (waveProgressBar != null)
        {
            waveProgressBar.ResetProgressBar();
        }
    }
}