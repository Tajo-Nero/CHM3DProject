using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth;
    public float currentHealth;

    // ��� �̺�Ʈ ����
    public delegate void EnemyDeathHandler(GameObject enemy);
    public event EnemyDeathHandler OnEnemyDeath;

    // �ʱ�ȭ �Լ�
    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealthValue;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // ü�� ������Ʈ �Լ�
    public void UpdateHealth(float currentHealthValue, float maxHealthValue)
    {
        currentHealth = currentHealthValue;
        maxHealth = maxHealthValue;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // ��� üũ
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(transform.parent.gameObject);
            }
        }
    }

    // ���� �Ա� �Լ�
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // ��� üũ
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(transform.parent.gameObject);
            }
        }
    }
}