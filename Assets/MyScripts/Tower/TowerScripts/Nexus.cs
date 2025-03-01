using System.Collections;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f; // 탐지 범위
    [SerializeField] private float attackDamage = 20f; // 공격 데미지
    [SerializeField] private float attackInterval = 2f; // 공격 간격 (2초)
    [SerializeField] private float health = 100f; // 넥서스 체력
    private bool isAttacking = false; // 공격 중인지 여부
    public float DetectionRange => detectionRange;

    void Start()
    {
        SetRange(detectionRange); // 탐지 범위 설정
    }

    void Update()
    {
        DetectEnemiesInRange(); // 적 탐지
    }

    private void DetectEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && !isAttacking)
            {
                StartCoroutine(AttackRoutine(hitCollider.transform));
            }
        }
    }

    private IEnumerator AttackRoutine(Transform target)
    {
        isAttacking = true;

        while (target != null && Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            EnemyBase enemyAI = target.GetComponent<EnemyBase>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(attackDamage); // 적에게 공격 데미지 입히기
                TakeDamage(enemyAI.enemy_attackDamage); // 넥서스가 적의 공격 데미지를 받음
                Debug.Log("넥서스가 " + target.name + "에게 공격을 가했습니다! 공격 데미지: " + attackDamage);
                Debug.Log("넥서스가 " + target.name + "에게 공격을 받았습니다! 공격 데미지: " + enemyAI.enemy_attackDamage);
            }

            //  target이 파괴되었는지 확인
            if (target == null)
            {
                Debug.Log("타겟이 파괴되었습니다.");
                break;
            }

            yield return new WaitForSeconds(attackInterval); // 공격 간격 대기
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("넥서스가 데미지를 입었습니다! 현재 체력: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("넥서스가 파괴되었습니다!");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    private void SetRange(float range)
    {
        detectionRange = range; // 탐지 범위 설정
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // 탐지 범위 시각화
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HandleCollision());
        }
    }

    private IEnumerator HandleCollision()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.BakeNavMesh();
        
    }
}
