using System;
using UnityEngine;

using BugCatcher.Utils;
using BugCatcher.Utils.ObjectPooling;
using BugCatcher.Interfaces;
using BugCatcher.Extensions;

[DefaultExecutionOrder(-9999), DisallowMultipleComponent]
public class PoolSetup 
    : MonoSingle<PoolSetup>
    , ISingleSetup<PoolSetup>
{
    [SerializeField] bool _destroyComponentWhenDone = true;
    
    [SerializeField, TooltipAttribute("Reset all Pools whenever this component reaches OnAwake()")] 
    bool _resetPools = true;

    [SerializeField] PoolSetupArgs[] _args;
    [Serializable] public class PoolSetupArgs
    {
        [SerializeField] GameObject  _prefab = null;
        [SerializeField, Min(1)] int _count  = 1;
    
        public GameObject prefab { get => _prefab; }
        public int        count  { get => _count; }
    }

    public bool Done { get; private set; } = false;

    protected override void OnAwake() => Setup();
    
    public bool Setup()
    {
        if ( Done )
        {
            if ( _resetPools )
            {
                Pool.ClearAll();
                goto SetupPools;
            }

            if ( _destroyComponentWhenDone ) Destroy( this );
            return false;
        }

    SetupPools:
        for ( int i = 0; i < _args.Length; ++i )
        {
            var a = _args[i];
            if ( a.prefab.IsMissingOrNone() )
            {
                Debug.LogWarning( $"[PoolSetup] - Args' Element {i} has no valid prefab, skipping" );
                continue;
            }

            Pool.GetAndFill( a.prefab, a.count );
        }

        Done = true;
        if ( _destroyComponentWhenDone ) Destroy( this );
        return true;
    }
}
