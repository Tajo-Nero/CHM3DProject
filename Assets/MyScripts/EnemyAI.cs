using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public string enemyName;
    public LayerMask targetLayerMask; // Ÿ�� ���̾� ����ũ
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    public float health = 50f;
    private bool isAttacking = false;
    private NavMeshAgent navMeshAgent;
    private EnemyPool enemyPool;
    private GameObject target;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyPool = FindObjectOfType<EnemyPool>();
        FindTargetWithLayerMask();

        if (target == null)
        {
            Debug.LogError("Ÿ���� �������� �ʾҽ��ϴ�.");
        }

        Debug.Log("�� �̸�: " + enemyName);
    }

    void Update()
    {
        if (target == null)
        {
            FindTargetWithLayerMask(); // Ÿ���� null�̸� �ٽ� ã�� �õ�
        }

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

    private void FindTargetWithLayerMask()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f, targetLayerMask); // ���� ���� ���� Ÿ�� �˻�
        if (hitColliders.Length > 0)
        {
            target = hitColliders[0].gameObject;
        }
        else
        {
            Debug.LogWarning("Ÿ�� ���̾� ����ũ�� Ÿ���� ã�� ���߽��ϴ�.");
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
                    Debug.Log("Ÿ���� �����߽��ϴ�! ���ط�: " + attackDamage);
                }
            }
            yield return new WaitForSeconds(attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(enemyName + "��(��) ���ظ� �Ծ����ϴ�! ���� ü��: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(enemyName + "��(��) ����߽��ϴ�!");
        enemyPool.ReturnEnemy(gameObject); // ��ü Ǯ�� ��ȯ
    }
}
