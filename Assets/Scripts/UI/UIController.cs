using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("test");
        SCManager.Instance.LoadScene("Game");
    }
}
