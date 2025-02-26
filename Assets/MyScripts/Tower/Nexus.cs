using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus : TowerBase
{
    [SerializeField] private float detectionRange = 5f; // Ž�� ����
    [SerializeField] private float attackDamage = 20f; // ���� ������
    [SerializeField] private float attackInterval = 2f; // ���� ���� (2��)
    [SerializeField] private float health = 100f; // �ؼ��� ü��
    private bool isAttacking = false; // ���� �� ����
    public float DetectionRange => detectionRange;

    void Start()
    {
        SetRange(detectionRange); // Ž�� ���� ����
    }

    void Update()
    {
        DetectEnemiesInRange(); // ���� �� �� Ž��
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
                enemyAI.TakeDamage(attackDamage); // ������ ������ ������
                TakeDamage(enemyAI.enemy_attackDamage); // �ؼ����� �����κ��� ������ �Ա�
                Debug.Log("�ؼ����� " + target.name + "���� ������ ���߽��ϴ�! ������: " + attackDamage);
                Debug.Log("�ؼ����� " + target.name + "���Լ� �������� �Ծ����ϴ�! ������: " + enemyAI.enemy_attackDamage);
            }
            yield return new WaitForSeconds(attackInterval); // ���� ���ݸ�ŭ ���
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("�ؼ����� ���ظ� �Ծ����ϴ�! ���� ü��: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("�ؼ����� �ı��Ǿ����ϴ�!");
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public override void SetRange(float range)
    {
        detectionRange = range; // Ž�� ���� ����
        Debug.Log("Ž�� ������ �����Ǿ����ϴ�: " + detectionRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Ž�� ���� �׸���
    }
    //ī �÷��̾� ����϶� ������ �׺�Ž� ����ũ �ؾ��ϴ´� �±װ� �ȸ��� ����
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

