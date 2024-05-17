using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BugCatcher.Interfaces;

public class NetScaler 
    : MonoBehaviour
    , INetScaler<NetScaler>
{
    public Transform netRing;
    public Transform netHandle;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void NextScale()
    {
        throw new System.NotImplementedException();
    }

    public void PreviousScale()
    {
        throw new System.NotImplementedException();
    }

    public void SetScale( int i )
    {
        throw new System.NotImplementedException();
    }
}
