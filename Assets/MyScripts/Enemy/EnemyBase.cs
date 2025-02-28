using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//�� Ŭ������ �θ� ����Ͽ� �ڽĿ��� ��üȭ�� �����մϴ�
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
            Debug.LogError("Target�� ã�� �� �����ϴ�.");
        }
    }

    void Start()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMesh ������Ʈ�� ��ȿ�� NavMesh ���� ���� �ʽ��ϴ�.");
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
                Debug.Log("Ÿ�� ���� �� ����");
            }

            if (distanceToTarget <= 1.5f && !isAttacking)
            {
                StartCoroutine(AttackTarget());
                Debug.Log("���� ����");
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
            Debug.Log("�ؼ����� �浹�Ͽ� ����");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            navMeshAgent.isStopped = true;
            Debug.Log("�ؼ����� Ʈ���� �̺�Ʈ�� ����");
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
                    Debug.Log("Ÿ���� �����߽��ϴ�! ������: " + enemy_attackDamage);
                }
            }
            yield return new WaitForSeconds(enemy_attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("���� �������� �޾ҽ��ϴ�! ���� ü��: " + GetCurrentHealth());
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
        Debug.Log("���� ����߽��ϴ�!");
        enemyPool.ReturnEnemy(gameObject);
    }

    public void Victory()
    {
        animator.SetBool("isVictory", true);
        animator.SetTrigger("Attack");
    }
}
