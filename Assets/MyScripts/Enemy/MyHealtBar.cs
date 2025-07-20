using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public float maxHealth;
    public float currentHealth;

    // 사망 이벤트 정의
    public delegate void EnemyDeathHandler(GameObject enemy);
    public event EnemyDeathHandler OnEnemyDeath;

    // 초기화 함수
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

    // 체력 업데이트 함수
    public void UpdateHealth(float currentHealthValue, float maxHealthValue)
    {
        currentHealth = currentHealthValue;
        maxHealth = maxHealthValue;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 사망 체크
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(transform.parent.gameObject);
            }
        }
    }

    // 피해 입기 함수
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // 사망 체크
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(transform.parent.gameObject);
            }
        }
    }
}