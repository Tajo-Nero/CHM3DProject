using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// ���� ���� Ŭ����, ���� ���¸� �����ϰ� ������Ʈ�մϴ�.
/// </summary>
public class GameManager : MonoBehaviour, IObserver
{
    public static GameManager Instance { get; private set; } // ���� �Ŵ��� �ν��Ͻ�
    public PauseMenuController pauseMenuController; // PauseMenuController ����
    public int installationCost = 40; // ��ġ ���
    public int shopCost = 0; // ���� ���

    [Header("Wave Manager")]
    public WaveManager waveManager; // ���̺� �Ŵ���

    [Header("Player Settings")]
    public GameObject playerPrefab; // �÷��̾� ������
    public GameObject carPlayerPrefab; // ���� �÷��̾� ������
    public Transform playerSpawnPoint; // �÷��̾� ���� ����

    [Header("Nexus Settings")]
    public GameObject nexusPrefab; // �ؼ��� ������
    public Transform nexusSpawnPoint; // �ؼ��� ���� ����
    private int nexusHealth = 100; // �ؼ��� ü��
    public int maxNexusHealth = 100; // �ִ� �ؼ��� ü��

    [Header("Terrain Settings")]
    public TerrainManager terrainManager; // ���� �Ŵ���

    [Header("UI References")]
    public Text healthText; // ü�� �ؽ�Ʈ
    public Slider healthSlider; // ü�� �����̴�
    public Text resourceText; // �ڿ� �ؽ�Ʈ
    public GameObject gameOverPanel; // ���� ���� �г�
    public GameObject victoryPanel; // �¸� �г�

    // ���� ���� ����
    private bool isCarModeActive = true; // ���� ��尡 Ȱ��ȭ�Ǿ� �ִ���
    private bool pathGenerated = false; // ��ΰ� �����Ǿ�����
    private bool isGameOver = false; // ���� ���� ��������
    private bool isGameWon = false; // ���� �¸� ��������

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
        // ó������ ���� ���� ����
        SpawnCarPlayer();

        // ���̺� �Ŵ��� ã��
        if (waveManager == null)
        {
            waveManager = FindObjectOfType<WaveManager>();
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager �ν��Ͻ��� ã�� �� �����ϴ�.");
        }
        

        // ü�� �ʱ�ȭ
        nexusHealth = maxNexusHealth;
        UpdateHealthUI();

