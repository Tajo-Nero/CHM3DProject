using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class TerrainNavi : MonoBehaviour
{
    public NavMeshSurface terrainNavMeshSurface;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            RebuildTerrainNavMesh();
        }
    }

    void RebuildTerrainNavMesh()
    {
        if (terrainNavMeshSurface != null)
        {
            // 기존 네비매쉬 데이터 제거
            terrainNavMeshSurface.RemoveData();

            // 네비매쉬 다시 베이크
            terrainNavMeshSurface.BuildNavMesh();
            Debug.Log("터레인 네비메쉬가 다시 베이크되었습니다.");
        }
        else
        {
            Debug.LogError("NavMeshSurface가 할당되지 않았습니다.");
        }
    }
}
