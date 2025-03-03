using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Image healthBarImage; // ü�¹� �̹���
    public Text healthBarText; // ü�¹� �ؽ�Ʈ
    public float maxHealth; // �ִ� ü��

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
            Debug.LogError("healthBarImage�� null�Դϴ�. MyHealthBar ��ũ��Ʈ���� ü�¹� �̹����� �����Ͻʽÿ�.");
        }

        if (healthBarText != null)
        {
            healthBarText.text = $"{currentHealth} / {maxHealth}";
        }
        else
        {
            Debug.LogError("healthBarText�� null�Դϴ�. MyHealthBar ��ũ��Ʈ���� ü�¹� �ؽ�Ʈ�� �����Ͻʽÿ�.");
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
