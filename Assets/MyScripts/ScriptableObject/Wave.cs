using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "ScriptableObjects/Wave", order = 1)]
public class Wave : ScriptableObject
{
    public int wave_enemyCount; // ���̺꿡 ������ ���� ��
    public EnemyData[] wave_enemyData; // ���̺꿡 ������ ���� ���� �迭
}
