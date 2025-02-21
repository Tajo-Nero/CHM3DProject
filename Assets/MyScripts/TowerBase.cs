using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour 
{
    //1.Ÿ�� ���ݷ�
    //public int towerAttackPower;
    ////2.Ÿ�� �����
    //public int towerPenetrationPower;
    ////3.ġ��Ÿ��
    //public float criticalHitRate;
    ////4.���ݼӵ�
    //public float attackSpeed;
    ////5.��ġ ���
    //public int installationCost;
    //6.����

    //���� �ϴ� �Լ�,�������� �� ���� �Լ�,���� �����ϴ� �Լ�
    //void TowerAttak()//Ÿ�� �����ϴ� ���
    //{
    //    
    //}
    //void SetRange()//Ÿ�� ���� ������ ����
    //{
    //    
    //}
    //void DetectEnemiesInRange()//Ÿ�� �������� �� ����
    //{
    //    
    //}
    public float attackRange = 10.0f;  // ���� ����
    public float attackInterval = 1.0f;  // ���� ���� (��)
    public int attackDamage = 10;  // ���� ������
    public string enemyTag = "Enemy";  // �� �±�

    private float attackTimer;  // ���� Ÿ�̸�

    void Start()
    {
        attackTimer = 0f;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            DetectAndAttackEnemies();
            attackTimer = 0f;
        }
    }

    void DetectAndAttackEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                AttackEnemy(hitCollider.gameObject);
                break;  // �� ���� ���� ���� ���� �� ���� ����
            }
        }
    }

    void AttackEnemy(GameObject enemy)
    {
        // ������ �������� �ִ� ����
        EnemyHp enemyHealth = enemy.GetComponent<EnemyHp>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackDamage);
            Debug.Log("Attacked enemy: " + enemy.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        // ���� ������ �ð������� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }





    //������ Ÿ���� TowerBase�� ��ӹ���

    //ĳ�� Ÿ�� ���ϴ�� ���� ĳ��Ÿ�� 
    //1 : 80 2: 25 3: 5% 4: 0.7 5 : 8 6 : ��������

    //������ Ÿ�� ���� ���� ������ ���Ÿ�� 
    //1 : 250 2: 0 3: 15% 4: 2.5 5: 11 6: ����������

    //���� Ÿ�� ���÷��������ϴ� Ÿ�� 
    //1 : 50 2: 100 3: 5% 4: 1.5 5:10 6: ���� ��ä�� ����
    //���÷��� ����, ���ӵ����� ���� �߰�
    //���÷��� ���� �Լ�, ���� �������� �ִ� �Լ� �߰�

    //��ȭ Ÿ�� �������� Ÿ���� ������ ��ȭ�����ְ� �������� ����� ���� ����
    //1 : 14 2: 100 3: 5% 4: 3.5 5: 14 6: ���� ��ä�� ����
    //��ȭ���ӽð�, ���ݷ� �÷��� ���� �߰�
    //�������� Towers �±� �پ��ִ� Ÿ���� ���ݷ� ��ġ �����ϴ� �Լ� �߰�



}
