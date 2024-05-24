using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class InteractionHandlerScript : MonoBehaviour,
    IMixedRealityInputActionHandler,
    IMixedRealityFocusHandler
{
    private bool isFocused = false;

    private void OnEnable()
    {
        CoreServices.InputSystem.RegisterHandler<IMixedRealityInputActionHandler>( this );
        CoreServices.InputSystem.RegisterHandler<IMixedRealityFocusHandler>( this );
    }

    private void OnDisable()
    {
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityInputActionHandler>( this );
        CoreServices.InputSystem.UnregisterHandler<IMixedRealityFocusHandler>( this );
    }

    void IMixedRealityInputActionHandler.OnActionStarted( BaseInputEventData eventData )
    {
        if ( !isFocused )
        {
            return;
        }

        if ( eventData.MixedRealityInputAction.Description == "Select" )
        {
            gameObject.GetComponent<SimpleCollectibleScript>().Collect();
        }
    }

    void IMixedRealityInputActionHandler.OnActionEnded( BaseInputEventData eventData ) { }

    void IMixedRealityFocusHandler.OnFocusEnter( FocusEventData eventData )
    {
        isFocused = eventData.NewFocusedObject.GetHashCode() == gameObject.GetHashCode();
    }

    void IMixedRealityFocusHandler.OnFocusExit( FocusEventData eventData )
    {
        isFocused = false;
    }
}
