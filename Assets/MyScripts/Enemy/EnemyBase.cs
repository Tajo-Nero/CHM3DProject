using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//적 클래스의 부모를 상속하여 자식에서 구체화를 진행합니다
public abstract class EnemyBase : MonoBehaviour
{
    public float enemy_attackDamage;
    public float enemy_attackSpeed;
    private bool isAttacking = false;
    private NavMeshAgent navMeshAgent;
    private EnemyPool enemyPool;
    private GameObject target;
    protected Animator animator;


    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyPool = FindObjectOfType<EnemyPool>();
        animator = GetComponent<Animator>();
        target = FindObjectOfType<Nexus>().gameObject;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (target == null)
        {
            Debug.LogError("Target을 찾을 수 없습니다.");
        }
    }

    void Start()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMesh 에이전트가 유효한 NavMesh 위에 있지 않습니다.");
            return;
        }
    }

    void Update()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.isStopped = true;
                Debug.Log("타겟 도달 후 멈춤");
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
            Debug.Log("넥서스와 충돌하여 멈춤");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            navMeshAgent.isStopped = true;
            Debug.Log("넥서스와 트리거 이벤트로 멈춤");
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
                    Debug.Log("타겟을 공격했습니다! 데미지: " + enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("적이 데미지를 받았습니다! 현재 체력: " + GetCurrentHealth());
        ApplyDamage(damage);
        if (GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    public abstract float GetCurrentHealth();
    protected abstract void ApplyDamage(float damage);

    private void Die()
    {
        Debug.Log("적이 사망했습니다!");
        enemyPool.ReturnEnemy(gameObject);
    }

    public void Victory()
    {
        animator.SetBool("isVictory", true);
        animator.SetTrigger("Attack");
    }
}
