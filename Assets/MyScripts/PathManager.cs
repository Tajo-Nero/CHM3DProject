using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    // 메인 경로 저장
    private List<Vector3> mainPath = new List<Vector3>();

    // 경로 시각화 설정
    public bool visualizePath = true;
    public float pathWidth = 0.2f;
    public Color pathColor = Color.blue;

    // 라인 렌더러 참조
    private LineRenderer pathRenderer;

    void Awake()
    {
        // 라인 렌더러 초기화
        pathRenderer = GetComponent<LineRenderer>();
        if (pathRenderer == null && visualizePath)
        {
            pathRenderer = gameObject.AddComponent<LineRenderer>();
            pathRenderer.startWidth = pathWidth;
            pathRenderer.endWidth = pathWidth;
            pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
            pathRenderer.startColor = pathColor;
            pathRenderer.endColor = pathColor;
        }
    }

    // 경로 설정 함수 (플레이어 카 모드에서 호출)
    public void SetMainPath(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count < 2)
        {
            Debug.LogWarning("유효하지 않은 경로입니다!");
            return;
        }

        mainPath = new List<Vector3>(newPath);
        Debug.Log($"새 경로 설정: {mainPath.Count}개 지점");

        // 경로 시각화 업데이트
        UpdatePathVisualization();
    }

    // 경로 시각화 업데이트
    private void UpdatePathVisualization()
    {
        if (!visualizePath || pathRenderer == null || mainPath.Count < 2)
            return;

        pathRenderer.positionCount = mainPath.Count;
        for (int i = 0; i < mainPath.Count; i++)
        {
            pathRenderer.SetPosition(i, mainPath[i]);
        }
    }
    // PathManager.cs에 추가
    public bool HasValidPath()
    {
        // 경로가 유효한지 확인하는 로직
        return (mainPath != null && mainPath.Count >= 2);
    }

    // 경로 가져오기 함수 (WaveManager에서 호출)
    public List<Vector3> GetMainPath()
    {
        // 경로가 비어있으면 기본 경로 생성
        if (mainPath == null || mainPath.Count < 2)
        {
            return GenerateDefaultPath();
        }

        return new List<Vector3>(mainPath);
    }

    // 기본 경로 생성
    private List<Vector3> GenerateDefaultPath()
    {
        List<Vector3> defaultPath = new List<Vector3>();

        // 스폰 포인트를 찾아 시작점으로 사용
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawn");
        if (spawnPoints.Length > 0)
        {
            defaultPath.Add(spawnPoints[0].transform.position);
        }
        else
        {
            defaultPath.Add(new Vector3(-10, 0, -10)); // 기본 시작점
        }

        // 넥서스를 찾아 목적지로 사용
        Nexus nexus = FindObjectOfType<Nexus>();
        if (nexus != null)
        {
            defaultPath.Add(nexus.transform.position);
        }
        else
        {
            defaultPath.Add(new Vector3(10, 0, 10)); // 기본 목적지
        }

        Debug.LogWarning("기본 경로 생성됨. 플레이어 카 모드로 경로를 생성하세요!");
        return defaultPath;
    }

    // 경로 지우기
    public void ClearPath()
    {
        mainPath.Clear();

        if (pathRenderer != null)
        {
            pathRenderer.positionCount = 0;
        }
    }
}