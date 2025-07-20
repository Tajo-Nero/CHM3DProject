using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [Header("웨이브 설정")]
    public Wave[] waves;
    public int currentWaveIndex = -1; // -1은 아직 시작하지 않음을 의미
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
    private PathManager pathManager; // 경로 관리자 참조

    // 상태 관리
    private int remainingEnemies = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    // 웨이브 완료 이벤트
    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveCompleted;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        UpdateWaveUI();

        // 첫 웨이브 준비 메시지 표시
        if (waveText != null)
        {
            waveText.text = "G 키를 눌러 첫 웨이브 시작";
        }
    }

    void Update()
    {
        // G 키를 눌러 웨이브 시작
        if (Input.GetKeyDown(KeyCode.G) && !isWaveActive && !waitingForNextWave)
        {
            StartNextWave();
        }

        // 현재 웨이브 진행 상황 확인 (모든 적이 처치되었는지)
        if (isWaveActive)
        {
            CheckRemainingEnemies();
        }
    }

    // 다음 웨이브 시작
    public void StartNextWave()
    {
        if (isWaveActive || waitingForNextWave)
        {
            Debug.Log("웨이브가 이미 진행 중입니다!");
            return;
        }

        currentWaveIndex++;

        // 모든 웨이브가 종료되었는지 확인
        if (currentWaveIndex >= waves.Length)
        {
            GameWon();
            return;
        }

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));

        UpdateWaveUI();
    }

    // 현재 웨이브 스폰
    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveActive = true;
        waitingForNextWave = false;

        // 웨이브 시작 알림
        Debug.Log($"웨이브 {currentWaveIndex + 1} 시작!");

        // 총 적 수 계산
        int totalEnemies = wave.wave_enemyCount;
        remainingEnemies = totalEnemies;

        // 웨이브 UI 업데이트
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWaveIndex + 1}/{waves.Length}";
        }

        // 웨이브 프로그레스 바 업데이트
        if (waveProgressBar != null)
        {
            waveProgressBar.StartWave();
        }

        // 경로가 설정되어 있는지 확인
        List<Vector3> mainPath = pathManager != null ? pathManager.GetMainPath() : null;

        if (mainPath == null || mainPath.Count == 0)
        {
            Debug.LogWarning("경로가 설정되지 않았습니다. 기본 경로를 사용합니다.");
            mainPath = GenerateDefaultPath();
        }

        // 적 스폰
        for (int i = 0; i < totalEnemies; i++)
        {
            // 해당 웨이브의 적 데이터 가져오기
            EnemyData enemyData = GetEnemyDataFromWave(wave, i);

            if (enemyData != null)
            {
                // 적 스폰
                SpawnEnemy(enemyData, mainPath);

                // 스폰 간격 대기
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        // 모든 적 스폰 완료
        Debug.Log($"웨이브 {currentWaveIndex + 1} 스폰 완료 - 남은 적: {remainingEnemies}");
    }

    // 웨이브에서 적 데이터 가져오기
    private EnemyData GetEnemyDataFromWave(Wave wave, int index)
    {
        if (wave.wave_enemyData == null || wave.wave_enemyData.Length == 0)
            return null;

        // 단일 유형 또는 반복 패턴
        if (wave.wave_enemyData.Length == 1)
            return wave.wave_enemyData[0];

        // 여러 유형 패턴 (순환 또는 랜덤)
        bool useRandomPattern = false; // 랜덤 패턴 사용 여부 (웨이브 설정에 따라 변경 가능)

        if (useRandomPattern)
        {
            int randomIndex = Random.Range(0, wave.wave_enemyData.Length);
            return wave.wave_enemyData[randomIndex];
        }
        else
        {
            // 순환 패턴
            int dataIndex = index % wave.wave_enemyData.Length;
            return wave.wave_enemyData[dataIndex];
        }
    }

    // 기본 경로 생성 (경로가 설정되지 않았을 때 사용)
    private List<Vector3> GenerateDefaultPath()
    {
        List<Vector3> defaultPath = new List<Vector3>();

        // 스폰 포인트에서 넥서스까지의 직선 경로
        if (spawnPoints.Length > 0)
        {
            defaultPath.Add(spawnPoints[0].position);

            // 넥서스 위치 찾기
            Nexus nexus = FindObjectOfType<Nexus>();
            if (nexus != null)
            {
                defaultPath.Add(nexus.transform.position);
            }
            else
            {
                // 넥서스가 없다면 중앙으로 향하는 경로
                defaultPath.Add(new Vector3(0, 0, 0));
            }
        }

        return defaultPath;
    }

    // 적 스폰
    // WaveManager.cs의 SpawnEnemy 메서드 개선
    private GameObject SpawnEnemy(EnemyData enemyData, List<Vector3> path)
    {
        if (spawnPoints.Length == 0 || enemyPool == null)
        {
            Debug.LogError("스폰 포인트 또는 적 풀이 설정되지 않았습니다!");
            return null;
        }

        // 랜덤 스폰 포인트 선택
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // 간소화된 코드 - 타입으로 직접 가져오기
        GameObject enemy = enemyPool.GetEnemy(enemyData.enemyType, spawnPoint.position);

        if (enemy != null)
        {
            // 경로 설정
            EnemyPathFollower pathFollower = enemy.GetComponent<EnemyPathFollower>();
            if (pathFollower != null)
            {
                pathFollower.SetPath(path);
            }
        }

        return enemy;
    }

    // 적 사망 처리 (MyHealthBar의 OnEnemyDeath 이벤트에 연결)
    private void HandleEnemyDeath(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            remainingEnemies--;

            // 웨이브 진행률 업데이트
            UpdateWaveProgress();
        }
    }

    // 남은 적 확인
    private void CheckRemainingEnemies()
    {
        // 활성 적 목록 정리 (없어진 적 제거)
        activeEnemies.RemoveAll(enemy => enemy == null);

        // 실제 남은 적 수 확인
        remainingEnemies = activeEnemies.Count;

        // 모든 적이 제거되었는지 확인
        if (remainingEnemies <= 0 && isWaveActive)
        {
            WaveCompleted();
        }
    }

    // 웨이브 진행률 업데이트
    private void UpdateWaveProgress()
    {
        if (waveProgressBar != null && currentWaveIndex < waves.Length)
        {
            int totalEnemies = waves[currentWaveIndex].wave_enemyCount;
            int killedEnemies = totalEnemies - remainingEnemies;

            float progress = (float)killedEnemies / totalEnemies;
            // 프로그레스 바 업데이트 (선택 사항)
        }
    }

    // 웨이브 완료 처리
    private void WaveCompleted()
    {
        isWaveActive = false;
        waitingForNextWave = true;

        Debug.Log($"웨이브 {currentWaveIndex + 1} 완료!");

        // 웨이브 완료 이벤트 발생
        if (OnWaveCompleted != null)
        {
            OnWaveCompleted.Invoke(currentWaveIndex + 1);
        }

        // 보상 지급
        if (gameManager != null)
        {
            int reward = 50 + (currentWaveIndex * 10); // 기본 보상 + 웨이브별 추가 보상
            gameManager.AddResources(reward);

            Debug.Log($"웨이브 보상: {reward} 자원");
        }

        // 웨이브 텍스처 비활성화
        if (waveProgressBar != null)
        {
            waveProgressBar.EndWave();
        }

        // 다음 웨이브 안내 메시지
        if (waveText != null)
        {
            if (currentWaveIndex + 1 < waves.Length)
            {
                waveText.text = $"Wave {currentWaveIndex + 1} 완료! G 키를 눌러 다음 웨이브 시작";
            }
            else
            {
                waveText.text = "모든 웨이브 완료!";
            }
        }
    }

    // 게임 승리
    private void GameWon()
    {
        Debug.Log("게임 승리! 모든 웨이브를 클리어했습니다.");

        if (waveText != null)
        {
            waveText.text = "승리! 모든 웨이브 클리어";
        }

        // 게임 승리 처리 (GameManager에 위임)
        if (gameManager != null)
        {
            gameManager.GameWon();
        }
    }

    // 웨이브 UI 업데이트
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

    // 현재 웨이브 인덱스 반환 (외부 사용용)
    public int GetCurrentWave()
    {
        return currentWaveIndex + 1;
    }

    // 총 웨이브 수 반환
    public int GetTotalWaves()
    {
        return waves.Length;
    }

    // 웨이브 진행도 반환 (0~1)
    public float GetWaveProgress()
    {
        if (currentWaveIndex < 0 || currentWaveIndex >= waves.Length)
            return 0;

        int totalEnemies = waves[currentWaveIndex].wave_enemyCount;
        if (totalEnemies <= 0)
            return 1;

        return (float)(totalEnemies - remainingEnemies) / totalEnemies;
    }

    // 모든 적 제거 (디버그/테스트용)
    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies.ToArray())
        {
            if (enemy != null)
            {
                // 이벤트 구독 해제
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

    // 웨이브 리셋 (재시작용)
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
            // 프로그레스 바 리셋
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