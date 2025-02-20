using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navi : MonoBehaviour
{
    NavMeshAgent nav;
    [SerializeField] Transform target;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
         nav.SetDestination(target.position);
    }
}
