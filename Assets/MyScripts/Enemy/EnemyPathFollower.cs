using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyPathFollower : MonoBehaviour
{
    [Header("Enemy Data")]
    public EnemyData enemyData;
    public float enemy_attackDamage;
    public float enemy_attackSpeed;
    public float enemy_health;

    [Header("Movement")]
    public float moveSpeed = 5f; // �⺻�� ����
    public float rotationSpeed = 360f;
    public float waypointReachDistance = 1f;

    private MyHealthBar healthBar;
    private bool isAttacking = false;
    public List<Vector3> currentPath;
    public int currentWaypointIndex = 0;
    private GameObject target;
    private EnemyPool enemyPool;
    private TowerGenerator towerGenerator;
    private MyHealthBar myHealthBar;
    public GameObject healthBarPrefab;
    protected Animator animator;

    public delegate void DeathHandler(GameObject enemy);
    public static event DeathHandler OnAnyEnemyDeath;

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeEnemyData();

        // �ڽĿ��� ü�¹� ã��
        healthBar = GetComponentInChildren<MyHealthBar>();

        if (healthBar == null)
        {
            Debug.LogWarning($"{gameObject.name}�� ü�¹ٰ� �����ϴ�!");
        }
    }
    protected virtual void Start()
    {
        // ü�¹� �ʱ�ȭ
        if (healthBar != null && enemyData != null)
        {
            healthBar.Initialize(enemyData.health);
            // ������ �� �� �� ��ġ ������Ʈ
            healthBar.transform.position = transform.position + healthBar.offset;
        }
    }

    void OnEnable()
    {
        // ��Ȱ��ȭ�� �� ��ΰ� ������ �ʱ�ȭ
        currentPath = null;
        currentWaypointIndex = 0;
    }
    private void InitializeComponents()
    {
        animator = GetComponent<Animator>();
        enemyPool = FindObjectOfType<EnemyPool>();
        target = FindObjectOfType<Nexus>()?.gameObject;
        towerGenerator = FindObjectOfType<TowerGenerator>();
    }

    private void InitializeEnemyData()
    {
        if (enemyData != null)
        {
            enemy_attackDamage = enemyData.attackPower;
            enemy_health = enemyData.health;

            // moveSpeed ���� (0�̸� �⺻�� ���)
            if (enemyData.moveSpeed > 0)
            {
                moveSpeed = enemyData.moveSpeed;
            }

        }
        else
        {
            Debug.LogWarning($"EnemyData�� �������� �ʾҽ��ϴ�! �⺻�� ��� - �ӵ�: {moveSpeed}");
        }
    }


    public void SetPath(List<Vector3> waypoints)
    {
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError($"{gameObject.name}: �� ��ΰ� ���޵Ǿ����ϴ�!");
            return;
        }

        currentPath = new List<Vector3>(waypoints);
        currentWaypointIndex = 0;

        Debug.Log($"{gameObject.name}: ��� ���� �Ϸ� - {currentPath.Count}�� ����");
    }

    public void InitializeHealth()
    {
        if (enemyData != null)
        {
            enemy_health = enemyData.health;
        }

        if (myHealthBar == null && healthBarPrefab != null)
        {
            GameObject healthBar = Instantiate(healthBarPrefab, transform);
            myHealthBar = healthBar.GetComponent<MyHealthBar>();
        }

        if (myHealthBar != null)
        {
            myHealthBar.Initialize(enemy_health);
        }
    }

    protected virtual void Update()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            return;
        }

        MoveAlongPath();
        CheckForAttack();
    }

    void MoveAlongPath()
    {
        if (currentWaypointIndex >= currentPath.Count)
        {
            Debug.Log($"��� ��������Ʈ�� �����߽��ϴ�: {gameObject.name}");
            return;
        }

        Vector3 targetWaypoint = currentPath[currentWaypointIndex];
        Vector3 direction = (targetWaypoint - transform.position).normalized;

        // �̵� �Ÿ� ���
        float moveDistance = moveSpeed * Time.deltaTime;

        // �̵�
        transform.position += direction * moveDistance;

        // ȸ��
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // �ִϸ��̼� ������Ʈ
        if (animator != null)
        {
            animator.SetFloat("Run", moveSpeed);
        }

        // ��������Ʈ ���� üũ
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint);
        if (distanceToWaypoint < waypointReachDistance)
        {
            currentWaypointIndex++;
            Debug.Log($"��������Ʈ {currentWaypointIndex}/{currentPath.Count} ����");
        }

        // ����׿� - ���� ��ǥ ��������Ʈ ǥ��
        Debug.DrawLine(transform.position, targetWaypoint, Color.red, 0.1f);
    }

    void CheckForAttack()
    {
        if (target == null || isAttacking) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget <= 2f) // ���� �Ÿ�
        {
            StartCoroutine(AttackTarget());
        }
    }

    private IEnumerator AttackTarget()
    {
        isAttacking = true;
        Debug.Log($"{gameObject.name}��(��) �ؼ����� �����մϴ�!");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        while (Vector3.Distance(transform.position, target.transform.position) <= 2f)
        {
            if (target != null)
            {
                Nexus targetNexus = target.GetComponent<Nexus>();
                if (targetNexus != null)
                {
                    targetNexus.TakeDamage(enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        enemy_health -= damage;
        Debug.Log($"{gameObject.name}��(��) {damage} �������� �޾ҽ��ϴ�. ���� ü��: {enemy_health}");

        // ü�¹� ������Ʈ - healthBar ��� (myHealthBar �ƴ�!)
        if (healthBar != null)
        {
            healthBar.TakeDamage(damage); // UpdateHealth ��� TakeDamage ���
        }

        if (enemy_health <= 0)
        {
            Die();
        }
    }

    public float GetCurrentHealth()
    {
        return enemy_health;
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name}��(��) �׾����ϴ�!");

        // ���� �̺�Ʈ �߻�
        OnAnyEnemyDeath?.Invoke(gameObject);

        if (towerGenerator != null)
        {
            towerGenerator.NotifyObservers(gameObject, "EnemyDefeated");
        }

        if (enemyPool != null)
        {
            enemyPool.ReturnEnemy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Victory()
    {
        if (animator != null)
        {
            animator.SetBool("isVictory", true);
            animator.SetTrigger("Attack");
        }
    }

    // ����׿� - ��� �ð�ȭ
    void OnDrawGizmos()
    {
        if (currentPath != null && currentPath.Count > 1)
        {
            Gizmos.color = Color.blue;

            // ���� ��ġ���� ���� ��������Ʈ���� �� �׸���
            if (currentWaypointIndex < currentPath.Count)
            {
                Gizmos.DrawLine(transform.position, currentPath[currentWaypointIndex]);
            }

            // ��ü ��� �׸���
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                if (i >= currentWaypointIndex)
                {
                    Gizmos.color = Color.yellow; // ���� ���� ���� ���
                }
                else
                {
                    Gizmos.color = Color.gray; // �̹� ������ ���
                }

                Gizmos.DrawLine(currentPath[i], currentPath[i + 1]);
                Gizmos.DrawWireSphere(currentPath[i], 0.3f);
            }

            // ������ ��������Ʈ
            if (currentPath.Count > 0)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(currentPath[currentPath.Count - 1], 0.5f);
            }
        }
    }
}