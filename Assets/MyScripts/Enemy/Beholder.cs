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
        Debug.Log($"{gameObject.name}가 {damage}만큼의 데미지를 받았습니다. 남은 체력: {enemy_health}");
    }
}
