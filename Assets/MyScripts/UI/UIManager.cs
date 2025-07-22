using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton

    [Header("비용 UI")]
    public Text shopCostText; // 샵 코스트 텍스트
    public Text installationCostText; // 설치 코스트 텍스트

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

    // 샵 비용 업데이트
    public void UpdateShopCost(int cost)
    {
        if (shopCostText != null)
        {
            shopCostText.text = "Shop Cost: " + cost;
        }
    }

    // 설치 비용 업데이트
    public void UpdateInstallationCost(int cost)
    {
        if (installationCostText != null)
        {
            installationCostText.text = "Tower Cost: " + cost;
        }
    }

    // 모든 비용 UI 업데이트
    public void UpdateAllCosts(int shopCost, int installationCost)
    {
        UpdateShopCost(shopCost);
        UpdateInstallationCost(installationCost);
    }
}