using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;
    Vector3   _ogRot;

    void Start()
        => _ogRot = target.transform.rotation.eulerAngles;

    void Update()
        => transform.LookAt( target );

    void LateUpdate()
        => transform.eulerAngles += _ogRot;
}
