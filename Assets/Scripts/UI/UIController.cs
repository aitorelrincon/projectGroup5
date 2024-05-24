using BugCatcher.Utils;
using BugCatcher.Extensions.Functional;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        SCManager.Instance.TeeLog().LoadScene("Game");
    }

    public void LeaveGame()
    {
        SCManager.Instance.LoadScene("Main Menu");
    }

    public void Settings()
    {
        SCManager.Instance.LoadScene("Settings");
    }

    public void ExitGame()
    {
        SCManager.Instance.Exit();
    }
}
