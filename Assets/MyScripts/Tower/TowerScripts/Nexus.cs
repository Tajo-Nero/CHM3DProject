using System.Collections;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f; // 탐지 범위
    [SerializeField] private float attackDamage = 20f; // 공격 데미지
    [SerializeField] private float attackInterval = 2f; // 공격 간격 (2초)
    [SerializeField] private float maxHealth = 100f; // 넥서스 최대 체력
    [SerializeField] private float currentHealth = 100f; // 넥서스 현재 체력

    private bool isAttacking = false; // 공격 중인지 여부
    private PathManager pathManager;

    public float DetectionRange => detectionRange;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;

        pathManager = FindObjectOfType<PathManager>();

        SetRange(detectionRange); // 탐지 범위 설정
    }

    void Update()
    {
        DetectEnemiesInRange(); // 적 탐지
    }

    private void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && !isAttacking)
            {
                StartCoroutine(AttackRoutine(hitCollider.transform));
            }
        }
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        isAttacking = true;

        while (target != null && Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            // EnemyPathFollower로 변경
            EnemyPathFollower enemyPathFollower = target.GetComponent<EnemyPathFollower>();
            if (enemyPathFollower != null)
            {
                enemyPathFollower.TakeDamage(attackDamage); // 적에게 공격 데미지 입히기
                TakeDamage(enemyPathFollower.enemy_attackDamage); // 넥서스가 적의 공격 데미지를 받음

                Debug.Log($"넥서스가 {target.name}에게 공격을 가했습니다! 공격 데미지: {attackDamage}");
                Debug.Log($"넥서스가 {target.name}에게 공격을 받았습니다! 받은 데미지: {enemyPathFollower.enemy_attackDamage}");
            }

            // target이 파괴되었는지 확인
            if (target == null)
            {
                Debug.Log("적이 파괴되었습니다.");
                break;
            }

            yield return new WaitForSeconds(attackInterval); // 공격 간격 대기
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // 체력이 0 이하로 떨어지지 않도록

        Debug.Log($"넥서스가 데미지를 입었습니다! 현재 체력: {currentHealth}/{maxHealth}");

        // UI 업데이트 (필요하면 추가)
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        // UI 매니저가 있다면 체력바 업데이트
        if (UIManager.Instance != null)
        {
            // UIManager.Instance.UpdateNexusHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        Debug.Log("넥서스가 파괴되었습니다! 게임 오버!");

        // 게임 오버 처리
        if (GameManager.Instance != null)
        {
            // GameManager.Instance.GameOver();
        }

        gameObject.SetActive(false);
        // Destroy(gameObject); // 즉시 파괴하지 말고 게임 오버 연출 후에
    }

    private void SetRange(float range)
    {
        detectionRange = range; // 탐지 범위 설정
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // 탐지 범위 시각화
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HandlePlayerCollision(collision.gameObject));
        }
    }

    private IEnumerator HandlePlayerCollision(GameObject player)
    {
        Debug.Log("플레이어가 넥서스에 도달했습니다!");

        // 차량 모드에서 플레이어 모드로 전환하기 전에 경로 생성
        PlayerCarMode carMode = player.GetComponent<PlayerCarMode>();
        if (carMode != null && GameManager.Instance.IsCarModeActive())
        {
            Debug.Log("차량 모드 감지됨. 경로를 생성합니다...");

            if (pathManager != null && pathManager.HasValidPath())
            {
                Debug.Log("플레이어가 만든 경로를 사용합니다!");
            }
            else
            {
                Debug.LogError("유효한 경로가 없습니다!");
            }

            yield return new WaitForSeconds(0.5f); // 약간의 지연

            // GameManager를 통해 플레이어 모드로 전환
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SwitchToPlayerMode(carMode._PlayerMode);
                GameManager.Instance.OnPathGenerated(); // 경로 생성 완료 알림
            }

            Destroy(player); // 차량 모드 제거
        }
        else
        {
            Debug.Log("일반 플레이어가 넥서스에 도달했습니다.");
        }
    }

    // 체력 회복 기능 (필요시 사용)
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // 최대 체력을 초과하지 않도록

        Debug.Log($"넥서스가 {healAmount} 체력을 회복했습니다! 현재 체력: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
    }

    // 최대 체력 업그레이드 기능 (필요시 사용)
    public void UpgradeMaxHealth(float additionalHealth)
    {
        maxHealth += additionalHealth;
        currentHealth += additionalHealth; // 현재 체력도 같이 증가

        Debug.Log($"넥서스 최대 체력이 증가했습니다! 현재 체력: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
    }

    // 체력 비율 반환 (UI용)
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}