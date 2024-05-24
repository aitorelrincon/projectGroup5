using BugCatcher.Utils;
using BugCatcher.Extensions.Functional;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoSingle<UIController>
{
#if DEBUG
    void Update()
    {
        if ( Input.GetKeyDown( KeyCode.Alpha1 ) ) StartGame();
        if ( Input.GetKeyDown( KeyCode.Alpha2 ) ) Settings();
        if ( Input.GetKeyDown( KeyCode.Alpha3 ) ) LeaveGame();
        if ( Input.GetKeyDown( KeyCode.Escape ) ) ExitGame();
    }
#endif

    public void StartGame()
    {
        if ( SceneManager.GetActiveScene().name == "Main Menu" )
            SCManager.Instance.TeeLog().LoadScene("Game");
    }

    public void LeaveGame()
    {
        if ( SceneManager.GetActiveScene().name == "Game" )
            SCManager.Instance.LoadScene("Main Menu");
    }

    public void Settings()
    {
        if ( SceneManager.GetActiveScene().name == "Main Menu" )
            SCManager.Instance.LoadScene("Settings");
    }

    public void GoToRanking()
    {
        if ( SceneManager.GetActiveScene().name == "Main Menu" )
            SCManager.Instance.LoadScene( "Ranking" );
    }

    public void ExitGame()
    {
        if ( SceneManager.GetActiveScene().name == "Main Menu" )
            SCManager.Instance.Exit();
    }

    public void Save()
    {
        Debug.Log( "[UIController] - Speech:Save detected" );
        switch ( SceneManager.GetActiveScene().name )
        {
            case "Ranking":
                var updated = RankingUI.Instance.SaveEntry();
                Ranking.Entry l = Ranking.lastGame.entry, r = Ranking.lastGame.entry;
                if ( !updated
                &&   l.name  == r.name
                &&   l.secs  == r.secs
                &&   l.score == r.score )
                    SCManager.Instance.LoadScene( "Main Menu" );
                break;

            case "Settings":
                AudioManager.Instance.SavePrefs();
                SCManager.Instance.LoadScene( "Main Menu" );
                break;
        }
    }
}
