using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{

    [SerializeField]
    Material[] _Material;
    Transform _thisTower;
    List<Collider> _Colliders;
    [SerializeField] Tower _Tower;
    public void GetRed()//ÀÚ½Ä°´Ã¼ ¸ÞÅ×¸®¾ó »¡°£»öÀ¸·Î ¸¸µê
    {
        _thisTower = transform.GetComponentInChildren<Transform>();
        foreach (Transform t in _thisTower)
        {
            t.gameObject.GetComponent<Renderer>().material = _Material[1];
        }
    }
    public void GetGreen()
    {
        _thisTower = transform.GetComponentInChildren<Transform>();
        foreach (Transform t in _thisTower)
        {
            t.gameObject.GetComponent<Renderer>().material = _Material[0];
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