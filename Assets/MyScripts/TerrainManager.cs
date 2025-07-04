using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // ���� ��ü
    public Transform terrainSpawnPoint; // ���� ���� ��ġ
    public GameObject towerPowerUpForcePrefab; // Ÿ�� ��ȭ ������
    public GameObject[] towerPrefabs; // Ÿ�� ������ �迭
    [SerializeField] private int resolution = 129; // ���� �ػ�
    [SerializeField] private float heightScale = 3f; // ���� ������
    [SerializeField] private float terrainWidth = 60; // ���� �ʺ�
    [SerializeField] private float terrainLength = 60; // ���� ����

    private List<GameObject> towerPowerUpForceInstances = new List<GameObject>(); // Ÿ�� ��ȭ �ν��Ͻ� ����Ʈ

    // ���� �ʱ�ȭ
    public void InitializeTerrain()
    {
        Debug.Log("InitializeTerrain ȣ���!");
        SpawnTerrain();
    }

    // ���� ������ �ʱ�ȭ
    private void InitializeTerrainData(TerrainData terrainData)
    {
        terrainData.heightmapResolution = resolution;
        terrainData.size = new Vector3(terrainWidth, heightScale, terrainLength);
    }
    // ���� ����
    private void SpawnTerrain()
    {
        Debug.Log("=== SpawnTerrain ���� ===");
        Debug.Log($"terrain null ����: {terrain == null}");
        Debug.Log($"terrainSpawnPoint null ����: {terrainSpawnPoint == null}");

        if (terrain == null)
        {
            Debug.LogError("terrain�� null�Դϴ�!");
            return;
        }

        if (terrainSpawnPoint == null)
        {
            Debug.LogError("terrainSpawnPoint�� null�Դϴ�!");
            return;
        }

        Vector3 spawnPosition = terrainSpawnPoint.position;
        Debug.Log($"���� ��ġ: {spawnPosition}");

        terrain.transform.position = spawnPosition;
        Debug.Log("�ͷ��� ��ġ ���� �Ϸ�");

        // TerrainData�� ���� ���� ���� ���� (�ߺ� ���� ����!)
        if (terrain.terrainData == null)
        {
            terrain.terrainData = new TerrainData();
            Debug.Log("���ο� TerrainData ����!");
        }

        InitializeTerrainData(terrain.terrainData);
        Debug.Log("TerrainData �ʱ�ȭ �Ϸ�");

        SetTerrainHeights(terrain.terrainData);
        Debug.Log("�ͷ��� ���� ���� �Ϸ�");

        TerrainCollider terrainCollider = terrain.gameObject.GetComponent<TerrainCollider>();
        if (terrainCollider == null)
        {
            terrainCollider = terrain.gameObject.AddComponent<TerrainCollider>();
            Debug.Log("TerrainCollider �߰���");
        }
        terrainCollider.terrainData = terrain.terrainData;
        Debug.Log("TerrainCollider ���� �Ϸ�");

        SpawnTowerPowerUpForces(6);
        Debug.Log("=== SpawnTerrain �Ϸ� ===");
    }


    // ���� ���� ����
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

    // ���� �ε巴�� �ϱ�
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

    // Ÿ�� ��ȭ ����
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
            int maxAttempts = 100; // �ִ� �õ� Ƚ��
            int attempts = 0;

            // �浹�� ���� ���� ��ġ�� ã�� ������ �ݺ�
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
                    Debug.LogWarning("��ȿ�� ��ġ�� ã�� ���߽��ϴ�.");
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

    // ���� Ÿ�� ��ġ
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
                    towerBase.TowerPowUp(); // Ÿ�� �Ŀ��� �޼��� ȣ��
                    Debug.Log("�Ŀ����� ����Ǿ����ϴ�: " + towerBase.towerAttackPower);
                }
            }
        }
    }


    // ���� ����
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
