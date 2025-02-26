using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : TowerBase
{
    [SerializeField] private float detectionRange = 5f; // 탐지 범위
    [SerializeField] private float attackDamage = 20f; // 공격 데미지
    [SerializeField] private float attackInterval = 2f; // 공격 간격 (2초)
    [SerializeField] private float health = 100f; // 넥서스 체력
    private bool isAttacking = false; // 공격 중 여부
    public float DetectionRange => detectionRange;

    void Start()
    {
        SetRange(detectionRange); // 탐지 범위 설정
    }

    void Update()
    {
        DetectEnemiesInRange(); // 범위 내 적 탐지
    }

    public override void DetectEnemiesInRange()
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

        while (Vector3.Distance(transform.position, target.position) <= detectionRange)
        {
            EnemyBase enemyAI = target.GetComponent<EnemyBase>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(attackDamage); // 적에게 데미지 입히기
                TakeDamage(enemyAI.enemy_attackDamage); // 넥서스가 적으로부터 데미지 입기
                Debug.Log("넥서스가 " + target.name + "에게 공격을 가했습니다! 데미지: " + attackDamage);
                Debug.Log("넥서스가 " + target.name + "에게서 데미지를 입었습니다! 데미지: " + enemyAI.enemy_attackDamage);
            }
            yield return new WaitForSeconds(attackInterval); // 공격 간격만큼 대기
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("넥서스가 피해를 입었습니다! 현재 체력: " + health);

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

    public override void SetRange(float range)
    {
        detectionRange = range; // 탐지 범위 설정
        Debug.Log("탐지 범위가 설정되었습니다: " + detectionRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // 탐지 범위 그리기
    }
    //카 플레이어 모드일때 만나면 네비매쉬 배이크 해야하는대 태그가 안먹힘 지금
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        GameManager.Instance.BakeNavMesh();
    //    }
    //}
    IEnumerator OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            yield return new WaitForSeconds(1f);
            GameManager.Instance.BakeNavMesh();
        }
    }
}   

