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
    public EnemyData enemyData; // ScriptableObject ������

    protected override void Awake()
    {
        base.Awake();
        enemy_attackDamage = enemyData.attackPower;
        enemy_health = enemyData.health;

        // MyHealthBar �ʱ�ȭ
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
        // MyHealthBar ������Ʈ
        if (healthBar != null)
        {
            healthBar.UpdateHealth(GetCurrentHealth(), enemyData.health);
        }
    }
}

