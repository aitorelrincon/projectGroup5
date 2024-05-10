using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using BugCatcher.Utils;
using BugCatcher.Utils.ObjectPooling;
using UnityEditor;

public class ObjectPoolingTests
{
    const int INSTANCES = 50;

    GameObject      prefab;
    Pool            pool;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        prefab  = new( "TEST_ObjPrefab" );
        prefab.AddComponent<Timer>();

        pool    = Pool.GetAndFill( prefab, INSTANCES );
        yield return null;
    }

    [UnityTest]
    public IEnumerator CountTest() 
    {
        Assert.AreNotEqual( pool, null );
        Assert.AreEqual( INSTANCES, pool.TotalCount  );
        Assert.AreEqual( INSTANCES, pool.PooledCount );

        var g = pool.Get();
        var t = pool.Get<Timer>();
        Assert.AreEqual( INSTANCES - 2, pool.PooledCount );

        pool.Return( g );
        Object.Destroy( t );
        yield return new WaitForEndOfFrame();

        Assert.AreEqual( INSTANCES - 1, pool.TotalCount  );
        Assert.AreEqual( INSTANCES - 1, pool.PooledCount );
        yield return null; 
    }
}
