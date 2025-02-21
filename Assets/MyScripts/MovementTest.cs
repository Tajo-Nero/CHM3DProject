using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementTest : MonoBehaviour
{
    public Transform target;  // 타겟으로 설정할 Transform
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();  // NavMeshAgent 컴포넌트 가져오기
    }

    void Update()
    {
        if (target != null&& Input.GetMouseButtonDown(1))
        {
            navMeshAgent.SetDestination(target.position);  // 타겟 위치로 이동 설정
        }
    }
}
