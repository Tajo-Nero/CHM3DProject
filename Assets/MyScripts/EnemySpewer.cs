using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpewer : MonoBehaviour
{
    //2025- 02 -23
    //네비매쉬 에이전트를 이용하여 적을 생성하고 이동시키는 스크립트
    //하지만 네이게이션으로 베이크해야 소용있음 NavMeshSurface 에 있는 베이크 하면 몬스터 2개생성시 멈추는 현상있음
    public GameObject[] enemyPrefabs; // 적 프리팹 배열
    private bool isSpawning = false; // 생성 중 여부

    void Update()
    {
        // G 키를 누르면 적 생성 시작
        if (Input.GetKeyDown(KeyCode.G) && !isSpawning)
        {
            StartCoroutine(SpawnEnemies()); // 적 생성 코루틴 시작
        }
    }
    

    private IEnumerator SpawnEnemies()
    {
        isSpawning = true; // 생성 중으로 설정

        for (int i = 0; i < 10; i++) // 10개의 적 생성
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length); // 랜덤 인덱스 선택
            Instantiate(enemyPrefabs[randomIndex], transform.position, transform.rotation); // 랜덤 적 생성
            yield return new WaitForSeconds(1f); // 1초 대기
        }

        isSpawning = false; // 생성 완료로 설정
    }
}
