using System.Collections;
using UnityEngine;

public class Nexus : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f; // Ž�� ����
    [SerializeField] private float attackDamage = 20f; // ���� ������
    [SerializeField] private float attackInterval = 2f; // ���� ���� (2��)
    [SerializeField] private float health = 100f; // �ؼ��� ü��
    private bool isAttacking = false; // ���� ������ ����
    public float DetectionRange => detectionRange;

    void Start()
    {
        SetRange(detectionRange); // Ž�� ���� ����
    }

    void Update()
    {
        DetectEnemiesInRange(); // �� Ž��
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
                enemyAI.TakeDamage(attackDamage); // ������ ���� ������ ������
                TakeDamage(enemyAI.enemy_attackDamage); // �ؼ����� ���� ���� �������� ����
                Debug.Log("�ؼ����� " + target.name + "���� ������ ���߽��ϴ�! ���� ������: " + attackDamage);
                Debug.Log("�ؼ����� " + target.name + "���� ������ �޾ҽ��ϴ�! ���� ������: " + enemyAI.enemy_attackDamage);
            }

            //  target�� �ı��Ǿ����� Ȯ��
            if (target == null)
            {
                Debug.Log("Ÿ���� �ı��Ǿ����ϴ�.");
                break;
            }

            yield return new WaitForSeconds(attackInterval); // ���� ���� ���
        }

        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("�ؼ����� �������� �Ծ����ϴ�! ���� ü��: " + health);

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

    private void SetRange(float range)
    {
        detectionRange = range; // Ž�� ���� ����
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Ž�� ���� �ð�ȭ
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
