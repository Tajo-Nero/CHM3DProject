using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    // 이름 대신 enum 사용
    public EnemyType enemyType;

    // 프리팹 직접 참조
    public GameObject prefab;

    // 기타 속성
    public float health;
    public float attackPower;
    public float moveSpeed;

    // 문자열 호환성 유지 (필요한 경우)
    public string GetEnemyName()
    {
        return enemyType.ToString();
    }
}

// 적 타입 열거형
public enum EnemyType
{
    Bee,
    Cute,
    Mushroom,
    Slime,          
    TurtleShell,
    Elite,
    Beholder,
    ChestMonster,   
    Cactus,
    Boss
}