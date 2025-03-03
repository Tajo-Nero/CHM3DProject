using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public EnemyData enemyData; // ScriptableObject 연결
    public float enemy_attackDamage;
    public float enemy_attackSpeed;
    public float enemy_health;
    private bool isAttacking = false;
    private NavMeshAgent navMeshAgent;
    private EnemyPool enemyPool;
    private GameObject target;
    protected Animator animator;
    private TowerGenerator towerGenerator;
    private MyHealthBar myHealthBar;
    public GameObject healthBarPrefab;

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeEnemyData();

        if (target == null)
        {
            Debug.LogError("Target을 찾을 수 없습니다.");
        }
    }

    private void InitializeComponents()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyPool = FindObjectOfType<EnemyPool>();
        animator = GetComponent<Animator>();
        target = FindObjectOfType<Nexus>().gameObject;
        towerGenerator = FindObjectOfType<TowerGenerator>();
    }

    private void InitializeEnemyData()
    {
        if (enemyData != null)
        {
            enemy_attackDamage = enemyData.attackPower;
            enemy_attackSpeed = enemyData.attackSpeed;
            enemy_health = enemyData.health;
        }
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

    void Start()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMesh 에이전트가 유효한 NavMesh에 있지 않습니다.");
            return;
        }
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.isStopped = true;
                Debug.Log("목표에 도착했습니다.");
            }

            if (distanceToTarget <= 1.5f && !isAttacking)
            {
                StartCoroutine(AttackTarget());
                Debug.Log("공격 시작");
            }

            animator.SetFloat("Run", navMeshAgent.velocity.magnitude);
        }
        else
        {
            Debug.LogWarning("Target이 null입니다.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target)
        {
            navMeshAgent.isStopped = true;
            Debug.Log("목표와 충돌");
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
                    targetNexus.TakeDamage(enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        ApplyDamage(damage);

        if (GetCurrentHealth() <= 0)
        {
            Die();
        }

        if (myHealthBar != null)
        {
            myHealthBar.UpdateHealth(enemy_health, myHealthBar.maxHealth);
        }
    }

    public float GetCurrentHealth()
    {
        return enemy_health;
    }

    protected void ApplyDamage(float damage)
    {
        enemy_health -= damage;
    }

    private void Die()
    {
        Debug.Log("적이 죽었습니다!");
        if (towerGenerator != null)
        {
            towerGenerator.NotifyObservers(gameObject, "EnemyDefeated");
        }
        if (myHealthBar != null)
        {
            Destroy(myHealthBar.gameObject);
        }
        enemyPool.ReturnEnemy(gameObject);
    }

    public void Victory()
    {
        animator.SetBool("isVictory", true);
        animator.SetTrigger("Attack");
    }
}
