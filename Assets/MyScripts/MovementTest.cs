using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovementTest : MonoBehaviour
{
    public GameObject target;  // Ÿ������ ������ Transform
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();  // NavMeshAgent ������Ʈ ��������

    }
    void Update()
    {
        
            navMeshAgent.SetDestination(target.transform.position);  // Ÿ�� ��ġ�� �̵� ����
        
    }
}
