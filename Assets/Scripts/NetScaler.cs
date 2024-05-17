using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BugCatcher.Interfaces;

public class NetScaler : MonoBehaviour, INetScaler<NetScaler>
{
    public Transform netRing;
    public Transform netHandle;

    private float[] heightScales = new float[] { 1.0f, 1.5f, 2.0f };
    private int currentIndex = 0;
    private float baseHeight = 0.02f; // Adjust this to set the initial height of the handle

    void Start()
    {
        // Initialize the net to the default scale
        SetScale(currentIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            NextScale();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PreviousScale();
        }
    }

    public void NextScale()
    {
        currentIndex = (currentIndex + 1) % heightScales.Length;
        SetScale(currentIndex);
    }

    public void PreviousScale()
    {
        currentIndex = (currentIndex - 1 + heightScales.Length) % heightScales.Length;
        SetScale(currentIndex);
    }

    public void SetScale(int i)
    {
        if (i < 0 || i >= heightScales.Length)
        {
            Debug.LogError("Invalid scale index");
            return;
        }

        float scaleY = heightScales[i] * baseHeight;

        // Set the new scale for the net handle, keeping x and z scale constant
        netHandle.localScale = new Vector3(netHandle.localScale.x, scaleY, netHandle.localScale.z);

        // Here you can adjust the net ring position to be at the top of the handle
        netRing.localPosition = new Vector3(netRing.localPosition.x, netHandle.localScale.y * 5f, netRing.localPosition.z);
    }
}
