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
        public string   name;
        public float    secs;
        public uint     score;
    }

    public static Entry InstantiateEntry( string name, float secs, uint score )
        =>new() {
            name = name,
            secs = secs,
            score = score,
        };

    [Serializable]
    public class JsonList<T>
    {
        public List<T> list;
        public JsonList( List<T> list ) => this.list = list;
    }

    static readonly string SAVE_PATH =
        #if UNITY_EDITOR
            ".Assets/Ranking";

#else
            Application.persistentDataPath + "Ranking/";
#endif

    static readonly string SAVE_FILE = SAVE_PATH + "/ranks.json";

    public const int NAME_LENGTH = 10;

    public static readonly List<Entry> DEFAULT_RANKING = new() {
            InstantiateEntry( "P1", GameManager.MAX_TIME,     10000 ),
            InstantiateEntry( "P2", GameManager.MAX_TIME / 2,  5000 ),
            InstantiateEntry( "P3", GameManager.MAX_TIME / 4,  2500 )
        };

    public static List<Entry> ranking { get; private set; }

    static Ranking() => Load();

    public static readonly (int i, Entry entry) SENTINEL_GAME = ( -1, InstantiateEntry( null, -Mathf.Infinity, uint.MinValue ));
    public static (int i, Entry entry) lastGame { get; private set; } = SENTINEL_GAME;
    public static void Load()
    {
        if ( File.Exists( SAVE_FILE ) )
        {
            var r = JsonUtility.FromJson<JsonList<Entry>>(
                    File.ReadAllText(SAVE_FILE)
                );

            ranking = r is null || r.list.Count >= 3
                    ? r.list
                    : new( DEFAULT_RANKING );
        }
        else
        {
            if ( !Directory.Exists( SAVE_PATH ) )
                Directory.CreateDirectory( SAVE_PATH );
        
            File.WriteAllText( SAVE_FILE, JsonUtility.ToJson( new JsonList<Entry>( ranking ), true ) );
        }

        ranking.Sort( ( a, b ) => -a.secs.CompareTo( b.secs ) - a.score.CompareTo( b.score ) );
    }

    public static void CheckEntry( float secs, uint score )
    {
        Load();
        int i = 0; for ( ; i < ranking.Count; i++ )
        {
            if ( secs  > ranking[i].secs
            &&   score > ranking[i].score )
                break;
        }

        // New record
        lastGame = ( i, InstantiateEntry( null, secs, score ) );
        SCManager.Instance.LoadScene( "Ranking" );
    }

    public static bool SaveLastScore( string n )
    {
        if ( n is null || n.Length <= 0 || n.Length > 10 ) 
            return false;

        lastGame.entry.name = n;
        ranking.Insert(
            lastGame.i,
            lastGame.entry
        );

        ranking = ranking.GetRange( 0, 3 );
        File.WriteAllText( SAVE_FILE, JsonUtility.ToJson( new JsonList<Entry>( ranking ), true ) );
        lastGame = SENTINEL_GAME;
        return true;
    }
}