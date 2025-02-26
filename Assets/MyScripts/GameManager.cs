using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Settings")]
    public Wave[] waves; // ���̺� �����͸� �迭�� ����
    public int currentWaveIndex = 0; // ���� ���̺� �ε���

    [Header("Enemy Settings")]
    public Transform[] spawnPoints; // �� ���� ��ġ
    private EnemyPool enemyPool;

    [Header("Terrain Settings")]
    public Terrain _UpTerrain;
    public Transform terrainSpawnPoint;
    [SerializeField] private int resolution = 129;
    [SerializeField] private float heightScale = 3f;
    [SerializeField] private float terrainWidth = 60;
    [SerializeField] private float terrainLength = 60;

    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;

    [Header("Player Settings")]
    public GameObject playerPrefab;
    public GameObject carPlayerPrefab;
    public Transform playerSpawnPoint;

    [Header("Nexus Settings")]
    public GameObject nexusPrefab;
    public Transform nexusSpawnPoint;
    public bool IsWaveActive { get; private set; }
    private GameObject currentTerrain;
    private GameObject currentPlayer;
    private GameObject currentNexus;
    Vector3 currentSpawnPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnNexus();
        InitializeGame();
        enemyPool = FindObjectOfType<EnemyPool>();
        currentSpawnPoint = carPlayerPrefab.transform.position;
        Debug.Log(carPlayerPrefab.transform.position);
    }

    private void InitializeGame()
    {
        SpawnTerrain();
        // BakeNavMesh();
        SpawnPlayer(carPlayerPrefab);
    }
    //public IEnumerator StartWave()
    //{
    //    if (currentWaveIndex < waves.Length)
    //    {
    //        Wave waveData = waves[currentWaveIndex];
    //        int enemyCount = waveData.wave_enemyCount;

    //        for (int i = 0; i < enemyCount; i++)
    //        {
    //            int spawnIndex = i % spawnPoints.Length;
    //            EnemyData enemyData = waveData.wave_enemyData[i % waveData.wave_enemyData.Length];

    //            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
    //            Quaternion spawnRotation = Quaternion.identity;

    //            GameObject enemyInstance = enemyPool.SpawnEnemy(enemyData.enemyName, spawnPosition, spawnRotation); // GameObject�� ��ȯ����
    //            EnemyBase enemyComponent = enemyInstance.GetComponent<EnemyBase>();

    //            if (enemyComponent != null)
    //            {
    //                // �ڽ� Ŭ�������� ������ �����͸� �����մϴ�.
    //                enemyComponent.enemy_attackDamage = enemyData.attackPower;
    //                enemyComponent.enemy_attackSpeed = 1f; // ���÷� ������ ���� �ӵ�
    //                if (enemyComponent is Slime slime)
    //                {
    //                    slime.enemy_health = enemyData.health;
    //                }
    //                else if (enemyComponent is TurtleShell turtleShell)
    //                {
    //                    turtleShell.enemy_health = enemyData.health;
    //                }
    //                else if (enemyComponent is Beholder beholder)
    //                {
    //                    beholder.enemy_health = enemyData.health;
    //                }
    //                else if (enemyComponent is ChestMonster chestMonster)
    //                {
    //                    chestMonster.enemy_health = enemyData.health;
    //                }

    //            }

    //            yield return new WaitForSeconds(1f);
    //        }

    //        while (enemyPool.ActiveEnemies > 0)
    //        {
    //            yield return null;
    //        }

    //        currentWaveIndex++;

    //    }
    //}

    public IEnumerator StartWave()
    {
        if (currentWaveIndex < waves.Length)
        {
            Wave waveData = waves[currentWaveIndex];
            int enemyCount = waveData.wave_enemyCount;

            // �� ���� �ڷ�ƾ ����
            StartCoroutine(SpawnEnemies(waveData, enemyCount));

            // ������ ��� �Ҹ��� ������ ���
            while (enemyPool.ActiveEnemies > 0)
            {
                yield return null;
            }

            // ���̺� �ε��� ����
            currentWaveIndex++;
        }
    }

    private IEnumerator SpawnEnemies(Wave waveData, int enemyCount)
    {
        for (int i = 0; i < enemyCount; i++)
        {
            int spawnIndex = i % spawnPoints.Length;
            EnemyData enemyData = waveData.wave_enemyData[i % waveData.wave_enemyData.Length];

            Vector3 spawnPosition = spawnPoints[spawnIndex].position;
            Quaternion spawnRotation = Quaternion.identity;

            GameObject enemyInstance = enemyPool.SpawnEnemy(enemyData.enemyName, spawnPosition, spawnRotation); // GameObject�� ��ȯ����
            EnemyBase enemyComponent = enemyInstance.GetComponent<EnemyBase>();

            if (enemyComponent != null)
            {
                // �ڽ� Ŭ�������� ������ �����͸� �����մϴ�.
                enemyComponent.enemy_attackDamage = enemyData.attackPower;
                enemyComponent.enemy_attackSpeed = 1f; // ���÷� ������ ���� �ӵ�
                if (enemyComponent is Slime slime)
                {
                    slime.enemy_health = enemyData.health;
                }
                else if (enemyComponent is TurtleShell turtleShell)
                {
                    turtleShell.enemy_health = enemyData.health;
                }
                else if (enemyComponent is Beholder beholder)
                {
                    beholder.enemy_health = enemyData.health;
                }
                else if (enemyComponent is Bee bee)
                {
                    bee.enemy_health = enemyData.health;
                }
                else if (enemyComponent is ChestMonster chestMonster)
                {
                    chestMonster.enemy_health = enemyData.health;
                }
                else if (enemyComponent is Cactus cactus)
                {
                    cactus.enemy_health = enemyData.health;
                }
                else if (enemyComponent is Elite elite)
                {
                    elite.enemy_health = enemyData.health;
                }
                else if (enemyComponent is Cute cute)
                {
                    cute.enemy_health = enemyData.health;
                }
                else if (enemyComponent is Mushroom mushroom)
                {
                    mushroom.enemy_health = enemyData.health;
                }

            }

            yield return new WaitForSeconds(1f);
        }
    }

   

    private void SpawnTerrain()
    {
        if (_UpTerrain == null)
        {
            Debug.LogError("_UpTerrain�� �������� �ʾҽ��ϴ�.");
            return;
        }

        Vector3 spawnPosition = terrainSpawnPoint.position;
        _UpTerrain.transform.position = spawnPosition;
        _UpTerrain.terrainData = new TerrainData();
        InitializeTerrainData(_UpTerrain.terrainData);
        SetTerrainHeights(_UpTerrain.terrainData);

        TerrainCollider terrainCollider = _UpTerrain.gameObject.GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            terrainCollider = _UpTerrain.gameObject.AddComponent<TerrainCollider>();
        }
        terrainCollider.terrainData = _UpTerrain.terrainData;
    }

    private void InitializeTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(terrainWidth, heightScale, terrainLength);
    }

    private void SetTerrainHeights(TerrainData terrainData)
    {
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        for (int y = 2; y < terrainData.heightmapResolution - 2; y++)
        {
            for (int x = 2; x < terrainData.heightmapResolution - 2; x++)
            {
                heights[y, x] = heightScale / terrainData.size.y;
            }
        }

        SmoothTerrain(heights);
        terrainData.SetHeights(0, 0, heights);

        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            terrain.Flush();
        }
    }

    private void SmoothTerrain(float[,] heights)
    {
        int width = heights.GetLength(0);
        int height = heights.GetLength(1);

        for (int i = 0; i < 5; i++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    float average = (heights[y, x] + heights[y - 1, x] + heights[y + 1, x] + heights[y, x - 1] + heights[y, x + 1]) / 5f;
                    heights[y, x] = average;
                }
            }
        }
    }

    public void ResetTerrain()
    {
        if (_UpTerrain != null)
        {
            //Destroy(_UpTerrain);
            _UpTerrain.gameObject.SetActive(false);
            
        }            
        SpawnPlayer(carPlayerPrefab.gameObject);
        SpawnTerrain();
        _UpTerrain.gameObject.SetActive(true);

    }

    public void SpawnPlayer(GameObject playerPrefab)
    {
        currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
    }

    private void SpawnNexus()
    {
        if (nexusPrefab != null)
        {
            currentNexus = Instantiate(nexusPrefab, nexusSpawnPoint.position, nexusSpawnPoint.rotation);
        }
        else
        {
            Debug.LogError("NexusPrefab�� �������� �ʾҽ��ϴ�!");
        }
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh�� ���������ϴ�.");
    }

}