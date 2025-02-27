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

    // 1. Ÿ�� ���ݷ�
    [Header("Tower Settings")]
    public float towerAttackPower;
    // 2. Ÿ�� �����
    public float towerPenetrationPower;
    // 3. ġ��Ÿ Ȯ��
    public float criticalHitRate;
    // 4. ���� �ӵ�
    public float attackSpeed;
    // 5. ��ġ ���
    public float installationCost;
    // 6. ���� ��ȭ ���ɿ���
    public bool isAttackUp = false;

    // ���� �����ϴ� �Լ�
    public virtual void TowerAttack(List<Transform> targets)
    {
        // ���� ���� ����        
    }

    // ������ �����ϴ� �Լ�
    public virtual void SetRange(float range)
    {
        // ���� ���� ���� ����
    }

    // ���� ���� ���� Ž���ϴ� �Լ�
    public virtual void DetectEnemiesInRange()
    {
        // �� Ž�� ���� ����
    }
    public virtual void TowerPowUp()
    {
        if (isAttackUp)
        {
            towerAttackPower *= 2;
            isAttackUp = false;
        }
    }
}
