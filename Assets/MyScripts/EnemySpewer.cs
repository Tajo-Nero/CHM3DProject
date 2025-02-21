using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpewer : MonoBehaviour
{
    public GameObject Enemy;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(Enemy, transform.position, transform.rotation);
        }
    }
}
