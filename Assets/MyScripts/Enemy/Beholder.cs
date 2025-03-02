using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beholder : EnemyBase
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
        Debug.Log($"{gameObject.name}이 {damage}만큼의 피해를 받았습니다. 현재 체력: {enemy_health}");
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
