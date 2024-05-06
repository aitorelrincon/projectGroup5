using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// Automatically makes the Interactable load a scene.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class MRTK_LoadSceneButton : MonoBehaviour
{
    [SerializeField] string sceneName;

    Interactable _interactable;
    void OnAwake()   => _interactable = GetComponent<Interactable>();
    void OnEnable()  => _interactable.OnClick.AddListener( Load );
    void OnDisable() => _interactable.OnClick.RemoveListener( Load );

    void Load() => SCManager.Instance.LoadScene( sceneName );
}
