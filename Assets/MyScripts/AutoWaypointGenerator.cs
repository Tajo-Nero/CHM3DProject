using System.Collections.Generic;
using UnityEngine;

public class AutoWaypointGenerator : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform startPoint; // �� ���� ����
    public Transform endPoint;   // �ؼ��� ��ġ
    public float scanRadius = 2f;
    public float heightThreshold = 0.8f; // �� ���� ���ϸ� ��� �ν�
    public float waypointSpacing = 3f;   // ��������Ʈ ����

    [Header("Debug")]
    public bool showDebugPath = true;
    public Color pathColor = Color.green;

    private List<Vector3> generatedWaypoints = new List<Vector3>();
    private Terrain terrain;

    void Start()
    {
        terrain = FindObjectOfType<Terrain>();
        if (endPoint == null)
        {
            GameObject nexus = FindObjectOfType<Nexus>()?.gameObject;
            if (nexus != null) endPoint = nexus.transform;
        }
    }

    public List<Vector3> GenerateWaypoints()
    {
        if (terrain == null || startPoint == null || endPoint == null)
        {
            Debug.LogError("�ʿ��� ������Ʈ�� �����ϴ�!");
            return new List<Vector3>();
        }

        generatedWaypoints.Clear();

        Vector3 current = startPoint.position;
        Vector3 target = endPoint.position;

        // ������ �߰�
        generatedWaypoints.Add(current);

        int maxIterations = 100; // ���ѷ��� ����
        int iterations = 0;

        while (Vector3.Distance(current, target) > waypointSpacing && iterations < maxIterations)
        {
            Vector3 nextPoint = FindBestNextPoint(current, target);

            if (nextPoint != Vector3.zero)
            {
                generatedWaypoints.Add(nextPoint);
                current = nextPoint;
            }
            else
            {
                Debug.LogWarning("��θ� ã�� �� �����ϴ�!");
                break;
            }

            iterations++;
        }

        // ��ǥ�� �߰�
        generatedWaypoints.Add(target);

        Debug.Log($"��������Ʈ {generatedWaypoints.Count}�� ���� �Ϸ�!");
        return new List<Vector3>(generatedWaypoints);
    }

    Vector3 FindBestNextPoint(Vector3 current, Vector3 target)
    {
        Vector3 bestPoint = Vector3.zero;
        float bestScore = float.MinValue;

        // ��ǥ ���� ���
        Vector3 directionToTarget = (target - current).normalized;

        // ���� �������� �˻� (-60�� ~ +60��)
        for (int angle = -60; angle <= 60; angle += 10)
        {
            Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.up) * directionToTarget;
            Vector3 testPoint = current + rotatedDirection * waypointSpacing;

            // �ͷ��� ��� üũ
            if (!IsPointInTerrain(testPoint)) continue;

            // �ͷ��� ���� ��������
            float height = GetTerrainHeight(testPoint);
            testPoint.y = height;

            // �� ������ "��"���� Ȯ�� (���̰� ������ ���� ��)
            if (height < heightThreshold)
            {
                // ���� ��� (��ǥ�� ��������, ������ �������� ���� ����)
                float distanceToTarget = Vector3.Distance(testPoint, target);
                float angleScore = 1f - (Mathf.Abs(angle) / 60f); // �����ϼ��� ���� ����

                float score = (1000f / (distanceToTarget + 1f)) + angleScore * 100f;

                // �ֺ��� �� ��� ���������� �߰� ����
                float depthBonus = (heightThreshold - height) * 50f;
                score += depthBonus;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPoint = testPoint;
                }
            }
        }

        return bestPoint;
    }

    bool IsPointInTerrain(Vector3 point)
    {
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        return point.x >= terrainPos.x && point.x <= terrainPos.x + terrainSize.x &&
               point.z >= terrainPos.z && point.z <= terrainPos.z + terrainSize.z;
    }

    float GetTerrainHeight(Vector3 worldPos)
    {
        Vector3 terrainPos = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;

        float normalizedX = (worldPos.x - terrainPos.x) / terrainSize.x;
        float normalizedZ = (worldPos.z - terrainPos.z) / terrainSize.z;

        float height = terrain.terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
        return height + terrainPos.y;
    }

    public List<Vector3> GetCurrentWaypoints()
    {
        return new List<Vector3>(generatedWaypoints);
    }

    // ����׿� ��� �ð�ȭ
    void OnDrawGizmos()
    {
        if (!showDebugPath || generatedWaypoints.Count < 2) return;

        Gizmos.color = pathColor;

        for (int i = 0; i < generatedWaypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(generatedWaypoints[i], generatedWaypoints[i + 1]);
            Gizmos.DrawWireSphere(generatedWaypoints[i], 0.5f);
        }

        // ������ ��������Ʈ
        if (generatedWaypoints.Count > 0)
        {
            Gizmos.DrawWireSphere(generatedWaypoints[generatedWaypoints.Count - 1], 0.5f);
        }
    }
}