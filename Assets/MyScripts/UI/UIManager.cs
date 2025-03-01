using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton

    public Text shopCostText; // 샵 코스트 텍스트
    public Text installationCostText; // 설치 코스트 텍스트
    public Scrollbar waveScrollBar; // 웨이브 스크롤바
    public Text waveText; // 웨이브 텍스트

    //private int totalWaves = 12; // 총 웨이브 수
    
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
