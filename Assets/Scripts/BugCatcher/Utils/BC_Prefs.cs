using System;
using UnityEngine;

public static class BC_Prefs
{
    public static float GetFloat( string key, float failsafe )
        => PlayerPrefs.HasKey( key )
         ? PlayerPrefs.GetFloat( key )
         : failsafe;

    public static bool GetBool32( string key, bool failsafe )
        => PlayerPrefs.HasKey( key )
         ? Convert.ToBoolean( PlayerPrefs.GetInt( key ) )
         : failsafe;

    public static void SetBool32( string key, bool value )
        => PlayerPrefs.SetInt( key, Convert.ToInt32( value ) );
}
