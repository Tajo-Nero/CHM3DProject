using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//�� ����� �θ𿡸� ���Ű� �ڽĿ��� ü�µ ����ְ���
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
            Debug.LogError("Target�� ã�� �� �����ϴ�.");
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
            Debug.LogWarning("Target�� null�Դϴ�.");
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
                    Debug.Log("Ÿ���� �����߽��ϴ�! ���ݷ�: " + enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("���� ���ظ� �Ծ����ϴ�! ���� ü��: " + GetCurrentHealth());
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
        Debug.Log("���� ����߽��ϴ�!");
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
