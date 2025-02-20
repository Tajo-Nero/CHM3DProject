using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMove : MonoBehaviour
{
    Vector3 moveDir;
    Transform rt;
    public Transform camPos;
    public float camRotSpeed = 1;
    public Camera cam;
    public Transform target;

    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        if (moveDir != Vector3.zero)
        {
            transform.rotation =Quaternion.LookRotation(moveDir);
            transform.Translate(Vector3.forward * Time.deltaTime * 4);
            cam.transform.rotation = transform.rotation;
        }

    }

    void OnMove(InputValue val)
    {
        
        Vector2 dir = val.Get<Vector2>();
        moveDir = new Vector3(dir.x * Time.deltaTime, 0, dir.y * Time.deltaTime);
    }
    private void LateUpdate()
    {
        cam.transform.position = camPos.position;
    }

}
