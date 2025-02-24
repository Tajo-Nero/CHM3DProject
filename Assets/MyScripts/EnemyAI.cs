using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public string enemyName;
    public LayerMask targetLayerMask; // 타겟 레이어 마스크
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
            Debug.LogError("타겟이 설정되지 않았습니다.");
        }

        Debug.Log("적 이름: " + enemyName);
    }

    void Update()
    {
        if (target == null)
        {
            FindTargetWithLayerMask(); // 타겟이 null이면 다시 찾기 시도
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
            Debug.LogWarning("타겟이 null입니다.");
        }
    }

    private void FindTargetWithLayerMask()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f, targetLayerMask); // 일정 범위 내의 타겟 검색
        if (hitColliders.Length > 0)
        {
            target = hitColliders[0].gameObject;
        }
        else
        {
            Debug.LogWarning("타겟 레이어 마스크로 타겟을 찾지 못했습니다.");
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
                    Debug.Log("타겟을 공격했습니다! 피해량: " + attackDamage);
                }
            }
            yield return new WaitForSeconds(attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(enemyName + "이(가) 피해를 입었습니다! 남은 체력: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(enemyName + "이(가) 사망했습니다!");
        enemyPool.ReturnEnemy(gameObject); // 객체 풀로 반환
    }
}
