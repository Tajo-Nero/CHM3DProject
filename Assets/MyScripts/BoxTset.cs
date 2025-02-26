using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCastTest : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform boxCastStartPoint;
    [SerializeField] private Vector3 boxHalfExtents = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private LayerMask targetLayer;

    void Update()
    {
        DetectObjectsInRange();
    }

    private void DetectObjectsInRange()
    {
        Vector3 boxCenter = boxCastStartPoint.position + boxCastStartPoint.forward * (detectionRange / 2);
        Vector3 boxSize = new Vector3(boxHalfExtents.x * 2, boxHalfExtents.y * 2, detectionRange);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxHalfExtents, boxCastStartPoint.rotation, targetLayer);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("박스 캐스트로 감지된 객체: " + hitCollider.transform.name);
        }

        // 박스 캐스트 시각화
        Debug.DrawRay(boxCastStartPoint.position, boxCastStartPoint.forward * detectionRange, Color.red, 0.1f);
    }

    private void OnDrawGizmos()
    {
        if (boxCastStartPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 boxCenter = boxCastStartPoint.position + boxCastStartPoint.forward * (detectionRange / 2);
            Vector3 boxSize = new Vector3(boxHalfExtents.x * 2, boxHalfExtents.y * 2, detectionRange);
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, boxCastStartPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}
