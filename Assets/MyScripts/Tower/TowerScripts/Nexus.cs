using System.Collections;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f; // Ž�� ����
    [SerializeField] private float attackDamage = 20f; // ���� ������
    [SerializeField] private float attackInterval = 2f; // ���� ���� (2��)
    [SerializeField] private float maxHealth = 100f; // �ؼ��� �ִ� ü��
    [SerializeField] private float currentHealth = 100f; // �ؼ��� ���� ü��

    private bool isAttacking = false; // ���� ������ ����
    private PathManager pathManager;

    public float DetectionRange => detectionRange;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    void Start()
    {
        currentHealth = maxHealth;

        pathManager = FindObjectOfType<PathManager>();

        SetRange(detectionRange); // Ž�� ���� ����
    }

    void Update()
    {
        DetectEnemiesInRange(); // �� Ž��
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
            // EnemyPathFollower�� ����
            EnemyPathFollower enemyPathFollower = target.GetComponent<EnemyPathFollower>();
            if (enemyPathFollower != null)
            {
                enemyPathFollower.TakeDamage(attackDamage); // ������ ���� ������ ������
                TakeDamage(enemyPathFollower.enemy_attackDamage); // �ؼ����� ���� ���� �������� ����

                Debug.Log($"�ؼ����� {target.name}���� ������ ���߽��ϴ�! ���� ������: {attackDamage}");
                Debug.Log($"�ؼ����� {target.name}���� ������ �޾ҽ��ϴ�! ���� ������: {enemyPathFollower.enemy_attackDamage}");
            }

            // target�� �ı��Ǿ����� Ȯ��
            if (target == null)
            {
                Debug.Log("���� �ı��Ǿ����ϴ�.");
                break;
            }

            yield return new WaitForSeconds(attackInterval); // ���� ���� ���
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // ü���� 0 ���Ϸ� �������� �ʵ���

        Debug.Log($"�ؼ����� �������� �Ծ����ϴ�! ���� ü��: {currentHealth}/{maxHealth}");

        // UI ������Ʈ (�ʿ��ϸ� �߰�)
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthUI()
    {
        // UI �Ŵ����� �ִٸ� ü�¹� ������Ʈ
        if (UIManager.Instance != null)
        {
            // UIManager.Instance.UpdateNexusHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        Debug.Log("�ؼ����� �ı��Ǿ����ϴ�! ���� ����!");

        // ���� ���� ó��
        if (GameManager.Instance != null)
        {
            // GameManager.Instance.GameOver();
        }

        gameObject.SetActive(false);
        // Destroy(gameObject); // ��� �ı����� ���� ���� ���� ���� �Ŀ�
    }

    private void SetRange(float range)
    {
        detectionRange = range; // Ž�� ���� ����
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Ž�� ���� �ð�ȭ
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
        Debug.Log("�÷��̾ �ؼ����� �����߽��ϴ�!");

        // ���� ��忡�� �÷��̾� ���� ��ȯ�ϱ� ���� ��� ����
        PlayerCarMode carMode = player.GetComponent<PlayerCarMode>();
        if (carMode != null && GameManager.Instance.IsCarModeActive())
        {
            Debug.Log("���� ��� ������. ��θ� �����մϴ�...");

            if (pathManager != null && pathManager.HasValidPath())
            {
                Debug.Log("�÷��̾ ���� ��θ� ����մϴ�!");
            }
            else
            {
                Debug.LogError("��ȿ�� ��ΰ� �����ϴ�!");
            }

            yield return new WaitForSeconds(0.5f); // �ణ�� ����

            // GameManager�� ���� �÷��̾� ���� ��ȯ
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SwitchToPlayerMode(carMode._PlayerMode);
                GameManager.Instance.OnPathGenerated(); // ��� ���� �Ϸ� �˸�
            }

            Destroy(player); // ���� ��� ����
        }
        else
        {
            Debug.Log("�Ϲ� �÷��̾ �ؼ����� �����߽��ϴ�.");
        }
    }

    // ü�� ȸ�� ��� (�ʿ�� ���)
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth); // �ִ� ü���� �ʰ����� �ʵ���

        Debug.Log($"�ؼ����� {healAmount} ü���� ȸ���߽��ϴ�! ���� ü��: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
    }

    // �ִ� ü�� ���׷��̵� ��� (�ʿ�� ���)
    public void UpgradeMaxHealth(float additionalHealth)
    {
        maxHealth += additionalHealth;
        currentHealth += additionalHealth; // ���� ü�µ� ���� ����

        Debug.Log($"�ؼ��� �ִ� ü���� �����߽��ϴ�! ���� ü��: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
    }

    // ü�� ���� ��ȯ (UI��)
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}