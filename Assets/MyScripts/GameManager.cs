using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Enemy Settings")]
    public EnemyPool enemyPool;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    private int enemyCount = 0;
    private int maxEnemies = 10;

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

    private GameObject currentTerrain;
    private GameObject currentPlayer;
    private GameObject currentNexus;

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
    }

    private void InitializeGame()
    {
        SpawnTerrain();
        //BakeNavMesh();
        SpawnPlayer(carPlayerPrefab);
    }

    public void SetupTerrain()
    {
        ResetTerrain();
        SpawnTerrain();
    }

    private void SpawnTerrain()
    {
        if (_UpTerrain == null)
        {
            Debug.LogError("_UpTerrain이 설정되지 않았습니다.");
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
        if (currentTerrain != null)
        {
            Destroy(currentTerrain);
        }
        SetupTerrain();
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
            Debug.LogError("NexusPrefab이 설정되지 않았습니다!");
        }
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh가 빌드되었습니다.");
    }

    public IEnumerator SpawnEnemies()
    {
        while (enemyCount < maxEnemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemyPool.SpawnEnemy(spawnPoint.position, spawnPoint.rotation);
            enemyCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
