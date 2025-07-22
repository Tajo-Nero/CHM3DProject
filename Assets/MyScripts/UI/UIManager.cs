using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; } // Singleton

    [Header("��� UI")]
    public Text shopCostText; // �� �ڽ�Ʈ �ؽ�Ʈ
    public Text installationCostText; // ��ġ �ڽ�Ʈ �ؽ�Ʈ

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

    // �� ��� ������Ʈ
    public void UpdateShopCost(int cost)
    {
        if (shopCostText != null)
        {
            shopCostText.text = "Shop Cost: " + cost;
        }
    }

    // ��ġ ��� ������Ʈ
    public void UpdateInstallationCost(int cost)
    {
        if (installationCostText != null)
        {
            installationCostText.text = "Tower Cost: " + cost;
        }
    }

    // ��� ��� UI ������Ʈ
    public void UpdateAllCosts(int shopCost, int installationCost)
    {
        UpdateShopCost(shopCost);
        UpdateInstallationCost(installationCost);
    }
}