using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 게임 관리 클래스, 게임 상태를 관리하고 업데이트합니다.
/// </summary>
public class GameManager : MonoBehaviour, IObserver
{
    public static GameManager Instance { get; private set; } // 게임 매니저 인스턴스
    public PauseMenuController pauseMenuController; // PauseMenuController 참조
    public int installationCost = 40; // 설치 비용
    public int shopCost = 0; // 상점 비용

    [Header("Wave Manager")]
    public WaveManager waveManager; // 웨이브 매니저

    [Header("Player Settings")]
    public GameObject playerPrefab; // 플레이어 프리팹
    public GameObject carPlayerPrefab; // 차량 플레이어 프리팹
    public Transform playerSpawnPoint; // 플레이어 스폰 지점

    [Header("Nexus Settings")]
    public GameObject nexusPrefab; // 넥서스 프리팹
    public Transform nexusSpawnPoint; // 넥서스 스폰 지점
    private int nexusHealth = 100; // 넥서스 체력
    public int maxNexusHealth = 100; // 최대 넥서스 체력

    [Header("Terrain Settings")]
    public TerrainManager terrainManager; // 지형 매니저

    [Header("UI References")]
    public Text healthText; // 체력 텍스트
    public Slider healthSlider; // 체력 슬라이더
    public Text resourceText; // 자원 텍스트
    public GameObject gameOverPanel; // 게임 오버 패널
    public GameObject victoryPanel; // 승리 패널

    // 게임 상태 관리
    private bool isCarModeActive = true; // 차량 모드가 활성화되어 있는지
    private bool pathGenerated = false; // 경로가 생성되었는지
    private bool isGameOver = false; // 게임 오버 상태인지
    private bool isGameWon = false; // 게임 승리 상태인지

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (terrainManager != null)
        {
            terrainManager.InitializeTerrain(); // 지형 초기화
        }

