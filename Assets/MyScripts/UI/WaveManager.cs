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
    private bool isPlayerReady = false;  // �÷��̾� �غ� ���� �߰�

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        EnemyPathFollower.OnAnyEnemyDeath += HandleEnemyDeath;

        currentWaveIndex = -1;

        // �ʱ⿡�� �ȳ� �ؽ�Ʈ ����
        if (waveText != null)
        {
            waveText.text = "";  // �� �ؽ�Ʈ�� ����
        }

        UpdateWaveUI();
    }

    void OnDestroy()
    {
        // �̺�Ʈ ���� ����
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
    // �÷��̾ �غ�Ǿ��� �� ȣ��
    public void OnPlayerReady()
    {
        isPlayerReady = true;

        if (waveText != null && currentWaveIndex < 0)
        {
            waveText.text = "G Ű�� ���� ù ���̺� ����";
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

        Debug.Log($"���̺� {currentWaveIndex + 1} ����!");

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
                    remainingEnemies--; // ���� ������ ���� ī��Ʈ���� ����
                }
                else
                {
                    Debug.Log($"�� ���� ����: {enemyData.enemyType} ({i + 1}/{totalEnemies})");
                }
            }
            else
            {
                Debug.LogError($"EnemyData�� null�Դϴ�! �ε���: {i}");
                remainingEnemies--; // �����Ͱ� ���� ���� ī��Ʈ���� ����
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        Debug.Log($"���̺� {currentWaveIndex + 1} ���� �Ϸ� - Ȱ�� ��: {activeEnemies.Count}, ���� ��: {remainingEnemies}");
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
            // Ȱ�� �� ����Ʈ�� �߰�
            activeEnemies.Add(enemy);

            // ��� ����
            EnemyPathFollower pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
                Debug.Log($"{enemy.name}�� ��� ���� �Ϸ�");
            }
            else
            {
                Debug.LogError($"{enemy.name}�� EnemyPathFollower�� �����ϴ�!");
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

            Debug.Log($"[���] {enemy.name} | Ȱ��: {activeEnemies.Count} | ������: {remainingEnemies}");


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

        if (OnWaveCompleted != null)
        {
            OnWaveCompleted.Invoke(currentWaveIndex + 1);
        }

        if (gameManager != null)
        {
            int reward = 50 + (currentWaveIndex * 10);
            gameManager.AddResources(reward);
            Debug.Log($"���̺� ����: {reward} �ڿ�");
        }

        if (waveProgressBar != null)
        {
            waveProgressBar.EndWave();
        }

        // ���̺� �Ϸ� �ؽ�Ʈ
        if (waveText != null)
        {
            if (currentWaveIndex + 1 < waves.Length)
            {
                // 2�� �Ŀ� ���� ���̺� �ȳ� ǥ��
                StartCoroutine(ShowNextWaveText());
            }
            else
            {
                waveText.text = "��� ���̺� �Ϸ�! �¸�!";
            }
        }
    }

    // ���̺� �Ϸ� �� ��� ��� �� �ȳ� �ؽ�Ʈ ǥ��
    private IEnumerator ShowNextWaveText()
    {
        waveText.text = $"���̺� {currentWaveIndex + 1} �Ϸ�!";
        yield return new WaitForSeconds(2f);
        waveText.text = $"G Ű�� ���� ���̺� {currentWaveIndex + 2} ����";
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
        if (waveText != null)
        {
            if (currentWaveIndex >= 0 && currentWaveIndex < waves.Length)
            {
                waveText.text = $"Wave {currentWaveIndex + 1}/{waves.Length}";
            }
            else if (currentWaveIndex < 0)
            {
                waveText.text = "G Ű�� ���� ù ���̺� ����";
            }
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

        UpdateWaveUI();

        if (waveProgressBar != null)
        {
            for (int i = 0; i < waveProgressBar.waveTextures.Length; i++)
            {
                if (waveProgressBar.waveTextures[i] != null)
                {
                    waveProgressBar.waveTextures[i].SetActive(false);
                }
            }
            waveProgressBar.scrollbar.value = 0;
        }
    }
}