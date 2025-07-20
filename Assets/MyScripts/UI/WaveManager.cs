using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("���̺� ����")]
    public Wave[] waves;
    public int currentWaveIndex = -1; // -1�� ���� �������� ������ �ǹ�
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
    private PathManager pathManager; // ��� ������ ����

    // ���� ����
    private int remainingEnemies = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // ���̺� �Ϸ� �̺�Ʈ
    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveCompleted;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        UpdateWaveUI();

        // ù ���̺� �غ� �޽��� ǥ��
        if (waveText != null)
        {
            waveText.text = "G Ű�� ���� ù ���̺� ����";
        }
    }

    void Update()
    {
        // G Ű�� ���� ���̺� ����
        if (Input.GetKeyDown(KeyCode.G) && !isWaveActive && !waitingForNextWave)
        {
            StartNextWave();
        }

        // ���� ���̺� ���� ��Ȳ Ȯ�� (��� ���� óġ�Ǿ�����)
        if (isWaveActive)
        {
            CheckRemainingEnemies();
        }
    }

    // ���� ���̺� ����
    public void StartNextWave()
    {
        if (isWaveActive || waitingForNextWave)
        {
            Debug.Log("���̺갡 �̹� ���� ���Դϴ�!");
            return;
        }

        currentWaveIndex++;

        // ��� ���̺갡 ����Ǿ����� Ȯ��
        if (currentWaveIndex >= waves.Length)
        {
            GameWon();
            return;
        }

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));

        UpdateWaveUI();
    }

    // ���� ���̺� ����
    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveActive = true;
        waitingForNextWave = false;

        // ���̺� ���� �˸�
        Debug.Log($"���̺� {currentWaveIndex + 1} ����!");

        // �� �� �� ���
        int totalEnemies = wave.wave_enemyCount;
        remainingEnemies = totalEnemies;

        // ���̺� UI ������Ʈ
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWaveIndex + 1}/{waves.Length}";
        }

        // ���̺� ���α׷��� �� ������Ʈ
        if (waveProgressBar != null)
        {
            waveProgressBar.StartWave();
        }

        // ��ΰ� �����Ǿ� �ִ��� Ȯ��
        List<Vector3> mainPath = pathManager != null ? pathManager.GetMainPath() : null;

        if (mainPath == null || mainPath.Count == 0)
        {
            Debug.LogWarning("��ΰ� �������� �ʾҽ��ϴ�. �⺻ ��θ� ����մϴ�.");
            mainPath = GenerateDefaultPath();
        }

        // �� ����
        for (int i = 0; i < totalEnemies; i++)
        {
            // �ش� ���̺��� �� ������ ��������
            EnemyData enemyData = GetEnemyDataFromWave(wave, i);

            if (enemyData != null)
            {
                // �� ����
                SpawnEnemy(enemyData, mainPath);

                // ���� ���� ���
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        // ��� �� ���� �Ϸ�
        Debug.Log($"���̺� {currentWaveIndex + 1} ���� �Ϸ� - ���� ��: {remainingEnemies}");
    }

    // ���̺꿡�� �� ������ ��������
    private EnemyData GetEnemyDataFromWave(Wave wave, int index)
    {
        if (wave.wave_enemyData == null || wave.wave_enemyData.Length == 0)
            return null;

        // ���� ���� �Ǵ� �ݺ� ����
        if (wave.wave_enemyData.Length == 1)
            return wave.wave_enemyData[0];

        // ���� ���� ���� (��ȯ �Ǵ� ����)
        bool useRandomPattern = false; // ���� ���� ��� ���� (���̺� ������ ���� ���� ����)

        if (useRandomPattern)
        {
            int randomIndex = Random.Range(0, wave.wave_enemyData.Length);
            return wave.wave_enemyData[randomIndex];
        }
        else
        {
            // ��ȯ ����
            int dataIndex = index % wave.wave_enemyData.Length;
            return wave.wave_enemyData[dataIndex];
        }
    }

    // �⺻ ��� ���� (��ΰ� �������� �ʾ��� �� ���)
    private List<Vector3> GenerateDefaultPath()
    {
        List<Vector3> defaultPath = new List<Vector3>();

        // ���� ����Ʈ���� �ؼ��������� ���� ���
        if (spawnPoints.Length > 0)
        {
            defaultPath.Add(spawnPoints[0].position);

            // �ؼ��� ��ġ ã��
            Nexus nexus = FindObjectOfType<Nexus>();
            if (nexus != null)
            {
                defaultPath.Add(nexus.transform.position);
            }
            else
            {
                // �ؼ����� ���ٸ� �߾����� ���ϴ� ���
                defaultPath.Add(new Vector3(0, 0, 0));
            }
        }

        return defaultPath;
    }

    // �� ����
    // WaveManager.cs�� SpawnEnemy �޼��� ����
    private GameObject SpawnEnemy(EnemyData enemyData, List<Vector3> path)
    {
        if (spawnPoints.Length == 0 || enemyPool == null)
        {
            Debug.LogError("���� ����Ʈ �Ǵ� �� Ǯ�� �������� �ʾҽ��ϴ�!");
            return null;
        }

        // ���� ���� ����Ʈ ����
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // ����ȭ�� �ڵ� - Ÿ������ ���� ��������
        GameObject enemy = enemyPool.GetEnemy(enemyData.enemyType, spawnPoint.position);

        if (enemy != null)
        {
            // ��� ����
            EnemyPathFollower pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
            }
        }

        return enemy;
    }

    // �� ��� ó�� (MyHealthBar�� OnEnemyDeath �̺�Ʈ�� ����)
    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            remainingEnemies--;

            // ���̺� ����� ������Ʈ
            UpdateWaveProgress();
        }
    }

    // ���� �� Ȯ��
    private void CheckRemainingEnemies()
    {
        // Ȱ�� �� ��� ���� (������ �� ����)
        activeEnemies.RemoveAll(enemy => enemy == null);

        // ���� ���� �� �� Ȯ��
        remainingEnemies = activeEnemies.Count;

        // ��� ���� ���ŵǾ����� Ȯ��
        if (remainingEnemies <= 0 && isWaveActive)
        {
            WaveCompleted();
        }
    }

    // ���̺� ����� ������Ʈ
    private void UpdateWaveProgress()
    {
        if (waveProgressBar != null && currentWaveIndex < waves.Length)
        {
            int totalEnemies = waves[currentWaveIndex].wave_enemyCount;
            int killedEnemies = totalEnemies - remainingEnemies;

            float progress = (float)killedEnemies / totalEnemies;
            // ���α׷��� �� ������Ʈ (���� ����)
        }
    }

    // ���̺� �Ϸ� ó��
    private void WaveCompleted()
    {
        isWaveActive = false;
        waitingForNextWave = true;

        Debug.Log($"���̺� {currentWaveIndex + 1} �Ϸ�!");

        // ���̺� �Ϸ� �̺�Ʈ �߻�
        if (OnWaveCompleted != null)
        {
            OnWaveCompleted.Invoke(currentWaveIndex + 1);
        }

        // ���� ����
        if (gameManager != null)
        {
            int reward = 50 + (currentWaveIndex * 10); // �⺻ ���� + ���̺꺰 �߰� ����
            gameManager.AddResources(reward);

            Debug.Log($"���̺� ����: {reward} �ڿ�");
        }

        // ���̺� �ؽ�ó ��Ȱ��ȭ
        if (waveProgressBar != null)
        {
            waveProgressBar.EndWave();
        }

        // ���� ���̺� �ȳ� �޽���
        if (waveText != null)
        {
            if (currentWaveIndex + 1 < waves.Length)
            {
                waveText.text = $"Wave {currentWaveIndex + 1} �Ϸ�! G Ű�� ���� ���� ���̺� ����";
            }
            else
            {
                waveText.text = "��� ���̺� �Ϸ�!";
            }
        }
    }

    // ���� �¸�
    private void GameWon()
    {
        Debug.Log("���� �¸�! ��� ���̺긦 Ŭ�����߽��ϴ�.");

        if (waveText != null)
        {
            waveText.text = "�¸�! ��� ���̺� Ŭ����";
        }

        // ���� �¸� ó�� (GameManager�� ����)
        if (gameManager != null)
        {
            gameManager.GameWon();
        }
    }

    // ���̺� UI ������Ʈ
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

    // ���� ���̺� �ε��� ��ȯ (�ܺ� ����)
    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }

    // �� ���̺� �� ��ȯ
    public int GetTotalWaves()
    {
        return waves.Length;
    }

    // ���̺� ���൵ ��ȯ (0~1)
    public float GetWaveProgress()
    {
        if (currentWaveIndex < 0 || currentWaveIndex >= waves.Length)
            return 0;

        int totalEnemies = waves[currentWaveIndex].wave_enemyCount;
        if (totalEnemies <= 0)
            return 1;

        return (float)(totalEnemies - remainingEnemies) / totalEnemies;
    }

    // ��� �� ���� (�����/�׽�Ʈ��)
    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies.ToArray())
        {
            if (enemy != null)
            {
                // �̺�Ʈ ���� ����
                MyHealthBar healthBar = enemy.GetComponentInChildren<MyHealthBar>();
                if (healthBar != null)
                {
                    healthBar.OnEnemyDeath -= HandleEnemyDeath;
                }

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

    // ���̺� ���� (����ۿ�)
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
            // ���α׷��� �� ����
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