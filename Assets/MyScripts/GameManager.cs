using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    //2025-02-23
    //일단 맨처음 시작시 생성될건 생성되게 간단하게만 만들어놨음
    //터레인 생성시 맨저음 작업했던거랑 느낌이 좀 다름 왜 달라졌을까 전에 해놨던거 기록좀해놓을껄
    public static GameManager Instance { get; private set; }

    [Header("Enemy Settings")]
    public EnemyPool enemyPool;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    private int enemyCount = 0;
    private int maxEnemies = 10;

    [Header("Terrain Settings")]
    public GameObject terrainPrefab;
    public Transform terrainSpawnPoint;
    [SerializeField] private int resolution = 129;
    [SerializeField] private float heightScale = 2f;
    [SerializeField] private float terrainWidth = 50;
    [SerializeField] private float terrainLength = 50;

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
    private bool isCarPlayerActive = true;

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
        SpawnNexus();  // 넥서스를 먼저 생성
        InitializeGame();  // 게임 초기화
    }

    private void InitializeGame()
    {
        SpawnTerrain();  // 넥서스 주변이 아닌 곳에 터레인을 생성
        BakeNavMesh();
        SpawnPlayer(carPlayerPrefab);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(SpawnEnemies());
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ResetTerrain();

        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SwapPlayerPrefab();
        }
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

    private void SpawnTerrain()
    {
        if (currentNexus == null)
        {
            Debug.LogError("Nexus 객체가 생성되지 않았습니다!");
            return;
        }

        Vector3 nexusPosition = currentNexus.transform.position;
        Nexus nexusComponent = currentNexus.GetComponent<Nexus>();
        if (nexusComponent == null)
        {
            Debug.LogError("Nexus 컴포넌트를 찾을 수 없습니다!");
            return;
        }

        float exclusionRadius = nexusComponent.DetectionRange; // 넥서스의 공격 범위를 제외 반경으로 설정
        Vector3 spawnPosition = terrainSpawnPoint.position;

        // 넥서스 근처라면 Terrain을 생성하지 않음
        if (Vector3.Distance(spawnPosition, nexusPosition) < exclusionRadius)
        {
            Debug.Log("넥서스 공격 범위 내이므로 Terrain이 생성되지 않습니다.");
            return;
        }

        currentTerrain = Instantiate(terrainPrefab, spawnPosition, terrainSpawnPoint.rotation);
        Terrain terrainComponent = currentTerrain.GetComponent<Terrain>();

        if (terrainComponent != null)
        {
            TerrainData terrainData = new TerrainData();
            terrainComponent.terrainData = terrainData;
            InitializeTerrainData(terrainData);
            SetTerrainHeights(terrainComponent.terrainData);
        }

        // TerrainCollider 추가하기
        TerrainCollider terrainCollider = currentTerrain.GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            terrainCollider = currentTerrain.AddComponent<TerrainCollider>();
        }
        terrainCollider.terrainData = terrainComponent.terrainData;
    }

    private void ResetTerrain()
    {
        if (currentTerrain != null)
        {
            Destroy(currentTerrain);
        }
        SpawnTerrain();
    }

    private void SpawnPlayer(GameObject playerPrefab)
    {
        currentPlayer = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
    }

    public void BakeNavMesh()
    {
        navMeshSurface.BuildNavMesh();
        Debug.Log("NavMesh가 빌드되었습니다.");
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

    private IEnumerator SpawnEnemies()
    {
        while (enemyCount < maxEnemies)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemyPool.SpawnEnemy(spawnPoint.position, spawnPoint.rotation);
            enemyCount++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SwapPlayerPrefab()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        isCarPlayerActive = !isCarPlayerActive;
        GameObject prefabToSpawn = isCarPlayerActive ? carPlayerPrefab : playerPrefab;
        SpawnPlayer(prefabToSpawn);
    }
    //충돌 체크중 카플레이어 일때 넥서스(타겟)이랑 만나면 네비매쉬 새로 베이크하고 플레이어 프리팹 생성 그자리에 생성
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target") && currentPlayer.CompareTag("Player"))
        {
            BakeNavMesh();
            if (isCarPlayerActive)
            {
                Destroy(currentPlayer);
                SpawnPlayer(playerPrefab);
            }
        }

    }
}