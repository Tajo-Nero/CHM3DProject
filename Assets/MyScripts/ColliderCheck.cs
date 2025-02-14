using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{

    [SerializeField]
    Material[] _Material;
    Transform _Tower;
    public void GetRed()//일단 자식객체 메테리얼 빨간색으로 만듦
    {
        _Tower = transform.GetComponentInChildren<Transform>();
        foreach (Transform t in _Tower)
        {
            t.gameObject.GetComponent<Renderer>().material = _Material[1];
        }
    }
    public void GetGreen()
    {
        _Tower = transform.GetComponentInChildren<Transform>();
        foreach (Transform t in _Tower)
        {
            t.gameObject.GetComponent<Renderer>().material = _Material[0];
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GetRed();
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Towers")
        {
            GetRed();

        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Towers")
        {
            GetGreen();
        }
    }

}