// #define ORIGINAL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BugCatcher.Interfaces;
using BugCatcher.Extensions;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

public class NetScaler : MonoBehaviour, INetScaler<NetScaler>, IMixedRealityInputHandler
{
    public Transform netRing;
    public Transform netHandle;

    private float[] heightScales = new float[] { 1.0f, 1.5f, 2.0f };
    private int   currentIndex = 0;
#if ORIGINAL
    private float baseHeight = 0.02f; // Adjust this to set the initial height of the handle
    private float baseY;
#else
    Vector2 baseHandle;
    float   baseRingY;
#endif

    void Start()
    {
        baseHandle[0]   = netHandle.localScale.y;
        baseHandle[1]   = netHandle.localPosition.y;
        baseRingY       = netRing.localPosition.y;

        // Initialize the net to the default scale
        SetScale(currentIndex);
    }

#if UNITY_EDITOR
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    NextScale();
        //}
        //if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    PreviousScale();
        //}
    }
#endif

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


#if ORIGINAL
        float scaleY = heightScales[i] * baseHandle[0];
        // Set the new scale for the net handle, keeping x and z scale constant
        netHandle.localScale = new Vector3(netHandle.localScale.x, scaleY, netHandle.localScale.z);

        // Here you can adjust the net ring position to be at the top of the handle
        netRing.localPosition = new Vector3(netRing.localPosition.x, netHandle.localScale.y * 5f, netRing.localPosition.z);
#else
        netHandle.localScale    = netHandle.localScale.WithZ( heightScales[i] );
        netHandle.localPosition = netHandle.localPosition.WithY( baseHandle[1] * heightScales[i]  );
        netRing.localPosition   = netRing.localPosition.WithY( baseRingY * heightScales[i] );
#endif
    }

    public void OnInputUp(InputEventData eventData)
    {
        if (eventData.MixedRealityInputAction.Description == "Trigger" && eventData.Handedness == Handedness.Right)
        {
            NextScale();
        }

        if (eventData.MixedRealityInputAction.Description == "Grip Press" && eventData.Handedness == Handedness.Right)
        {
            PreviousScale();
        }
    }

    public void OnInputDown(InputEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
