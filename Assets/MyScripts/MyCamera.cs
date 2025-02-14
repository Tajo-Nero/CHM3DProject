using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public float _Yaxis;
    public float _Xaxis;

    public Transform target;

    private float rotSnsitive = 3f;
    private float dis = 5f;
    private float RotationMin = -10;
    private float RotationMax = 80f;
    private float smoothTime = 0.12f;

    private Vector3 targetRotation;
    private Vector3 currenVel;

    void LateUpdate()
    {
        _Yaxis = _Yaxis + Input.GetAxis("Mouse X") * rotSnsitive;
        _Xaxis = _Xaxis + Input.GetAxis("Mouse Y") * rotSnsitive;
        _Xaxis = Mathf.Clamp(_Xaxis, RotationMin, RotationMax);
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(_Xaxis, _Yaxis), ref currenVel, smoothTime);
        this.transform.eulerAngles = targetRotation;

        transform.position = target.position - transform.forward * dis;

    }



}
