using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public EnemyData enemyData;
    public float enemy_attackDamage;
    public float enemy_health;
    public float enemy_attackSpeed;
    public MyHealthBar healthBar; // MyHealthBar �ʵ� �߰�
    private bool isAttacking = false;
    private NavMeshAgent navMeshAgent;
    private EnemyPool enemyPool;
    private GameObject target;
    protected Animator animator;
    private TowerGenerator towerGenerator;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyPool = FindObjectOfType<EnemyPool>();
        animator = GetComponent<Animator>();
        target = FindObjectOfType<Nexus>().gameObject;
        towerGenerator = FindObjectOfType<TowerGenerator>();
        enemy_attackDamage = enemyData.attackPower;
        enemy_attackSpeed = enemyData.attackSpeed;
        enemy_health = enemyData.health;

        if (target == null)
        {
            Debug.LogError("Target�� ã�� �� �����ϴ�.");
        }

        // �߰��� �ɼ� ����
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = enemyData.movementSpeed;
        }
    }

    void Start()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMesh ������Ʈ�� ��ȿ�� NavMesh �ȿ� ���� �ʽ��ϴ�.");
            return;
        }

        // ü�� �� �ʱ�ȭ (������ EnemyPool���� �̹� �����)
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        // EnemyPool���� ���޵� healthBar�� �����մϴ�.
        healthBar = GetComponentInChildren<MyHealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(enemy_health);
        }
    }

    public void Update()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.isStopped = true;
                Debug.Log("Ÿ�� ��ó");
            }

            if (distanceToTarget <= 1.5f && !isAttacking)
            {
                StartCoroutine(AttackTarget());
                Debug.Log("���� ��");
            }

            animator.SetFloat("Run", navMeshAgent.velocity.magnitude);
        }
        else
        {
            Debug.LogWarning("Target�� null�Դϴ�.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target)
        {
            navMeshAgent.isStopped = true;
            Debug.Log("Ÿ�ٰ� �浹");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            navMeshAgent.isStopped = true;
        }
    }

    private IEnumerator AttackTarget()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        while (Vector3.Distance(transform.position, target.transform.position) <= 1.5f)
        {
            if (target != null)
            {
                Nexus targetNexus = target.GetComponent<Nexus>();
                if (targetNexus != null)
                {
                    targetNexus.TakeDamage(enemyData.attackPower);
                }
            }
            yield return new WaitForSeconds(enemyData.attackSpeed); // enemyData.attackSpeed ���
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        ApplyDamage(damage);

        // ü�� �� ������Ʈ
        if (healthBar != null)
        {
            healthBar.UpdateHealth(GetCurrentHealth(), enemyData.health);
        }

        if (GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    public virtual float GetCurrentHealth()
    {
        return enemy_health;
    }

    protected virtual void ApplyDamage(float damage)
    {
        enemy_health -= damage;
    }

    private void Die()
    {
        Debug.Log("�� ���!");
        if (towerGenerator != null)
        {
            towerGenerator.NotifyObservers(gameObject, "EnemyDefeated");
        }
        enemyPool.ReturnEnemy(gameObject);
    }

    public void Victory()
    {
        animator.SetBool("isVictory", true);
        animator.SetTrigger("Attack");
    }
}
