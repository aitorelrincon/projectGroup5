using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// Automatically makes the Interactable quit the game.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class MRTK_ExitButton : MonoBehaviour
{
    Interactable _interactable;
    void OnAwake()   => _interactable = GetComponent<Interactable>();
    void OnEnable()  => _interactable.OnClick.AddListener( SCManager.Instance.Exit );
    void OnDisable() => _interactable.OnClick.RemoveListener( SCManager.Instance.Exit );
}
