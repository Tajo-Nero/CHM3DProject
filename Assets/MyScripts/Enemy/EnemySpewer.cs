using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpewer : MonoBehaviour
{
    [Header("Enemy Spawning")]
    public string[] enemyNames; // ������ ���� �̸��� (EnemyPool�� ��ϵ� �̸�)
    public int enemiesPerWave = 10; // ���̺�� �� ��
    public float spawnInterval = 1f; // ���� ����

    private bool isSpawning = false; // ���� �� ����
    private EnemyPool enemyPool;
    private PathManager pathManager;

    void Start()
    {
        enemyPool = FindObjectOfType<EnemyPool>();
        pathManager = FindObjectOfType<PathManager>();

        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool�� ã�� �� �����ϴ�!");
        }

        if (pathManager == null)
        {
            Debug.LogError("PathManager�� ã�� �� �����ϴ�!");
        }
    }

    void Update()
    {
        // G Ű�� ������ ��ΰ� �������� �ʾ����� �������� ����
        if (Input.GetKeyDown(KeyCode.G) && !isSpawning)
        {
            if (pathManager.GetMainPath() != null && pathManager.GetMainPath().Count >= 2)
            {
                StartCoroutine(SpawnEnemies());
            }
            else
            {
                Debug.LogWarning("��ΰ� �������� �ʾҽ��ϴ�! ���� �������� ���� ���弼��.");
            }
        }
    }

    private IEnumerator SpawnEnemies()
    {
        if (enemyPool == null)
        {
            Debug.LogError("EnemyPool�� ��� ���� ������ �� �����ϴ�!");
            yield break;
        }

        if (enemyNames == null || enemyNames.Length == 0)
        {
            Debug.LogError("������ ���� �̸��� �������� �ʾҽ��ϴ�!");
            yield break;
        }

        isSpawning = true;
        Debug.Log($"�� ���� ����! {enemiesPerWave}������ ���� ������� �����մϴ�.");

        for (int i = 0; i < enemiesPerWave; i++)
        {
            // ������� �� ���� (�迭�� ��ȯ)
            int enemyIndex = i % enemyNames.Length;
            string enemyName = enemyNames[enemyIndex];

            // EnemyPool�� ���� �� ����
            GameObject spawnedEnemy = enemyPool.GetEnemy(enemyName, transform.position);

            if (spawnedEnemy != null)
            {
                spawnedEnemy.transform.rotation = transform.rotation;
                Debug.Log($"�� ���� ����: {enemyName} ({i + 1}/{enemiesPerWave}) - ����: {enemyIndex}");
            }
            else
            {
                Debug.LogError($"�� ���� ����: {enemyName}");
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
        Debug.Log("�� ���� �Ϸ�!");
    }

    // �ܺο��� ȣ���� �� �ִ� ���� �޼���
    public void SpawnWave(int waveSize = -1)
    {
        if (isSpawning)
        {
            Debug.LogWarning("�̹� ���� ���Դϴ�!");
            return;
        }

        if (waveSize > 0)
        {
            enemiesPerWave = waveSize;
        }

        StartCoroutine(SpawnEnemies());
    }

    // Ư�� ���� ����
    public void SpawnSpecificEnemy(string enemyName, int count = 1)
    {
        if (isSpawning)
        {
            Debug.LogWarning("�̹� ���� ���Դϴ�!");
            return;
        }

        StartCoroutine(SpawnSpecificEnemyCoroutine(enemyName, count));
    }

    private IEnumerator SpawnSpecificEnemyCoroutine(string enemyName, int count)
    {
        isSpawning = true;

        for (int i = 0; i < count; i++)
        {
            GameObject spawnedEnemy = enemyPool.GetEnemy(enemyName, transform.position);

            if (spawnedEnemy != null)
            {
                spawnedEnemy.transform.rotation = transform.rotation;
                Debug.Log($"Ư�� �� ����: {enemyName} ({i + 1}/{count})");
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    void OnDrawGizmos()
    {
        // ���� ���� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        // ���� ���� ǥ��
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
    }
}