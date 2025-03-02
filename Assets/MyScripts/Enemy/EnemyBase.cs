using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public EnemyData enemyData;
    public float enemy_attackDamage;
    public float enemy_health;
    public float enemy_attackSpeed;
    public MyHealthBar healthBar; // MyHealthBar 필드 추가
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
            Debug.LogError("Target을 찾을 수 없습니다.");
        }

        // 추가된 옵션 설정
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = enemyData.movementSpeed;
        }
    }

    void Start()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMesh 에이전트가 유효한 NavMesh 안에 있지 않습니다.");
            return;
        }

        // 체력 바 초기화 (생성은 EnemyPool에서 이미 수행됨)
        InitializeHealthBar();
    }

    void InitializeHealthBar()
    {
        // EnemyPool에서 전달된 healthBar를 설정합니다.
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
                Debug.Log("타겟 근처");
            }

            if (distanceToTarget <= 1.5f && !isAttacking)
            {
                StartCoroutine(AttackTarget());
                Debug.Log("공격 중");
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
            Debug.Log("타겟과 충돌");
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
            yield return new WaitForSeconds(enemyData.attackSpeed); // enemyData.attackSpeed 사용
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        ApplyDamage(damage);

        // 체력 바 업데이트
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
        Debug.Log("적 사망!");
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
