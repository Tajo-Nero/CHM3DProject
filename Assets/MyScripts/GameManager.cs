using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// ���� ���� Ŭ����, ���� ���¸� �����ϰ� ������Ʈ�մϴ�.
/// </summary>
public class GameManager : MonoBehaviour, IObserver
{
    public static GameManager Instance { get; private set; } // ���� �Ŵ��� �ν��Ͻ�
    public NavMeshSurface navMeshSurface; // ������̼� �޽�
    public PauseMenuController pauseMenuController; // PauseMenuController ����
    public int installationCost = 40; // ��ġ ���
    public int shopCost = 0; // �� ���

    [Header("Wave Manager")]
    public WaveManager waveManager; // ���̺� �Ŵ���

    [Header("Player Settings")]
    public GameObject playerPrefab; // �÷��̾� ������
    public GameObject carPlayerPrefab; // ���� �÷��̾� ������
    public Transform playerSpawnPoint; // �÷��̾� ���� ����

    [Header("Nexus Settings")]
    public GameObject nexusPrefab; // �ؼ��� ������
    public Transform nexusSpawnPoint; // �ؼ��� ���� ����

    [Header("Terrain Settings")]
    public TerrainManager terrainManager; // ���� �Ŵ���

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
            terrainManager.InitializeTerrain(); // ���� �ʱ�ȭ
        }      

        SpawnNexus(); // �ؼ��� ����
        InitializeGame(); // ���� �ʱ�ȭ
        UpdateUI(); // �ʱ� UI ������Ʈ
    }

    private void InitializeGame()
    {
        SpawnPlayer(carPlayerPrefab); // ���� �÷��̾� ����

        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
    }

    public bool CanAffordInstallation(int cost)
    {
        return installationCost >= cost; // ��ġ ����� ������ �� �ִ��� Ȯ��
    }

    public void DeductCost(int cost)
    {
        installationCost -= cost; // ��ġ ��� ����
    }

    /// <summary>
    /// �� ���� ��ġ ����� Ȯ�������� ������ŵ�ϴ�.
    /// </summary>
    /// <param name="shopIncrement">�� ��� ������</param>
    /// <param name="installationIncrement">��ġ ��� ������</param>
    /// <param name="shopProbability">�� ��� ���� Ȯ�� (0.0���� 1.0 ����)</param>
    /// <param name="installationProbability">��ġ ��� ���� Ȯ�� (0.0���� 1.0 ����)</param>
    public void IncreaseCosts(int shopIncrement, int installationIncrement, float shopProbability, float installationProbability)
    {
        float shopRandomValue = Random.Range(0f, 1f);
        if (shopRandomValue <= shopProbability)
        {
            shopCost += shopIncrement;
            Debug.Log($"���� �׾����ϴ�. �� ����� {shopIncrement} ��ŭ �����߽��ϴ�. ���� �� ���: {shopCost}");
        }

        float installationRandomValue = Random.Range(0f, 1f);
        if (installationRandomValue <= installationProbability)
        {
            installationCost += installationIncrement;
            Debug.Log($"���� �׾����ϴ�. ��ġ ����� {installationIncrement} ��ŭ �����߽��ϴ�. ���� ��ġ ���: {installationCost}");
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
            Debug.LogError("WaveManager �ν��Ͻ��� null�Դϴ�.");
            return;
        }

        waveManager.StartNextWave(); // ���� ���̺� ����

        int currentWave = waveManager.GetCurrentWaveIndex();
        if (currentWave == 2 || currentWave == 3 || currentWave == 7 || currentWave == 8)
        {
            IncreaseCosts(0, 3, 1.0f, 1.0f); // �� ��� 0, ��ġ ��� 3 ���� (Ȯ�� 100%)
        }

        UpdateUI(); // ���̺� ���� �� UI ������Ʈ
    }

    public void EnemyDefeated()
    {
        IncreaseCosts(3, 2, 0.2f, 0.2f); // �� ��� 5, ��ġ ��� 3���� (Ȯ������ ������ �°� ����)
    }

    public void InstallationCostBonus(int bonusAmount)
    {
        installationCost += bonusAmount;
        Debug.Log("��ġ ��� ���ʽ��� �߰��Ǿ����ϴ�: " + bonusAmount);
    }

    public void OnWaveCompleted()
    {
        Debug.Log("���̺� �Ϸ�! ���� ���̺긦 �����Ϸ��� 'G' Ű�� ��������.");

        int currentWave = waveManager.GetCurrentWaveIndex();
        if (currentWave == 5 || currentWave == 10)
        {
            OpenShop();
        }

        UpdateUI(); // ���̺� �Ϸ� �� UI ������Ʈ
    }

    private void OpenShop()
    {
        Debug.Log("������ ���Ƚ��ϴ�! �ʿ��� ��ǰ�� �����ϼ���.");
    }

    public void SpawnPlayer(GameObject playerPrefab)
    {
        Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation); // �÷��̾� ����
    }

    private void SpawnNexus()
    {
        if (nexusPrefab != null)
        {
            Instantiate(nexusPrefab, nexusSpawnPoint.position, nexusSpawnPoint.rotation); // �ؼ��� ����
        }
    }

    public void ResetTerrain()
    {
        if (terrainManager != null)
        {
            terrainManager.ResetTerrain(); // ���� �ʱ�ȭ
        }
    }

    public void BakeNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // ������̼� �޽� ����
            Debug.Log("NavMesh ���� �Ϸ�");
        }
        else
        {
            Debug.LogError("NavMeshSurface�� �������� �ʾҽ��ϴ�.");
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
