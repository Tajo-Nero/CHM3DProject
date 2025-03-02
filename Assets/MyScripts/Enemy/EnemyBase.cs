//using System.Collections;
//using UnityEngine;
//using UnityEngine.AI;

//public abstract class EnemyBase : MonoBehaviour
//{
//    public float enemy_attackDamage;
//    public float enemy_attackSpeed;
//    public float enemy_health;
//    private bool isAttacking = false;
//    private NavMeshAgent navMeshAgent;
//    private EnemyPool enemyPool;
//    private GameObject target;
//    protected Animator animator;
//    private TowerGenerator towerGenerator;

//    protected virtual void Awake()
//    {
//        navMeshAgent = GetComponent<NavMeshAgent>();
//        enemyPool = FindObjectOfType<EnemyPool>();
//        animator = GetComponent<Animator>();
//        target = FindObjectOfType<Nexus>().gameObject;
//        NavMeshAgent agent = GetComponent<NavMeshAgent>();
//        towerGenerator = FindObjectOfType<TowerGenerator>();

//        if (target == null)
//        {
//            Debug.LogError("Target�� ã�� �� �����ϴ�.");
//        }
//    }

//    void Start()
//    {
//        if (!navMeshAgent.isOnNavMesh)
//        {
//            Debug.LogError("NavMesh ������Ʈ�� ��ȿ�� NavMesh �ȿ� ���� �ʽ��ϴ�.");
//            return;
//        }
//    }

//    void Update()
//    {
//        if (target != null)
//        {
//            navMeshAgent.SetDestination(target.transform.position);
//            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

//            if (distanceToTarget <= navMeshAgent.stoppingDistance)
//            {
//                navMeshAgent.isStopped = true;
//                Debug.Log("Ÿ�� ����");
//            }

//            if (distanceToTarget <= 1.5f && !isAttacking)
//            {
//                StartCoroutine(AttackTarget());
//                Debug.Log("���� ����");
//            }

//            animator.SetFloat("Run", navMeshAgent.velocity.magnitude);
//        }
//        else
//        {
//            Debug.LogWarning("Target�� null�Դϴ�.");
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        if (collision.gameObject == target)
//        {
//            navMeshAgent.isStopped = true;
//            Debug.Log("Ÿ�ٰ� �浹�Ͽ� ����");
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject == target)
//        {
//            navMeshAgent.isStopped = true;
//        }
//    }

//    private IEnumerator AttackTarget()
//    {
//        isAttacking = true;
//        animator.SetTrigger("Attack");
//        while (Vector3.Distance(transform.position, target.transform.position) <= 1.5f)
//        {
//            if (target != null)
//            {
//                Nexus targetNexus = target.GetComponent<Nexus>();
//                if (targetNexus != null)
//                {
//                    targetNexus.TakeDamage(enemy_attackDamage);
//                    //Debug.Log("Ÿ�ٿ� �������� �������ϴ�! ���ݷ�: " + enemy_attackDamage);
//                }
//            }
//            yield return new WaitForSeconds(enemy_attackSpeed);
//        }
//        isAttacking = false;
//    }

//    public void TakeDamage(float damage)
//    {
//        //Debug.Log("�������� �Ծ����ϴ�! ���� ü��: " + GetCurrentHealth());
//        ApplyDamage(damage);
//        if (GetCurrentHealth() <= 0)
//        {
//            Die();
//        }
//    }

//    public abstract float GetCurrentHealth();
//    protected abstract void ApplyDamage(float damage);

//    private void Die()
//    {
//        Debug.Log("���� ����߽��ϴ�!");
//        // �������� �˸�
//        if (towerGenerator != null)
//        {
//            towerGenerator.NotifyObservers(gameObject, "EnemyDefeated");
//        }
//        enemyPool.ReturnEnemy(gameObject);
//    }

//    public void Victory()
//    {
//        animator.SetBool("isVictory", true);
//        animator.SetTrigger("Attack");
//    }
//}
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public float enemy_attackDamage;
    public float enemy_attackSpeed;
    public float enemy_health;
    public GameObject healthBarPrefab; // ü�¹� ������
    public MyHealthBar healthBar; // MyHealthBar �ʵ� �߰�
    private GameObject healthBarInstance; // ü�¹� �ν��Ͻ�
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

        if (target == null)
        {
            Debug.LogError("Target�� ã�� �� �����ϴ�.");
        }
    }

    void Start()
    {
        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError("NavMesh ������Ʈ�� ��ȿ�� NavMesh �ȿ� ���� �ʽ��ϴ�.");
            return;
        }

        // ü�¹� �ʱ�ȭ
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBar = healthBarInstance.GetComponent<MyHealthBar>();
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
                Debug.Log("Ÿ�� ����");
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
            Debug.Log("Ÿ�ٰ� �浹�Ͽ� ����");
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

        // ü�¹� ������Ʈ
        if (healthBarInstance != null)
        {
            healthBar.UpdateHealth(GetCurrentHealth(), enemy_health);
        }

        if (GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    public abstract float GetCurrentHealth();
    protected abstract void ApplyDamage(float damage);

    private void Die()
    {
        Debug.Log("�� ���!");
        if (towerGenerator != null)
        {
            towerGenerator.NotifyObservers(gameObject, "EnemyDefeated");
        }
        enemyPool.ReturnEnemy(gameObject);
        Destroy(healthBarInstance); // ü�¹� �ν��Ͻ� ����
    }

    public void Victory()
    {
        animator.SetBool("isVictory", true);
        animator.SetTrigger("Attack");
    }
}


