using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    [SerializeField]
    private float _maxDistance = 1.0f;
    [SerializeField]
    private Color _ryrColor = Color.red;
    void OnDrawGizmos()
    {
        Gizmos.color = _ryrColor;
        float sphereScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y+1, transform.lossyScale.z);

        if (true == Physics.SphereCast(new Vector3(transform.position.x,transform.position.y+1,transform.position.z), sphereScale / 2.0f, transform.forward, out RaycastHit hit, _maxDistance))
        {
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            Gizmos.DrawWireSphere(transform.position + transform.forward * hit.distance, sphereScale / 2.0f);
        }
        else
        {
            Gizmos.DrawRay(transform.position, transform.forward * _maxDistance);
        }
    }


}
