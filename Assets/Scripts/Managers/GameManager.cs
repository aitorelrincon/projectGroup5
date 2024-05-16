using BugCatcher.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoShared<GameManager>
{
    TMP_Text _timeTmp, _scoreTmp;
    float currentTime = 0f;
    int currentScore = 0;

    Timer _timer;

    void Start()
    {
        _timeTmp = GetComponentInChildren<TMP_Text>();
        _scoreTmp = GetComponentInChildren<TMP_Text>();

        if (Camera.main != null)
        {
            // Places the text elements in front of the camera for now
            transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
    }
   
    void UpdateTS()
    {
        // Updates time and score here
        UpdateTime();
       // UpdateScore();
    }

    void UpdateTime()
    {
        currentTime += Time.deltaTime;
        _timeTmp.text = "Time: " + FormatTime(currentTime);
    }

    void UpdateScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        _scoreTmp.text = "Score: " + currentScore.ToString();
    }
    public void IncreaseScore()
    {
        UpdateScore(1); // Increase score by 1
    }

    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, secs);
    }
}