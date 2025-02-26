using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "ScriptableObjects/Wave", order = 1)]
public class Wave : ScriptableObject
{
    public int wave_enemyCount; // 웨이브에 생성될 적의 수
    public EnemyData[] wave_enemyData; // 웨이브에 생성될 적의 정보 배열
}
