using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpewer : MonoBehaviour
{
    //2025- 02 -23
    //�׺�Ž� ������Ʈ�� �̿��Ͽ� ���� �����ϰ� �̵���Ű�� ��ũ��Ʈ
    //������ ���̰��̼����� ����ũ�ؾ� �ҿ����� NavMeshSurface �� �ִ� ����ũ �ϸ� ���� 2�������� ���ߴ� ��������
    public GameObject[] enemyPrefabs; // �� ������ �迭
    private bool isSpawning = false; // ���� �� ����

    void Update()
    {
        // G Ű�� ������ �� ���� ����
        if (Input.GetKeyDown(KeyCode.G) && !isSpawning)
        {
            StartCoroutine(SpawnEnemies()); // �� ���� �ڷ�ƾ ����
        }
    }
    

    private IEnumerator SpawnEnemies()
    {
        isSpawning = true; // ���� ������ ����

        for (int i = 0; i < 10; i++) // 10���� �� ����
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length); // ���� �ε��� ����
            Instantiate(enemyPrefabs[randomIndex], transform.position, transform.rotation); // ���� �� ����
            yield return new WaitForSeconds(1f); // 1�� ���
        }

        isSpawning = false; // ���� �Ϸ�� ����
    }
}
