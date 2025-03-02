//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Bee : EnemyBase
//{

//    //public float enemy_health;

//    protected override void Awake()
//    {
//        base.Awake();
//        enemy_attackDamage = 10f;
//        enemy_attackSpeed = 1.5f;

//    }

//    public override float GetCurrentHealth()
//    {
//        return enemy_health;
//    }

//    protected override void ApplyDamage(float damage)
//    {
//        enemy_health -= damage;
//    }
//}

using UnityEngine;

public class Bee : EnemyBase
{
    public EnemyData enemyData; // ScriptableObject 데이터

    protected override void Awake()
    {
        base.Awake();
        enemy_attackDamage = enemyData.attackPower;
        enemy_health = enemyData.health;

        // MyHealthBar 초기화
        healthBar = GetComponentInChildren<MyHealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(enemyData.health);
        }
    }

    public override float GetCurrentHealth()
    {
        return enemy_health;
    }

    protected override void ApplyDamage(float damage)
    {
        enemy_health -= damage;
    }

    new void Update()
    {
        base.Update();
        // MyHealthBar 업데이트
        if (healthBar != null)
        {
            healthBar.UpdateHealth(GetCurrentHealth(), enemyData.health);
        }
    }
}

