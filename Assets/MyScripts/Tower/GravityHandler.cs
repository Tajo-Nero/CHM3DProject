//using System.Collections.Generic;
//using UnityEngine;
//public interface IGravityHandler
//{
//    void ApplyGravity(Collider[] colliders);
//    void RemoveGravity(Collider[] colliders);
//}

//public class GravityHandler : MonoBehaviour
//{
//    public void ApplyGravity(Collider[] colliders)
//    {
//        foreach (Collider collider in colliders)
//        {
//            Rigidbody rb = collider.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                rb.useGravity = true;
//                rb.isKinematic = false;
//                Debug.Log("중력 적용: " + rb.gameObject.name);
//            }
//        }
//    }

//    public void RemoveGravity(Collider[] colliders)
//    {
//        foreach (Collider collider in colliders)
//        {
//            Rigidbody rb = collider.GetComponent<Rigidbody>();
//            if (rb != null)
//            {
//                rb.useGravity = false;
//                rb.isKinematic = true;
//                Debug.Log("중력 해제: " + rb.gameObject.name);
//            }
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, 0.5f);
//    }
//}
