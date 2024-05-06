using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Automatically makes the Button quit the game.
/// </summary>
[RequireComponent(typeof(Button))]
public class ExitButton : MonoBehaviour
{
    Button _button;
    void OnAwake()   => _button = GetComponent<Button>();
    void OnEnable()  => _button.onClick.AddListener( SCManager.Instance.Exit );
    void OnDisable() => _button.onClick.RemoveListener( SCManager.Instance.Exit );
}
