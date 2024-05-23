using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void StartGame()
    {
        SCManager.Instance.LoadScene("Game");
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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
