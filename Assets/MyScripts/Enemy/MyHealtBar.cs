using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Image healthFillImage;
    public Text healthText;

    public void Initialize(float maxHealth)
    {
        UpdateHealth(maxHealth, maxHealth);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealth / maxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
