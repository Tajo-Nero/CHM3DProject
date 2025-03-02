using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// 게임 관리 클래스, 게임 상태를 관리하고 업데이트합니다.
/// </summary>
public class GameManager : MonoBehaviour, IObserver
{
    public static GameManager Instance { get; private set; } // 게임 매니저 인스턴스
    public NavMeshSurface navMeshSurface; // 내비게이션 메쉬
    public PauseMenuController pauseMenuController; // PauseMenuController 참조
    public int installationCost = 40; // 설치 비용
    public int shopCost = 0; // 샵 비용

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
        SpawnPlayer(carPlayerPrefab); // 차량 플레이어 스폰

        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager 인스턴스를 찾을 수 없습니다.");
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
    /// 샵 비용과 설치 비용을 확률적으로 증가시킵니다.
    /// </summary>
    /// <param name="shopIncrement">샵 비용 증가량</param>
    /// <param name="installationIncrement">설치 비용 증가량</param>
    /// <param name="shopProbability">샵 비용 증가 확률 (0.0에서 1.0 사이)</param>
    /// <param name="installationProbability">설치 비용 증가 확률 (0.0에서 1.0 사이)</param>
    public void IncreaseCosts(int shopIncrement, int installationIncrement, float shopProbability, float installationProbability)
    {
        float shopRandomValue = Random.Range(0f, 1f);
        if (shopRandomValue <= shopProbability)
        {
            shopCost += shopIncrement;
            Debug.Log($"적이 죽었습니다. 샵 비용이 {shopIncrement} 만큼 증가했습니다. 현재 샵 비용: {shopCost}");
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
            UIManager.Instance.UpdateWaveProgress(waveManager.GetCurrentWaveIndex(), waveManager.GetTotalWaves());
        }
    }
    void Update()
    {
        
    }

    public void AdvanceWave()
    {
        if (waveManager == null)
        {
            Debug.LogError("WaveManager 인스턴스가 null입니다.");
            return;
        }

        waveManager.StartNextWave(); // 다음 웨이브 시작

        int currentWave = waveManager.GetCurrentWaveIndex();
        if (currentWave == 2 || currentWave == 3 || currentWave == 7 || currentWave == 8)
        {
            IncreaseCosts(0, 3, 1.0f, 1.0f); // 샵 비용 0, 설치 비용 3 증가 (확률 100%)
        }

        UpdateUI(); // 웨이브 진행 후 UI 업데이트
    }

    public void EnemyDefeated()
    {
        IncreaseCosts(3, 2, 0.2f, 0.2f); // 샵 비용 5, 설치 비용 3증가 (확률값은 설정에 맞게 조정)
    }

    public void InstallationCostBonus(int bonusAmount)
    {
        installationCost += bonusAmount;
        Debug.Log("설치 비용 보너스가 추가되었습니다: " + bonusAmount);
    }

    public void OnWaveCompleted()
    {
        Debug.Log("웨이브 완료! 다음 웨이브를 시작하려면 'G' 키를 누르세요.");

        int currentWave = waveManager.GetCurrentWaveIndex();
        if (currentWave == 5 || currentWave == 10)
        {
            OpenShop();
        }

        UpdateUI(); // 웨이브 완료 후 UI 업데이트
    }

    private void OpenShop()
    {
        Debug.Log("상점이 열렸습니다! 필요한 물품을 구매하세요.");
    }

    public void SpawnPlayer(GameObject playerPrefab)
    {
        Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation); // 플레이어 스폰
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
    }

    public void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // 내비게이션 메쉬 빌드
            Debug.Log("NavMesh 빌드 완료");
        }
        else
        {
            Debug.LogError("NavMeshSurface가 지정되지 않았습니다.");
        }
    }

    public void OnNotify(GameObject obj, string eventMessage)
    {
        if (eventMessage == "EnemyDefeated")
        {
            EnemyDefeated();
        }
    }
}
