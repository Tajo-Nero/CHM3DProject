using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlaneNaviBake : MonoBehaviour
{
    //[SerializeField] private GameObject plane; // NavMesh�� ����ũ�� Plane ������Ʈ

    //void Update()
    //{
    //    // ���� 5�� Ű�� ������ �� NavMesh�� �ٽ� ����ũ�մϴ�.
    //    if (Input.GetKeyDown(KeyCode.Alpha5))
    //    {
    //        BakeNavMesh();
    //    }
    //}

    //void BakeNavMesh()
    //{
    //    // NavMeshSurface ������Ʈ�� ������ NavMesh�� ����ũ�մϴ�.
    //    NavMeshSurface navMeshSurface = plane.GetComponent<NavMeshSurface>();
    //    if (navMeshSurface == null)
    //    {
    //        navMeshSurface = plane.AddComponent<NavMeshSurface>();
    //    }

    //    // Layer Mask�� �����Ͽ� Plane���� NavMesh�� �����մϴ�.
    //    navMeshSurface.layerMask = LayerMask.GetMask("NavMesh");

    //    // ���� NavMesh ������ �ʱ�ȭ
    //    navMeshSurface.RemoveData();

    //    // NavMesh�� �ٽ� ����ũ
    //    navMeshSurface.BuildNavMesh();

    //    Debug.Log("Plane�� NavMesh�� �ٽ� ����ũ�Ǿ����ϴ�.");
    //}
    [SerializeField] private GameObject plane; // NavMesh�� ����ũ�� Plane ������Ʈ
    [SerializeField] private Terrain terrain; // Terrain ������Ʈ
    void BakeNavMesh()
    {


        void Update()
        {
            // ���� 5�� Ű�� ������ �� NavMesh�� �ٽ� ����ũ�մϴ�.
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                BakeNavMesh();
            }
        }

        void BakeNavMesh()
        {
            // Plane�� NavMeshSurface ������Ʈ�� �����ɴϴ�.
            NavMeshSurface navMeshSurface = plane.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = plane.AddComponent<NavMeshSurface>();
            }

            // ���� NavMesh ������ �ʱ�ȭ
            navMeshSurface.RemoveData();

            // NavMeshData�� �����ϰ� �ʱ�ȭ�մϴ�.
            NavMeshData navMeshData = new NavMeshData();
            navMeshSurface.navMeshData = navMeshData;

            // NavMesh�� �ٽ� ����ũ�մϴ�.
            navMeshSurface.BuildNavMesh();

            Debug.Log("Plane�� NavMesh�� �ٽ� ����ũ�Ǿ����ϴ�.");


        }
        }
    }