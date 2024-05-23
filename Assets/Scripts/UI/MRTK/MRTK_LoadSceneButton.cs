using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// Automatically makes the Interactable load a scene.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class MRTK_LoadSceneButton : MonoBehaviour
{
    Interactable _interactable;
    [SerializeField] public string sceneName;
    
    void Awake()     => _interactable = GetComponent<Interactable>();
    void OnEnable()  => _interactable.OnClick.AddListener( LoadScene );
    void OnDisable() => _interactable.OnClick.RemoveListener( LoadScene );
    public void LoadScene() { Debug.Log("test");  SceneManager.LoadScene(sceneName); }
    public string currentScene { get { return SceneManager.GetActiveScene().name; } }
    
    public void MiDebug()
    {
        Debug.Log("test");
    }
}
