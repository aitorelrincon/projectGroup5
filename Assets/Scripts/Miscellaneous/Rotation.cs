using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [Header("Rotation Config")]
    public bool rotate = true;
    public float rotationSpeed = 45f;

    void Update()
    {
        if ( rotate )
            transform.Rotate( Vector3.up * rotationSpeed * Time.deltaTime, Space.World );
    }
}
