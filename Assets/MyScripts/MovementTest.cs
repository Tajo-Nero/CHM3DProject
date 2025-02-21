using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementTest : MonoBehaviour
{
    public Transform target;  // Ÿ������ ������ Transform
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();  // NavMeshAgent ������Ʈ ��������
    }

    void Update()
    {
        if (target != null&& Input.GetMouseButtonDown(1))
        {
            navMeshAgent.SetDestination(target.position);  // Ÿ�� ��ġ�� �̵� ����
        }
    }
}
