using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("웨이브 설정")]
    public Wave[] waves;
    public int currentWaveIndex = -1;
    public bool isWaveActive = false;
    public bool waitingForNextWave = false;

    [Header("스폰 설정")]
    public Transform[] spawnPoints;
    public float timeBetweenSpawns = 1.5f;

    [Header("UI 참조")]
    public Text waveText;
    public WaveProgressBar waveProgressBar;

    // 참조
    private GameManager gameManager;
    private EnemyPool enemyPool;
    private PathManager pathManager;

    // 상태 관리
    private int remainingEnemies = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // 웨이브 완료 이벤트
    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveCompleted;
    private bool isPlayerReady = false;  // 플레이어 준비 상태 추가

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        EnemyPathFollower.OnAnyEnemyDeath += HandleEnemyDeath;

        currentWaveIndex = -1;

        // 초기에는 안내 텍스트 숨김
        if (waveText != null)
        {
            waveText.text = "";  // 빈 텍스트로 시작
        }

        UpdateWaveUI();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        EnemyPathFollower.OnAnyEnemyDeath -= HandleEnemyDeath;
    }

    void Update()
    {
        // 플레이어가 준비되었고, 웨이브가 활성화되지 않았고, 활성 적이 없을 때만
        if (Input.GetKeyDown(KeyCode.G) && isPlayerReady && !isWaveActive && activeEnemies.Count == 0)
        {
            StartNextWave();
        }
    }
    // 플레이어가 준비되었을 때 호출
    public void OnPlayerReady()
    {
        isPlayerReady = true;

        if (waveText != null && currentWaveIndex < 0)
        {
            waveText.text = "G 키를 눌러 첫 웨이브 시작";
        }
    }

    public void StartNextWave()
    {
        if (isWaveActive || activeEnemies.Count > 0)
        {
            Debug.Log($"웨이브 시작 불가 - 활성: {isWaveActive}, 남은 적: {activeEnemies.Count}");
            return;
        }

        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            GameWon();
            return;
        }

        Debug.Log($"웨이브 {currentWaveIndex + 1} 시작!");
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
        UpdateWaveUI();
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveActive = true;
        waitingForNextWave = false;

        Debug.Log($"웨이브 {currentWaveIndex + 1} 시작!");

        int totalEnemies = wave.wave_enemyCount;
        remainingEnemies = totalEnemies;


        if (waveProgressBar != null)
        {
            waveProgressBar.StartWave();
        }

        // 경로 확인
        List<Vector3> mainPath = pathManager != null ? pathManager.GetMainPath() : null;
        if (mainPath == null || mainPath.Count == 0)
        {
            Debug.LogWarning("경로가 설정되지 않았습니다. 기본 경로를 사용합니다.");
            mainPath = GenerateDefaultPath();
        }

        // 정확히 wave_enemyCount만큼만 스폰
        for (int i = 0; i < totalEnemies; i++)
        {
            EnemyData enemyData = GetEnemyDataFromWave(wave, i);

            if (enemyData != null)
            {
                GameObject enemy = SpawnEnemy(enemyData, mainPath);

                if (enemy == null)
                {
                    Debug.LogError($"적 생성 실패: {i + 1}/{totalEnemies}");
                    remainingEnemies--; // 생성 실패한 적은 카운트에서 제외
                }
                else
                {
                    Debug.Log($"적 생성 성공: {enemyData.enemyType} ({i + 1}/{totalEnemies})");
                }
            }
            else
            {
                Debug.LogError($"EnemyData가 null입니다! 인덱스: {i}");
                remainingEnemies--; // 데이터가 없는 적은 카운트에서 제외
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        Debug.Log($"웨이브 {currentWaveIndex + 1} 스폰 완료 - 활성 적: {activeEnemies.Count}, 남은 적: {remainingEnemies}");
    }

    private EnemyData GetEnemyDataFromWave(Wave wave, int index)
    {
        if (wave.wave_enemyData == null || wave.wave_enemyData.Length == 0)
        {
            Debug.LogError($"웨이브 {currentWaveIndex + 1}에 적 데이터가 없습니다!");
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
            Debug.LogError("스폰 포인트 또는 적 풀이 설정되지 않았습니다!");
            return null;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = enemyPool.GetEnemy(enemyData.enemyType, spawnPoint.position);

        if (enemy != null)
        {
            // 활성 적 리스트에 추가
            activeEnemies.Add(enemy);

            // 경로 설정
            EnemyPathFollower pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
                Debug.Log($"{enemy.name}에 경로 설정 완료");
            }
            else
            {
                Debug.LogError($"{enemy.name}에 EnemyPathFollower가 없습니다!");
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

            Debug.Log($"[사망] {enemy.name} | 활성: {activeEnemies.Count} | 남은수: {remainingEnemies}");


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

        Debug.Log($"웨이브 {currentWaveIndex + 1} 완료!");

        if (OnWaveCompleted != null)
        {
            OnWaveCompleted.Invoke(currentWaveIndex + 1);
        }

        if (gameManager != null)
        {
            int reward = 50 + (currentWaveIndex * 10);
            gameManager.AddResources(reward);
            Debug.Log($"웨이브 보상: {reward} 자원");
        }

        if (waveProgressBar != null)
        {
            waveProgressBar.EndWave();
        }

        // 웨이브 완료 텍스트
        if (waveText != null)
        {
            if (currentWaveIndex + 1 < waves.Length)
            {
                // 2초 후에 다음 웨이브 안내 표시
                StartCoroutine(ShowNextWaveText());
            }
            else
            {
                waveText.text = "모든 웨이브 완료! 승리!";
            }
        }
    }

    // 웨이브 완료 후 잠시 대기 후 안내 텍스트 표시
    private IEnumerator ShowNextWaveText()
    {
        waveText.text = $"웨이브 {currentWaveIndex + 1} 완료!";
        yield return new WaitForSeconds(2f);
        waveText.text = $"G 키를 눌러 웨이브 {currentWaveIndex + 2} 시작";
    }


    private void GameWon()
    {
        Debug.Log("게임 승리! 모든 웨이브를 클리어했습니다.");

        if (waveText != null)
        {
            waveText.text = "승리! 모든 웨이브 클리어";
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
                waveText.text = "G 키를 눌러 첫 웨이브 시작";
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