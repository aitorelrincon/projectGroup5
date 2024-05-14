#define MPT_DEBUG

using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof( Terrain ))]
public class MassPlaceTrees : MonoBehaviour
{
    #region Enums
    public enum MassPlaceMode { Awake, Start, Manual }
    #endregion

    #region Static properties
    public static readonly string Header = BC_Utils.Header<MassPlaceTrees>();
    #endregion

    #region Properties
    public int count { 
        get => _count;
        set => _count = Mathf.Max( value, 0 );
    }
    #endregion

    #region Mass Place Config
    [Header("Mass Place Config")]
    [SerializeField, Min(0)] int _count;
    
    public MassPlaceMode massPlaceMode = MassPlaceMode.Awake;
    #endregion

    #region Private members
    Terrain _terrain;
    #endregion

    #region Unity methods
    void Awake()
    {
        _terrain = GetComponent<Terrain>();
        WarnZero();
        
        if ( massPlaceMode == MassPlaceMode.Awake )
            MassPlace();
    }

    void Start()
    {
        if ( massPlaceMode == MassPlaceMode.Start )
            MassPlace();
    }
    #endregion

    #region Public methods
    public void MassPlace()
    {
        if ( count == 0 ) return;

        var trees = new TreeInstance[ _count ];
        var tdata = _terrain.terrainData;

        // Done
        tdata.SetTreeInstances( trees, true );
        _terrain.terrainData = tdata;
    }
    #endregion

    #region Private methods
    [Conditional("MPT_DEBUG")]
    void WarnZero()
    {
        if ( count == 0 )
            UnityEngine.Debug.LogWarning( Header + "Chance set to 0%, trees won't be placed" );
    }
    #endregion
}
