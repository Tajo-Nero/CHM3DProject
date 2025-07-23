using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveProgressBar : MonoBehaviour
{
    [Header("UI 요소")]
    public Text waveText; // 현재 웨이브 진행 상황 (1/12)
    public Text statusText; // 웨이브 상태 표시 (시작 대기 중, 진행 중, 완료 등)

    [Header("웨이브 설정")]
    public int totalWaves = 12;
    private int currentWave = 0;

    [Header("진행 상황")]
    private int maxEnemiesInWave = 0;
    private int remainingEnemies = 0;

    // 참조
    private WaveManager waveManager;

    void Start()
    {
        // WaveManager 참조 찾기
        StartCoroutine(InitializeAfterFrame());
    }

    IEnumerator InitializeAfterFrame()
    {
        // 한 프레임 대기
        yield return null;

        waveManager = FindObjectOfType<WaveManager>();

        if (waveManager != null)
        {
            // 이벤트 등록
            waveManager.OnWaveCompleted += OnWaveCompleted;

            // 총 웨이브 수 동기화
            totalWaves = waveManager.GetTotalWaves();
        }

        // 초기화
        UpdateWaveText();
        UpdateStatusText("");
    }

    void Update()
    {
        if (waveManager != null)
        {
            // 현재 웨이브 동기화
            int waveManagerWave = waveManager.GetCurrentWave();

            if (waveManagerWave > 0 && waveManagerWave - 1 != currentWave)
            {
                currentWave = waveManagerWave - 1;

                if (currentWave >= 0 && currentWave < totalWaves)
                {
                    UpdateWaveText();
                }
            }

            // 웨이브 진행 상황 업데이트
            if (waveManager.isWaveActive && maxEnemiesInWave > 0)
            {
                float progress = waveManager.GetWaveProgress();
                UpdateProgress(progress);
            }
        }
    }

    // 플레이어가 준비되었을 때 호출
    public void OnPlayerReady()
    {
        UpdateStatusText("G 키를 눌러 웨이브 시작");
    }

    // 웨이브 시작 시 호출
    public void OnWaveStarted()
    {
        if (currentWave < totalWaves)
        {
            // 상태 텍스트 숨김
            UpdateStatusText("");

            // 진행 상황 초기화
            if (waveManager != null && currentWave < waveManager.waves.Length)
            {
                maxEnemiesInWave = waveManager.waves[currentWave].wave_enemyCount;
                remainingEnemies = maxEnemiesInWave;
            }
        }
    }

    public void OnWaveCompleted(int waveNumber)
    {
        // waveNumber는 1부터 시작하므로 -1
        currentWave = waveNumber - 1;

        if (currentWave >= 0 && currentWave < totalWaves)
        {
            UpdateWaveText();

            // 다음 웨이브가 있다면 시작 안내
            if (currentWave + 1 < totalWaves)
            {
                UpdateStatusText("G 키를 눌러 다음 웨이브 시작");
            }
            else
            {
                UpdateStatusText("모든 웨이브 완료!");
            }
        }
    }

    // 웨이브 텍스트 업데이트
    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave {currentWave + 1}/{totalWaves}";
        }
    }

    // 상태 텍스트 업데이트
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    // 웨이브 내 진행 상황 업데이트
    public void UpdateProgress(float progress)
    {
        // 필요한 경우 진행 상황 표시 추가 가능
    }

    // 최대 적 수 설정
    public void SetMaxValue(int maxValue)
    {
        maxEnemiesInWave = maxValue;
    }

    // 현재 처치한 적 수 설정
    public void SetValue(int value)
    {
        remainingEnemies = maxEnemiesInWave - value;

        if (maxEnemiesInWave > 0)
        {
            float progress = (float)value / maxEnemiesInWave;
            UpdateProgress(progress);
        }
    }

    public void StartWave()
    {
        if (currentWave >= 0 && currentWave < totalWaves)
        {
            OnWaveStarted();
        }
    }

    // 웨이브 종료
    public void EndWave()
    {
        // 현재는 특별한 처리 없음
    }

    // 리셋
    public void ResetProgressBar()
    {
        currentWave = 0;
        UpdateWaveText();
        UpdateStatusText("");
    }

    // 정리
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (waveManager != null)
        {
            waveManager.OnWaveCompleted -= OnWaveCompleted;
        }
    }
}