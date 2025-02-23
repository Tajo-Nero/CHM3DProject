using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    //2025- 02 -23
    //적이 넥서스만나면 공격하고 넥서스도 공격함
    //오브젝트풀 사용햇는대 아직 잘작동되는지는 모름 저기 넥서스랑 게임매니저쪽 일이 되야 체크 가능
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
            Debug.LogError("타겟이 설정되지 않았습니다.");
        }
        Debug.Log("적 이름: " + enemyName);
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
            Debug.LogWarning("타겟이 null입니다.");
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
                    Debug.Log("타겟을 공격했습니다! 데미지: " + attackDamage);
                }
            }
            yield return new WaitForSeconds(attackSpeed);
        }
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log(enemyName + "이(가) 데미지를 받았습니다! 현재 체력: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(enemyName + "이(가) 파괴되었습니다!");
        enemyPool.ReturnEnemy(gameObject); // 객체 풀에 반환
    }
}