        SpawnNexus(); // 넥서스 스폰
        InitializeGame(); // 게임 초기화
        UpdateUI(); // 초기 UI 업데이트
    }

    private void InitializeGame()
    {
        // 처음에는 차량 모드로 시작
        SpawnCarPlayer();

        // 웨이브 매니저 찾기
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager 인스턴스를 찾을 수 없습니다.");
        }
        

        // 체력 초기화
        nexusHealth = maxNexusHealth;
        UpdateHealthUI();

        // 게임 오버/승리 패널 비활성화
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

    }

    // 차량 플레이어 스폰
    private void SpawnCarPlayer()
    {
        if (carPlayerPrefab != null && isCarModeActive)
        {
            Instantiate(carPlayerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            Debug.Log("차량 플레이어가 스폰되었습니다.");
        }
    }

    // 일반 플레이어 스폰 (넥서스 도달 후)
    public void SpawnPlayer(GameObject playerPrefab)
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            Debug.Log("일반 플레이어가 스폰되었습니다.");

            // 차량 모드 비활성화
            isCarModeActive = false;

            // 경로 생성됨으로 설정
            pathGenerated = true;

        }
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnPlayerReady();
        }
    }

    // 차량 모드에서 플레이어 모드로 전환
    public void SwitchToPlayerMode(GameObject playerModePrefab)
    {
        if (isCarModeActive && playerModePrefab != null)
        {
            isCarModeActive = false;
            pathGenerated = true;

            SpawnPlayer(playerModePrefab);
            Debug.Log("차량 모드에서 플레이어 모드로 전환되었습니다.");

        }
    }

    // 설치 비용 확인
    public bool CanAffordInstallation(int cost)
    {
        return installationCost >= cost; // 설치 비용을 감당할 수 있는지 확인
    }

    // 비용 차감
    public void DeductCost(int cost)
    {
        installationCost -= cost; // 설치 비용 차감
        UpdateResourceUI();
    }

    // 자원 얻기
    public int GetResources()
    {
        return installationCost;
    }

    // 자원 추가
    public void AddResources(int amount)
    {
        installationCost += amount;
        UpdateResourceUI();
        Debug.Log($"자원 {amount}이 추가되었습니다. 현재 자원: {installationCost}");
    }

    // 자원 사용
    public bool UseResources(int amount)
    {
        if (installationCost >= amount)
        {
            installationCost -= amount;
            UpdateResourceUI();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 상점 비용과 설치 비용을 확률적으로 증가시킵니다.
    /// </summary>
    /// <param name="shopIncrement">상점 비용 증가량</param>
    /// <param name="installationIncrement">설치 비용 증가량</param>
    /// <param name="shopProbability">상점 비용 증가 확률 (0.0에서 1.0 사이)</param>
    /// <param name="installationProbability">설치 비용 증가 확률 (0.0에서 1.0 사이)</param>
    public void IncreaseCosts(int shopIncrement, int installationIncrement, float shopProbability, float installationProbability)
    {
        float shopRandomValue = Random.Range(0f, 1f);
        if (shopRandomValue <= shopProbability)
        {
            shopCost += shopIncrement;
            Debug.Log($"적이 죽었습니다. 상점 비용이 {shopIncrement} 만큼 증가했습니다. 현재 상점 비용: {shopCost}");
        }

        float installationRandomValue = Random.Range(0f, 1f);
        if (installationRandomValue <= installationProbability)
        {
            installationCost += installationIncrement;
            Debug.Log($"적이 죽었습니다. 설치 비용이 {installationIncrement} 만큼 증가했습니다. 현재 설치 비용: {installationCost}");
        }

        UpdateUI();
    }

    // UI 업데이트
    public void UpdateUI()
    {
        UpdateResourceUI();
        UpdateHealthUI();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateShopCost(shopCost);
            UIManager.Instance.UpdateInstallationCost(installationCost);
            if (waveManager != null)
            {
                UIManager.Instance.UpdateWaveProgress(waveManager.GetCurrentWave(), waveManager.GetTotalWaves());
            }
        }
    }

    // 자원 UI 업데이트
    private void UpdateResourceUI()
    {
        if (resourceText != null)
        {
            resourceText.text = installationCost.ToString();
        }
    }

    // 체력 UI 업데이트
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxNexusHealth;
            healthSlider.value = nexusHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{nexusHealth}/{maxNexusHealth}";
        }
    }

    void Update()
    {
        // 게임 오버 상태에서는 입력 무시
        if (isGameOver || isGameWon)
            return;

        // 경로가 생성되었고 플레이어 모드인 경우에만 웨이브 진행 가능
        if (Input.GetKeyDown(KeyCode.G) && pathGenerated && !isCarModeActive)
        {
            AdvanceWave();
        }
    }

    // 웨이브 진행
    public void AdvanceWave()
    {
        // 경로가 생성되지 않았으면 웨이브 시작 불가
        if (!pathGenerated)
        {
            Debug.LogWarning("경로가 생성되지 않았습니다! 먼저 차량으로 길을 만드세요.");
            return;
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager 인스턴스가 null입니다.");
            return;
        }


        // 다음 웨이브 시작
        waveManager.StartNextWave();

        // 특정 웨이브에서 비용 증가
        int currentWave = waveManager.GetCurrentWave();
        if (currentWave == 2 || currentWave == 3 || currentWave == 7 || currentWave == 8)
        {
            IncreaseCosts(0, 3, 1.0f, 1.0f); // 상점 비용 0, 설치 비용 3 증가 (확률 100%)
        }

        UpdateUI(); // 웨이브 진행 후 UI 업데이트
    }

    // 적 처치 시 보상
    public void EnemyDefeated()
    {
        IncreaseCosts(3, 2, 0.2f, 0.2f); // 상점 비용 3, 설치 비용 2 증가 (확률값은 설정에 맞게 조정)
    }

    // 설치 비용 보너스
    public void InstallationCostBonus(int bonusAmount)
    {
        installationCost += bonusAmount;
        Debug.Log("설치 비용 보너스가 추가되었습니다: " + bonusAmount);
        UpdateUI();
    }



    // 상점 열기
    private void OpenShop()
    {
        Debug.Log("상점이 열렸습니다! 필요한 물건을 구매하세요.");
        // 상점 UI 표시 로직 추가
    }

    // 넥서스 스폰
    private void SpawnNexus()
    {
        if (nexusPrefab != null)
        {
            Instantiate(nexusPrefab, nexusSpawnPoint.position, nexusSpawnPoint.rotation); // 넥서스 스폰
        }
    }

    // 지형 리셋
    public void ResetTerrain()
    {
        if (terrainManager != null)
        {
            terrainManager.ResetTerrain(); // 지형 초기화
        }

        // 게임 상태 리셋
        isCarModeActive = true;
        pathGenerated = false;
    }

    // 넥서스 체력 관리
    public int GetHealth()
    {
        return nexusHealth;
    }

    // 체력 설정
    public void SetHealth(int newHealth)
    {
        nexusHealth = Mathf.Clamp(newHealth, 0, maxNexusHealth);
        UpdateHealthUI();

        // 체력이 0 이하면 게임 오버
        if (nexusHealth <= 0)
        {
            GameOver();
        }
    }

    // 넥서스 데미지 처리
    public void TakeDamage(float damage)
    {
        nexusHealth -= Mathf.RoundToInt(damage);
        nexusHealth = Mathf.Max(0, nexusHealth);

        UpdateHealthUI();

        Debug.Log($"넥서스가 {damage} 데미지를 받았습니다. 남은 체력: {nexusHealth}");

        // 체력이 0 이하면 게임 오버
        if (nexusHealth <= 0)
        {
            GameOver();
        }
    }

    // 게임 오버 처리
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("게임 오버! 넥서스가 파괴되었습니다.");

        // 게임 오버 UI 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 시간 느리게 설정 (완전히 멈추지 않고)
        Time.timeScale = 0.5f;

        // 게임 오버 효과 추가 (선택 사항)
        StartCoroutine(GameOverEffects());
    }

    // 게임 오버 효과 코루틴
    private IEnumerator GameOverEffects()
    {
        // 게임 오버 효과 (필요시 추가)
        yield return new WaitForSeconds(2f);

        // 시간 정상화
        Time.timeScale = 1f;
    }

    // 게임 승리 처리
    public void GameWon()
    {
        if (isGameWon) return;

        isGameWon = true;
        Debug.Log("게임 승리! 모든 웨이브를 클리어했습니다.");

        // 승리 UI 표시
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // 승리 효과 (선택 사항)
        StartCoroutine(VictoryEffects());
    }

    // 승리 효과 코루틴
    private IEnumerator VictoryEffects()
    {
        // 승리 효과 (필요시 추가)
        yield return new WaitForSeconds(2f);
    }

    // IObserver 인터페이스 구현
    public void OnNotify(GameObject obj, string eventMessage)
    {
        if (eventMessage == "EnemyDefeated")
        {
            EnemyDefeated();
        }
    }

    // 게임 상태 확인 메서드들
    public bool IsCarModeActive()
    {
        return isCarModeActive;
    }

    public bool IsPathGenerated()
    {
        return pathGenerated;
    }

    // 경로 생성 완료 알림
    public void OnPathGenerated()
    {
        pathGenerated = true;
        Debug.Log("경로 생성이 완료되었습니다! 이제 웨이브를 시작할 수 있습니다.");

    }
}