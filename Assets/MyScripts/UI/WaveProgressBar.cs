using UnityEngine;
using UnityEngine.UI;

public class WaveProgressBar : MonoBehaviour
{
    [Header("UI 요소")]
    public Scrollbar scrollbar;
    public Text waveText;
    public GameObject[] waveTextures; // 텍스처 오브젝트 배열

    [Header("웨이브 설정")]
    public int totalWaves = 12;
    private int currentWave = 0;

    [Header("진행 상황")]
    private int maxEnemiesInWave = 0;
    private int remainingEnemies = 0;

    [Header("상태 텍스트")]
    public Text statusText; // 웨이브 상태 표시 (시작 대기 중, 진행 중, 완료 등)

    // 참조
    private WaveManager waveManager;

    void Start()
    {
        // WaveManager 참조 찾기
        waveManager = FindObjectOfType<WaveManager>();

        if (waveManager != null)
        {
            // 이벤트 등록
            waveManager.OnWaveCompleted += OnWaveCompleted;

            // 총 웨이브 수 동기화
            totalWaves = waveManager.GetTotalWaves();
        }

        // 초기화
        if (scrollbar != null)
        {
            scrollbar.value = 0;
            UpdateWaveText();
        }

        // 모든 텍스처 비활성화
        foreach (var texture in waveTextures)
        {
            if (texture != null)
                texture.SetActive(false);
        }

        // 초기 상태 텍스트 설정
        UpdateStatusText("G 키를 눌러 웨이브 시작");
    }

    void Update()
    {
        // 웨이브 매니저 연동
        if (waveManager != null)
        {
            // 현재 웨이브 동기화
            int waveManagerWave = waveManager.GetCurrentWave();
            if (waveManagerWave != currentWave + 1)
            {
                currentWave = waveManagerWave - 1;
                UpdateWaveText();
                UpdateWaveTexture();
            }

            // 웨이브 진행 상황 업데이트 (적 처치 진행도)
            if (waveManager.isWaveActive && maxEnemiesInWave > 0)
            {
                float progress = waveManager.GetWaveProgress();
                UpdateProgress(progress);
            }
        }
    }

    // 웨이브 시작 시 호출
    public void OnWaveStarted()
    {
        if (currentWave < totalWaves && currentWave < waveTextures.Length)
        {
            // 현재 웨이브 텍스처 활성화
            UpdateWaveTexture();

            // 상태 텍스트 업데이트
            UpdateStatusText($"웨이브 {currentWave + 1} 진행 중");

            // 진행 상황 초기화
            if (waveManager != null && currentWave < waveManager.waves.Length)
            {
                maxEnemiesInWave = waveManager.waves[currentWave].wave_enemyCount;
                remainingEnemies = maxEnemiesInWave;
            }
        }
    }

    // 웨이브 종료 시 호출 (이벤트 콜백)
    public void OnWaveCompleted(int waveNumber)
    {
        // UI 업데이트
        currentWave = waveNumber;
        UpdateScrollbar();
        UpdateWaveText();

        // 다음 웨이브가 있다면 안내 메시지 표시
        if (currentWave < totalWaves)
        {
            UpdateStatusText($"웨이브 {currentWave} 완료! G 키를 눌러 다음 웨이브 시작");
        }
        else
        {
            UpdateStatusText("모든 웨이브 완료!");
        }

        // 현재 웨이브 텍스처 비활성화
        if (waveNumber - 1 < waveTextures.Length)
        {
            waveTextures[waveNumber - 1].SetActive(false);
        }
    }

    // 웨이브 텍스처 업데이트
    private void UpdateWaveTexture()
    {
        // 먼저 모든 텍스처 비활성화
        foreach (var texture in waveTextures)
        {
            if (texture != null)
                texture.SetActive(false);
        }

        // 현재 웨이브 텍스처 활성화
        if (currentWave < waveTextures.Length && waveTextures[currentWave] != null)
        {
            waveTextures[currentWave].SetActive(true);
        }
    }

    // 스크롤바 업데이트
    private void UpdateScrollbar()
    {
        if (scrollbar != null && totalWaves > 1)
        {
            float value = (float)currentWave / (totalWaves - 1);
            scrollbar.value = value;
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

    // 웨이브 내 진행 상황 업데이트 (적 처치율)
    public void UpdateProgress(float progress)
    {
        if (scrollbar != null)
        {
            // 현재 웨이브 내 진행 상황을 나타내는 보조 표시 가능
            // 예: 별도의 진행 바나 텍스트로 표시
        }
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

        // 진행 상황 UI 업데이트 (필요한 경우)
        if (maxEnemiesInWave > 0)
        {
            float progress = (float)value / maxEnemiesInWave;
            UpdateProgress(progress);
        }
    }

    // 웨이브 시작 - WaveManager에서 직접 호출하기 위한 함수
    public void StartWave()
    {
        // 이미 구현된 OnWaveStarted 함수 활용
        OnWaveStarted();
    }

    // 웨이브 종료 - WaveManager에서 직접 호출하기 위한 함수
    public void EndWave()
    {
        // 현재 웨이브 텍스처 비활성화
        if (currentWave < waveTextures.Length && waveTextures[currentWave] != null)
        {
            waveTextures[currentWave].SetActive(false);
        }

        // 현재 웨이브 증가
        currentWave++;

        // UI 업데이트
        UpdateScrollbar();
        UpdateWaveText();

        // 상태 텍스트 업데이트
        if (currentWave < totalWaves)
        {
            UpdateStatusText($"웨이브 {currentWave} 완료! G 키를 눌러 다음 웨이브 시작");
        }
        else
        {
            UpdateStatusText("모든 웨이브 완료!");
        }
    }

    // 리셋
    public void ResetProgressBar()
    {
        currentWave = 0;
        UpdateScrollbar();
        UpdateWaveText();
        UpdateWaveTexture();
        UpdateStatusText("G 키를 눌러 웨이브 시작");
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