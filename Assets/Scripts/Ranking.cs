using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ranking
{
    [Serializable]
    public class Entry
    {
        public string name;
        public float secs;
    }


    [System.Serializable]
    public class JsonList<T>
    {
        public List<T> list;
        public JsonList( List<T> list ) => this.list = list;
    }

    static readonly string SAVE_PATH =
        #if UNITY_EDITOR
            "./Ranking";

#else
            Application.persistentDataPath + "Ranking/";
#endif

    static readonly string SAVE_FILE = SAVE_PATH + "/ranks.json";

    public const int NAME_LENGTH = 10;

    public static readonly List<Entry> DEFAULT_RANKING = new() {
            new() { secs =  20f, name = "P1" },
            new() { secs =  30f, name = "P2" },
            new() { secs = 418f, name = "P3" }
        };

    public static List<Entry> ranking { get; private set; }

    static Ranking() => Load();

    public static (int i, float s) lastSecs { get; private set; } = (-1, Mathf.Infinity);
    public static void Load()
    {
        if ( File.Exists( SAVE_FILE ) )
        {
            var r = JsonUtility.FromJson<JsonList<Entry>>(
                    File.ReadAllText(SAVE_FILE)
                );

            ranking = r.list.Count >= 3
                    ? r.list
                    : new( DEFAULT_RANKING );
        }
        else
        {
            if ( !Directory.Exists( SAVE_PATH ) )
                Directory.CreateDirectory( SAVE_PATH );

            File.WriteAllText( SAVE_FILE, JsonUtility.ToJson( ranking, true ) );
        }

        ranking.Sort( ( a, b ) => a.secs.CompareTo( b.secs ) );
    }

    public static void CheckEntry( float secs )
    {
        Load();
        int i = 0; for ( ; i < ranking.Count; i++ )
        {
            if ( secs < ranking[i].secs )
                break;
        }

        // New record
        lastSecs = (i, secs);
        SCManager.Instance.LoadScene(
            i < ranking.Count
            ? "Entry"
            : "Ranking"
        );
    }

    public static void SaveLastScore( string n )
    {
        ranking.Insert(
            lastSecs.i,
            new() { name = n, secs = lastSecs.s }
        );

        ranking = ranking.GetRange( 0, 3 );
        File.WriteAllText( SAVE_FILE, JsonUtility.ToJson( new JsonList<Entry>( ranking ), true ) );
    }
}