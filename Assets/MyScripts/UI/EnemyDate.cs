using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    
    public string enemyName;
    public float health;
    public float attackPower;
    public float attackSpeed;
    public float movementSpeed;
}
