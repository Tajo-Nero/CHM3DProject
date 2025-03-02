using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beholder : EnemyBase
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
        Debug.Log($"{gameObject.name}�� {damage}��ŭ�� ���ظ� �޾ҽ��ϴ�. ���� ü��: {enemy_health}");
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
