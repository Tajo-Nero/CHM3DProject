using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beholder : EnemyBase
{
    //public float enemy_health;

    protected override void Awake()
    {
        base.Awake();
        enemy_attackDamage = 10f;
        enemy_attackSpeed = 1.5f;
       
    }

    public override float GetCurrentHealth()
    {
        return enemy_health;
    }

    protected override void ApplyDamage(float damage)
    {
        enemy_health -= damage;
        Debug.Log($"{gameObject.name}�� {damage}��ŭ�� �������� �޾ҽ��ϴ�. ���� ü��: {enemy_health}");
    }
}
