using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton

    public Text shopCostText; // �� �ڽ�Ʈ �ؽ�Ʈ
    public Text installationCostText; // ��ġ �ڽ�Ʈ �ؽ�Ʈ
    public Scrollbar waveScrollBar; // ���̺� ��ũ�ѹ�
    public Text waveText; // ���̺� �ؽ�Ʈ

    //private int totalWaves = 12; // �� ���̺� ��
    
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

    public void UpdateShopCost(int cost)
    {
        if (shopCostText != null)
        {
            shopCostText.text = "Shop Cost: " + cost;
        }
    }

    public void UpdateInstallationCost(int cost)
    {
        if (installationCostText != null)
        {
            installationCostText.text = "Installation Cost: " + cost;
        }
    }

    public void UpdateWaveProgress(int currentWave, int totalWaves)
    {
        if (waveScrollBar != null)
        {
            float scrollValue = (float)currentWave / totalWaves;
            waveScrollBar.size = scrollValue;

            if (waveText != null)
            {
                waveText.text = $"Wave {currentWave}/{totalWaves}";
            }
        }
    }
}