        // ���� ����/�¸� �г� ��Ȱ��ȭ
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

    }

    // ���� �÷��̾� ����
    private void SpawnCarPlayer()
    {
        if (carPlayerPrefab != null && isCarModeActive)
        {
            Instantiate(carPlayerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            Debug.Log("���� �÷��̾ �����Ǿ����ϴ�.");
        }
    }

    // �Ϲ� �÷��̾� ���� (�ؼ��� ���� ��)
    public void SpawnPlayer(GameObject playerPrefab)
    {
        if (playerPrefab != null)
        {
            Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            Debug.Log("�Ϲ� �÷��̾ �����Ǿ����ϴ�.");

            // ���� ��� ��Ȱ��ȭ
            isCarModeActive = false;

            // ��� ���������� ����
            pathGenerated = true;

        }
        WaveManager waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnPlayerReady();
        }
    }

    // ���� ��忡�� �÷��̾� ���� ��ȯ
    public void SwitchToPlayerMode(GameObject playerModePrefab)
    {
        if (isCarModeActive && playerModePrefab != null)
        {
            isCarModeActive = false;
            pathGenerated = true;

            SpawnPlayer(playerModePrefab);
            Debug.Log("���� ��忡�� �÷��̾� ���� ��ȯ�Ǿ����ϴ�.");

        }
    }

    // ��ġ ��� Ȯ��
    public bool CanAffordInstallation(int cost)
    {
        return installationCost >= cost; // ��ġ ����� ������ �� �ִ��� Ȯ��
    }

    // ��� ����
    public void DeductCost(int cost)
    {
        installationCost -= cost; // ��ġ ��� ����
        UpdateResourceUI();
    }

    // �ڿ� ���
    public int GetResources()
    {
        return installationCost;
    }

    // �ڿ� �߰�
    public void AddResources(int amount)
    {
        installationCost += amount;
        UpdateResourceUI();
        Debug.Log($"�ڿ� {amount}�� �߰��Ǿ����ϴ�. ���� �ڿ�: {installationCost}");
    }

    // �ڿ� ���
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
    /// ���� ���� ��ġ ����� Ȯ�������� ������ŵ�ϴ�.
    /// </summary>
    /// <param name="shopIncrement">���� ��� ������</param>
    /// <param name="installationIncrement">��ġ ��� ������</param>
    /// <param name="shopProbability">���� ��� ���� Ȯ�� (0.0���� 1.0 ����)</param>
    /// <param name="installationProbability">��ġ ��� ���� Ȯ�� (0.0���� 1.0 ����)</param>
    public void IncreaseCosts(int shopIncrement, int installationIncrement, float shopProbability, float installationProbability)
    {
        float shopRandomValue = Random.Range(0f, 1f);
        if (shopRandomValue <= shopProbability)
        {
            shopCost += shopIncrement;
            Debug.Log($"���� �׾����ϴ�. ���� ����� {shopIncrement} ��ŭ �����߽��ϴ�. ���� ���� ���: {shopCost}");
        }

        float installationRandomValue = Random.Range(0f, 1f);
        if (installationRandomValue <= installationProbability)
        {
            installationCost += installationIncrement;
            Debug.Log($"���� �׾����ϴ�. ��ġ ����� {installationIncrement} ��ŭ �����߽��ϴ�. ���� ��ġ ���: {installationCost}");
        }

        UpdateUI();
    }

    // UI ������Ʈ
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

    // �ڿ� UI ������Ʈ
    private void UpdateResourceUI()
    {
        if (resourceText != null)
        {
            resourceText.text = installationCost.ToString();
        }
    }

    // ü�� UI ������Ʈ
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
        // ���� ���� ���¿����� �Է� ����
        if (isGameOver || isGameWon)
            return;

        // ��ΰ� �����Ǿ��� �÷��̾� ����� ��쿡�� ���̺� ���� ����
        if (Input.GetKeyDown(KeyCode.G) && pathGenerated && !isCarModeActive)
        {
            AdvanceWave();
        }
    }

    // ���̺� ����
    public void AdvanceWave()
    {
        // ��ΰ� �������� �ʾ����� ���̺� ���� �Ұ�
        if (!pathGenerated)
        {
            Debug.LogWarning("��ΰ� �������� �ʾҽ��ϴ�! ���� �������� ���� ���弼��.");
            return;
        }

        if (waveManager == null)
        {
            Debug.LogError("WaveManager �ν��Ͻ��� null�Դϴ�.");
            return;
        }


        // ���� ���̺� ����
        waveManager.StartNextWave();

        // Ư�� ���̺꿡�� ��� ����
        int currentWave = waveManager.GetCurrentWave();
        if (currentWave == 2 || currentWave == 3 || currentWave == 7 || currentWave == 8)
        {
            IncreaseCosts(0, 3, 1.0f, 1.0f); // ���� ��� 0, ��ġ ��� 3 ���� (Ȯ�� 100%)
        }

        UpdateUI(); // ���̺� ���� �� UI ������Ʈ
    }

    // �� óġ �� ����
    public void EnemyDefeated()
    {
        IncreaseCosts(3, 2, 0.2f, 0.2f); // ���� ��� 3, ��ġ ��� 2 ���� (Ȯ������ ������ �°� ����)
    }

    // ��ġ ��� ���ʽ�
    public void InstallationCostBonus(int bonusAmount)
    {
        installationCost += bonusAmount;
        Debug.Log("��ġ ��� ���ʽ��� �߰��Ǿ����ϴ�: " + bonusAmount);
        UpdateUI();
    }



    // ���� ����
    private void OpenShop()
    {
        Debug.Log("������ ���Ƚ��ϴ�! �ʿ��� ������ �����ϼ���.");
        // ���� UI ǥ�� ���� �߰�
    }

    // �ؼ��� ����
    private void SpawnNexus()
    {
        if (nexusPrefab != null)
        {
            Instantiate(nexusPrefab, nexusSpawnPoint.position, nexusSpawnPoint.rotation); // �ؼ��� ����
        }
    }

    // ���� ����
    public void ResetTerrain()
    {
        if (terrainManager != null)
        {
            terrainManager.ResetTerrain(); // ���� �ʱ�ȭ
        }

        // ���� ���� ����
        isCarModeActive = true;
        pathGenerated = false;
    }

    // �ؼ��� ü�� ����
    public int GetHealth()
    {
        return nexusHealth;
    }

    // ü�� ����
    public void SetHealth(int newHealth)
    {
        nexusHealth = Mathf.Clamp(newHealth, 0, maxNexusHealth);
        UpdateHealthUI();

        // ü���� 0 ���ϸ� ���� ����
        if (nexusHealth <= 0)
        {
            GameOver();
        }
    }

    // �ؼ��� ������ ó��
    public void TakeDamage(float damage)
    {
        nexusHealth -= Mathf.RoundToInt(damage);
        nexusHealth = Mathf.Max(0, nexusHealth);

        UpdateHealthUI();

        Debug.Log($"�ؼ����� {damage} �������� �޾ҽ��ϴ�. ���� ü��: {nexusHealth}");

        // ü���� 0 ���ϸ� ���� ����
        if (nexusHealth <= 0)
        {
            GameOver();
        }
    }

    // ���� ���� ó��
    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("���� ����! �ؼ����� �ı��Ǿ����ϴ�.");

        // ���� ���� UI ǥ��
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // �ð� ������ ���� (������ ������ �ʰ�)
        Time.timeScale = 0.5f;

        // ���� ���� ȿ�� �߰� (���� ����)
        StartCoroutine(GameOverEffects());
    }

    // ���� ���� ȿ�� �ڷ�ƾ
    private IEnumerator GameOverEffects()
    {
        // ���� ���� ȿ�� (�ʿ�� �߰�)
        yield return new WaitForSeconds(2f);

        // �ð� ����ȭ
        Time.timeScale = 1f;
    }

    // ���� �¸� ó��
    public void GameWon()
    {
        if (isGameWon) return;

        isGameWon = true;
        Debug.Log("���� �¸�! ��� ���̺긦 Ŭ�����߽��ϴ�.");

        // �¸� UI ǥ��
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // �¸� ȿ�� (���� ����)
        StartCoroutine(VictoryEffects());
    }

    // �¸� ȿ�� �ڷ�ƾ
    private IEnumerator VictoryEffects()
    {
        // �¸� ȿ�� (�ʿ�� �߰�)
        yield return new WaitForSeconds(2f);
    }

    // IObserver �������̽� ����
    public void OnNotify(GameObject obj, string eventMessage)
    {
        if (eventMessage == "EnemyDefeated")
        {
            EnemyDefeated();
        }
    }

    // ���� ���� Ȯ�� �޼����
    public bool IsCarModeActive()
    {
        return isCarModeActive;
    }

    public bool IsPathGenerated()
    {
        return pathGenerated;
    }

    // ��� ���� �Ϸ� �˸�
    public void OnPathGenerated()
    {
        pathGenerated = true;
        Debug.Log("��� ������ �Ϸ�Ǿ����ϴ�! ���� ���̺긦 ������ �� �ֽ��ϴ�.");

    }
}