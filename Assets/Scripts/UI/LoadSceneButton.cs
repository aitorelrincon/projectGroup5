using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Automatically makes the Button load a scene.
/// </summary>
[RequireComponent(typeof(Button))]
public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] string sceneName;
    
    Button _button;
    void OnAwake()   => _button = GetComponent<Button>();
    void OnEnable()  => _button.onClick.AddListener( Load );
    void OnDisable() => _button.onClick.RemoveListener( Load );

    void Load() => SCManager.Instance.LoadScene( sceneName );
}
