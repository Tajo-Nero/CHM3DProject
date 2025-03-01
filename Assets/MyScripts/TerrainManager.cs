using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // 지형 객체
    public Transform terrainSpawnPoint; // 지형 스폰 위치
    public GameObject towerPowerUpForcePrefab; // 타워 강화 프리팹
    public GameObject[] towerPrefabs; // 타워 프리팹 배열
    [SerializeField] private int resolution = 129; // 지형 해상도
    [SerializeField] private float heightScale = 3f; // 높이 스케일
    [SerializeField] private float terrainWidth = 60; // 지형 너비
    [SerializeField] private float terrainLength = 60; // 지형 길이

    private List<GameObject> towerPowerUpForceInstances = new List<GameObject>(); // 타워 강화 인스턴스 리스트

    // 지형 초기화
    public void InitializeTerrain()
    {
        SpawnTerrain();
    }

    // 지형 스폰
    private void SpawnTerrain()
    {
        if (terrain == null)
        {
            return;
        }

        Vector3 spawnPosition = terrainSpawnPoint.position;
        terrain.transform.position = spawnPosition;
        terrain.terrainData = new TerrainData();
        InitializeTerrainData(terrain.terrainData);
        SetTerrainHeights(terrain.terrainData);

        TerrainCollider terrainCollider = terrain.gameObject.GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            terrainCollider = terrain.gameObject.AddComponent<TerrainCollider>();
        }
        terrainCollider.terrainData = terrain.terrainData;
        SpawnTowerPowerUpForces(6);
    }

    // 지형 데이터 초기화
    private void InitializeTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(terrainWidth, heightScale, terrainLength);
    }

    // 지형 높이 설정
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

    // 지형 부드럽게 하기
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

    // 타워 강화 생성
    private void SpawnTowerPowerUpForces(int count)
    {
        if (towerPowerUpForcePrefab == null)
        {
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPosition;
            bool positionIsValid;
            int maxAttempts = 100; // 최대 시도 횟수
            int attempts = 0;

            // 충돌이 없는 랜덤 위치를 찾을 때까지 반복
            do
            {
                float randomX = Random.Range(4, terrainWidth - 4);
                float randomZ = Random.Range(4, terrainLength - 4);
                randomPosition = new Vector3(randomX, 0, randomZ) + terrain.transform.position;

                positionIsValid = true;
                foreach (GameObject existingPowerUp in towerPowerUpForceInstances)
                {
                    if (Vector3.Distance(existingPowerUp.transform.position, randomPosition) < 1f)
                    {
                        positionIsValid = false;
                        break;
                    }
                }

                attempts++;
                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("유효한 위치를 찾지 못했습니다.");
                    break;
                }
            }
            while (!positionIsValid);

            if (positionIsValid)
            {
                GameObject powerUpInstance = Instantiate(towerPowerUpForcePrefab, randomPosition, Quaternion.identity);
                towerPowerUpForceInstances.Add(powerUpInstance);
            }
        }

        PlaceRandomTowers();
    }

    // 랜덤 타워 배치
    private void PlaceRandomTowers()
    {
        foreach (GameObject powerUp in towerPowerUpForceInstances)
        {
            if (powerUp != null)
            {
                int randomIndex = Random.Range(0, towerPrefabs.Length);
                GameObject randomTowerPrefab = towerPrefabs[randomIndex];

                Vector3 towerPosition = new Vector3(powerUp.transform.position.x, 2.2f, powerUp.transform.position.z);
                Quaternion towerRotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

                GameObject newTower = Instantiate(randomTowerPrefab, towerPosition, towerRotation);

                TowerBase towerBase = newTower.GetComponent<TowerBase>();
                if (towerBase != null)
                {
                    towerBase.isAttackUp = true;
                    towerBase.TowerPowUp(); // 타워 파워업 메서드 호출
                    Debug.Log("파워업이 적용되었습니다: " + towerBase.towerAttackPower);
                }
            }
        }
    }


    // 지형 리셋
    public void ResetTerrain()
    {
        if (terrain != null)
        {
            terrain.gameObject.SetActive(false);
            SpawnTerrain();
            terrain.gameObject.SetActive(true);
        }
    }
}
