using System.Collections.Generic;
using UnityEngine;

public class AutoWaypointGenerator : MonoBehaviour
{
    [Header("Path Settings")]
    public Transform startPoint; // 적 스폰 지점
    public Transform endPoint;   // 넥서스 위치
    public float scanRadius = 2f;
    public float heightThreshold = 0.8f; // 이 높이 이하만 길로 인식
    public float waypointSpacing = 3f;   // 웨이포인트 간격

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
            Debug.LogError("필요한 컴포넌트가 없습니다!");
            return new List<Vector3>();
        }

        generatedWaypoints.Clear();

        Vector3 current = startPoint.position;
        Vector3 target = endPoint.position;

        // 시작점 추가
        generatedWaypoints.Add(current);

        int maxIterations = 100; // 무한루프 방지
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
                Debug.LogWarning("경로를 찾을 수 없습니다!");
                break;
            }

            iterations++;
        }

        // 목표점 추가
        generatedWaypoints.Add(target);

        Debug.Log($"웨이포인트 {generatedWaypoints.Count}개 생성 완료!");
        return new List<Vector3>(generatedWaypoints);
    }

    Vector3 FindBestNextPoint(Vector3 current, Vector3 target)
    {
        Vector3 bestPoint = Vector3.zero;
        float bestScore = float.MinValue;

        // 목표 방향 계산
        Vector3 directionToTarget = (target - current).normalized;

        // 여러 각도에서 검사 (-60도 ~ +60도)
        for (int angle = -60; angle <= 60; angle += 10)
        {
            Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.up) * directionToTarget;
            Vector3 testPoint = current + rotatedDirection * waypointSpacing;

            // 터레인 경계 체크
            if (!IsPointInTerrain(testPoint)) continue;

            // 터레인 높이 가져오기
            float height = GetTerrainHeight(testPoint);
            testPoint.y = height;

            // 이 지점이 "길"인지 확인 (높이가 낮으면 파진 길)
            if (height < heightThreshold)
            {
                // 점수 계산 (목표에 가까울수록, 직선에 가까울수록 높은 점수)
                float distanceToTarget = Vector3.Distance(testPoint, target);
                float angleScore = 1f - (Mathf.Abs(angle) / 60f); // 직선일수록 높은 점수

                float score = (1000f / (distanceToTarget + 1f)) + angleScore * 100f;

                // 주변이 더 깊게 파져있으면 추가 점수
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

    // 디버그용 경로 시각화
    void OnDrawGizmos()
    {
        if (!showDebugPath || generatedWaypoints.Count < 2) return;

        Gizmos.color = pathColor;

        for (int i = 0; i < generatedWaypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(generatedWaypoints[i], generatedWaypoints[i + 1]);
            Gizmos.DrawWireSphere(generatedWaypoints[i], 0.5f);
        }

        // 마지막 웨이포인트
        if (generatedWaypoints.Count > 0)
        {
            Gizmos.DrawWireSphere(generatedWaypoints[generatedWaypoints.Count - 1], 0.5f);
        }
    }
}