using BugCatcher.Interfaces;
using BugCatcher.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder( -9999 ), DisallowMultipleComponent]
public class PhysicsSetup 
    : MonoSingle<PhysicsSetup>
    , ISingleSetup<PhysicsSetup>
{
    public bool Done { get; private set; } = false;

    protected override void OnAwake() => Setup();

    public bool Setup()
    {
        if ( Done )
        {
            Destroy( this );
            return false;
        }



        // Removing the collider is from the skybox is enough,
        // but this *ENSURES* no one ever collides with it.
        foreach ( var l in Layers.Array )
            Physics.IgnoreLayerCollision( Layers.Skybox, l );

        Done = true;
        Destroy( this );
        return true;
    }
}
