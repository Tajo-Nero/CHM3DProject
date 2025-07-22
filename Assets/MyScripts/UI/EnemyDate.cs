using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    // �̸� ��� enum ���
    public EnemyType enemyType;

    // ������ ���� ����
    public GameObject prefab;

    // ��Ÿ �Ӽ�
    public float health;
    public float attackPower;
    public float moveSpeed;

    // ���ڿ� ȣȯ�� ���� (�ʿ��� ���)
    public string GetEnemyName()
    {
        return enemyType.ToString();
    }
}

// �� Ÿ�� ������
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