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
            // ���� �׺�Ž� ������ ����
            terrainNavMeshSurface.RemoveData();

            // �׺�Ž� �ٽ� ����ũ
            terrainNavMeshSurface.BuildNavMesh();
            Debug.Log("�ͷ��� �׺�޽��� �ٽ� ����ũ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("NavMeshSurface�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }
}
