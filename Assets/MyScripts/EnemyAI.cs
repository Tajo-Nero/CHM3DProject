using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //2025- 02 -23
    //���� �ؼ��������� �����ϰ� �ؼ����� ������
    //������ƮǮ ����޴´� ���� ���۵��Ǵ����� �� ���� �ؼ����� ���ӸŴ����� ���� �Ǿ� üũ ����
    public string enemyName;
    public GameObject target;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    public float health = 50f;
    private bool isAttacking = false;
    private NavMeshAgent navMeshAgent;
    private EnemyPool enemyPool;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyPool = FindObjectOfType<EnemyPool>();
        if (target == null)
        {
            Debug.LogError("Ÿ���� �������� �ʾҽ��ϴ�.");
        }
        Debug.Log("�� �̸�: " + enemyName);
    }

    void Update()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.transform.position);
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distanceToTarget <= 1.5f && !isAttacking)
            {
                StartCoroutine(AttackTarget());
            }
        }
        else
        {
            Debug.LogWarning("Ÿ���� null�Դϴ�.");
        }
    }

    private IEnumerator AttackTarget()
    {
        isAttacking = true;
        while (Vector3.Distance(transform.position, target.transform.position) <= 1.5f)
        {
            if (target != null)
            {
                Nexus targetNexus = target.GetComponent<Nexus>();
                if (targetNexus != null)
                {
                    targetNexus.TakeDamage(attackDamage);
                    Debug.Log("Ÿ���� �����߽��ϴ�! ������: " + attackDamage);
                }
            }
            yield return new WaitForSeconds(attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(enemyName + "��(��) �������� �޾ҽ��ϴ�! ���� ü��: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(enemyName + "��(��) �ı��Ǿ����ϴ�!");
        enemyPool.ReturnEnemy(gameObject); // ��ü Ǯ�� ��ȯ
    }
}
