using BugCatcher.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSetup : MonoSingle<PhysicsSetup>
{
    public static bool Done { get; private set; } = false;

    protected override void OnAwake()
    {
        if ( Done )
        {
            Destroy( this );
            return;
        }

        // No one should collide with the skybox
        foreach( var l in Layers.Array )
            Physics.IgnoreLayerCollision( Layers.Skybox, l );

        Done = true;
        Destroy( this );
    }
}
