using UnityEngine;

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

    [Header("Terrain Settings")]
    public TerrainManager terrainManager; // 지형 매니저

    // 게임 상태 관리
    private bool isCarModeActive = true; // 차량 모드가 활성화되어 있는지
    private bool pathGenerated = false; // 경로가 생성되었는지

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

        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager 인스턴스를 찾을 수 없습니다.");
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
        if (playerPrefab != null && !isCarModeActive)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            Debug.Log("일반 플레이어가 스폰되었습니다.");
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

            // 웨이브 시작 가능 상태로 변경
            if (waveManager != null)
            {
                // waveManager.EnableWaveStart(); // WaveManager에 이 메서드가 있다면
            }
        }
    }

    public bool CanAffordInstallation(int cost)
    {
        return installationCost >= cost; // 설치 비용을 감당할 수 있는지 확인
    }

    public void DeductCost(int cost)
    {
        installationCost -= cost; // 설치 비용 차감
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

    public void UpdateUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateShopCost(shopCost);
            UIManager.Instance.UpdateInstallationCost(installationCost);
            if (waveManager != null)
            {
                UIManager.Instance.UpdateWaveProgress(waveManager.GetCurrentWaveIndex(), waveManager.GetTotalWaves());
            }
        }
    }

    void Update()
    {
        // 경로가 생성되었고 플레이어 모드인 경우에만 웨이브 진행 가능
        if (Input.GetKeyDown(KeyCode.G) && pathGenerated && !isCarModeActive)
        {
            AdvanceWave();
        }
    }

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

        waveManager.StartNextWave(); // 다음 웨이브 시작

        int currentWave = waveManager.GetCurrentWaveIndex();
        if (currentWave == 2 || currentWave == 3 || currentWave == 7 || currentWave == 8)
        {
            IncreaseCosts(0, 3, 1.0f, 1.0f); // 상점 비용 0, 설치 비용 3 증가 (확률 100%)
        }

        UpdateUI(); // 웨이브 진행 후 UI 업데이트
    }

    public void EnemyDefeated()
    {
        IncreaseCosts(3, 2, 0.2f, 0.2f); // 상점 비용 3, 설치 비용 2 증가 (확률값은 설정에 맞게 조정)
    }

    public void InstallationCostBonus(int bonusAmount)
    {
        installationCost += bonusAmount;
        Debug.Log("설치 비용 보너스가 추가되었습니다: " + bonusAmount);
        UpdateUI();
    }

    public void OnWaveCompleted()
    {
        Debug.Log("웨이브 완료! 다음 웨이브를 시작하려면 'G' 키를 눌러주세요.");

        int currentWave = waveManager.GetCurrentWaveIndex();
        if (currentWave == 5 || currentWave == 10)
        {
            OpenShop();
        }

        UpdateUI(); // 웨이브 완료 후 UI 업데이트
    }

    private void OpenShop()
    {
        Debug.Log("상점이 열렸습니다! 필요한 물건을 구매하세요.");
    }

    private void SpawnNexus()
    {
        if (nexusPrefab != null)
        {
            Instantiate(nexusPrefab, nexusSpawnPoint.position, nexusSpawnPoint.rotation); // 넥서스 스폰
        }
    }

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