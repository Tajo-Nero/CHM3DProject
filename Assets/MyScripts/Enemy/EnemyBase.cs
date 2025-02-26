using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//적 기능은 부모에만 쓸거고 자식에선 체력등만 들고있게함
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

        if (target == null)
        {
            Debug.LogError("Target을 찾을 수 없습니다.");
        }
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= 1.5f && !isAttacking)
            {
                StartCoroutine(AttackTarget());
            }

            animator.SetFloat("Run", navMeshAgent.velocity.magnitude);
        }
        else
        {
            Debug.LogWarning("Target이 null입니다.");
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
                    Debug.Log("타겟을 공격했습니다! 공격력: " + enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("적이 피해를 입었습니다! 남은 체력: " + GetCurrentHealth());
        ApplyDamage(damage);
        if (GetCurrentHealth() <= 0)
        {            
            Die();
        }
    }

    public abstract float GetCurrentHealth();
    protected abstract void ApplyDamage(float damge);


    private void Die()
    {
        Debug.Log("적이 사망했습니다!");
       //animator.SetBool("isDie", true);
       //navMeshAgent.isStopped = true;
       //yield return new WaitForSeconds(2f);
       enemyPool.ReturnEnemy(gameObject);
    }

    public void Victory()
    {
        animator.SetBool("isVictory", true);
        animator.SetTrigger("Attack");
    }
}
