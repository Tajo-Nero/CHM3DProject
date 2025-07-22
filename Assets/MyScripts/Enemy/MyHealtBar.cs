using UnityEngine;
using UnityEngine.UI;

public class MyHealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Text healthText; // 체력 텍스트 추가
    public float maxHealth;
    public float currentHealth;

    // 체력바 오프셋
    public Vector3 offset = new Vector3(0, 2f, 0);

    private Camera mainCamera;
    private Transform enemyTransform;

    // 사망 이벤트 정의
    public delegate void EnemyDeathHandler(GameObject enemy);
    public event EnemyDeathHandler OnEnemyDeath;

    void Awake()
    {
        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }

        // 텍스트 자동 찾기
        if (healthText == null)
        {
            healthText = GetComponentInChildren<Text>();
        }

        mainCamera = Camera.main;
        enemyTransform = transform.root;

        // 즉시 위치 설정
        SetInitialPosition();
    }

    void Start()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
        {
            canvas.worldCamera = mainCamera;
        }

        // Start에서도 한 번 더 위치 설정
        SetInitialPosition();
    }

    void SetInitialPosition()
    {
        if (enemyTransform != null)
        {
            transform.position = enemyTransform.position + offset;
        }
    }

    // 초기화 함수
    public void Initialize(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        currentHealth = maxHealthValue;

        UpdateHealthDisplay();

        // 초기화할 때도 위치 설정
        SetInitialPosition();
    }

    // 체력 업데이트 함수
    public void UpdateHealth(float currentHealthValue, float maxHealthValue)
    {
        currentHealth = currentHealthValue;
        maxHealth = maxHealthValue;

        UpdateHealthDisplay();

        // 사망 체크
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(enemyTransform.gameObject);
            }
        }
    }

    // 피해 입기 함수
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);

        UpdateHealthDisplay();

        // 사망 체크
        if (currentHealth <= 0)
        {
            if (OnEnemyDeath != null)
            {
                OnEnemyDeath.Invoke(enemyTransform.gameObject);
            }
        }
    }

    // 체력 표시 업데이트
    private void UpdateHealthDisplay()
    {
        // 슬라이더 업데이트
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // 텍스트 업데이트 (현재체력/최대체력)
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(currentHealth)}/{Mathf.Ceil(maxHealth)}";
        }
    }
}