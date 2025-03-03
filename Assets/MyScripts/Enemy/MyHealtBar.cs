using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Image healthBarImage; // 체력바 이미지
    public Text healthBarText; // 체력바 텍스트
    public float maxHealth; // 최대 체력

    public void Initialize(float health)
    {
        maxHealth = health;
        UpdateHealth(health, health);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBarImage != null)
        {
            float healthRatio = currentHealth / maxHealth;
            healthBarImage.fillAmount = healthRatio;
        }
        else
        {
            Debug.LogError("healthBarImage가 null입니다. MyHealthBar 스크립트에서 체력바 이미지를 참조하십시오.");
        }

        if (healthBarText != null)
        {
            healthBarText.text = $"{currentHealth} / {maxHealth}";
        }
        else
        {
            Debug.LogError("healthBarText가 null입니다. MyHealthBar 스크립트에서 체력바 텍스트를 참조하십시오.");
        }
    }

    public void SetHealthBarImage(Image image)
    {
        healthBarImage = image;
    }

    public void SetHealthBarText(Text text)
    {
        healthBarText = text;
    }
}
