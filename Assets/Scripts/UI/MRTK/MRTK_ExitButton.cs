using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using BugCatcher.Extensions.Functional;

/// <summary>
/// Automatically makes the Interactable quit the game.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class MRTK_ExitButton : MonoBehaviour
{
    Interactable _interactable;
    void Awake()   => _interactable = GetComponent<Interactable>().Tee( (i) => Debug.Log(i) );
    void OnEnable()  => _interactable.OnClick.AddListener( SCManager.Instance.Exit );
    void OnDisable() => _interactable.OnClick.RemoveListener( SCManager.Instance.Exit );
}
