using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PlaneNaviBake : MonoBehaviour
{
    //[SerializeField] private GameObject plane; // NavMesh를 베이크할 Plane 오브젝트

    //void Update()
    //{
    //    // 숫자 5번 키를 눌렀을 때 NavMesh를 다시 베이크합니다.
    //    if (Input.GetKeyDown(KeyCode.Alpha5))
    //    {
    //        BakeNavMesh();
    //    }
    //}

    //void BakeNavMesh()
    //{
    //    // NavMeshSurface 컴포넌트를 가져와 NavMesh를 베이크합니다.
    //    NavMeshSurface navMeshSurface = plane.GetComponent<NavMeshSurface>();
    //    if (navMeshSurface == null)
    //    {
    //        navMeshSurface = plane.AddComponent<NavMeshSurface>();
    //    }

    //    // Layer Mask를 설정하여 Plane에만 NavMesh를 적용합니다.
    //    navMeshSurface.layerMask = LayerMask.GetMask("NavMesh");

    //    // 기존 NavMesh 데이터 초기화
    //    navMeshSurface.RemoveData();

    //    // NavMesh를 다시 베이크
    //    navMeshSurface.BuildNavMesh();

    //    Debug.Log("Plane에 NavMesh가 다시 베이크되었습니다.");
    //}
    [SerializeField] private GameObject plane; // NavMesh를 베이크할 Plane 오브젝트
    [SerializeField] private Terrain terrain; // Terrain 오브젝트
    void BakeNavMesh()
    {


        void Update()
        {
            // 숫자 5번 키를 눌렀을 때 NavMesh를 다시 베이크합니다.
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                BakeNavMesh();
            }
        }

        void BakeNavMesh()
        {
            // Plane의 NavMeshSurface 컴포넌트를 가져옵니다.
            NavMeshSurface navMeshSurface = plane.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = plane.AddComponent<NavMeshSurface>();
            }

            // 기존 NavMesh 데이터 초기화
            navMeshSurface.RemoveData();

            // NavMeshData를 설정하고 초기화합니다.
            NavMeshData navMeshData = new NavMeshData();
            navMeshSurface.navMeshData = navMeshData;

            // NavMesh를 다시 베이크합니다.
            navMeshSurface.BuildNavMesh();

            Debug.Log("Plane에 NavMesh가 다시 베이크되었습니다.");


        }
        }
    }