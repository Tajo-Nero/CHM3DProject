using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Text healthText; // ü�� �ؽ�Ʈ �߰�
    public float maxHealth;
    public float currentHealth;

    // ü�¹� ������
    public Vector3 offset = new Vector3(0, 2f, 0);

    private Camera mainCamera;
    private Transform enemyTransform;

    // ��� �̺�Ʈ ����
    public delegate void EnemyDeathHandler(GameObject enemy);
    public event EnemyDeathHandler OnEnemyDeath;

    void Awake()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }

        // �ؽ�Ʈ �ڵ� ã��
        if (healthText == null)
        {
            healthText = GetComponentInChildren<Text>();
        }

        mainCamera = Camera.main;
        enemyTransform = transform.root;

        // ��� ��ġ ����
        SetInitialPosition();
    }

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
        {
            canvas.worldCamera = mainCamera;
        }

        // Start������ �� �� �� ��ġ ����
        SetInitialPosition();
    }

    void SetInitialPosition()
    {
        if (enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;
        }
    }

    // �ʱ�ȭ �Լ�
    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealthValue;

        UpdateHealthDisplay();

        // �ʱ�ȭ�� ���� ��ġ ����
        SetInitialPosition();
    }

    // ü�� ������Ʈ �Լ�
    public void UpdateHealth(float currentHealthValue, float maxHealthValue)
    {
        currentHealth = currentHealthValue;
        maxHealth = maxHealthValue;

        UpdateHealthDisplay();

        // ��� üũ
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(enemyTransform.gameObject);
            }
        }
    }

    // ���� �Ա� �Լ�
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        UpdateHealthDisplay();

        // ��� üũ
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(enemyTransform.gameObject);
            }
        }
    }

    // ü�� ǥ�� ������Ʈ
    private void UpdateHealthDisplay()
    {
        // �����̴� ������Ʈ
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // �ؽ�Ʈ ������Ʈ (����ü��/�ִ�ü��)
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{Mathf.Ceil(maxHealth)}";
        }
    }
}