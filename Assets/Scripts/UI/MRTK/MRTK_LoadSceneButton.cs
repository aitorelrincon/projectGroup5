using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// Automatically makes the Interactable load a scene.
/// </summary>
[RequireComponent(typeof(Interactable))]
public class MRTK_LoadSceneButton : MonoBehaviour
{
    [SerializeField] public string sceneName;
  
    public void LoadScene(string sceneName) { SceneManager.LoadScene(sceneName); }
    public string currentScene { get { return SceneManager.GetActiveScene().name; } }
}
