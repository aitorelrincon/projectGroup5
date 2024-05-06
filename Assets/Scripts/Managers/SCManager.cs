// ---------------------------------------------------------------------------------
// SCRIPT PARA LA GESTI�N DE ESCENAS (vinculado a un GameObject vac�o "SceneManager")
// ---------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement; // Se incluye la librer�a para el manejo de escenas
// OJO: al incluir esta librer�a, no se podr� usar el nombre "SceneManager" porque
// ya hay una clase Static con dicho nombre. Por eso la clase se llama "SCManager"

using BugCatcher.Utils;
using UnityEditor;

public class SCManager : MonoSingle<SCManager>
{
    protected override void OnAwake()
    {
        // This way we can easily customize scene loading
        // SceneManager.sceneLoaded   += ( s, m ) => { };
        // SceneManager.sceneUnloaded += ( s, m ) => { };
    }

    public void LoadScene( string sceneName ) => SceneManager.LoadScene( sceneName );
    public void LoadScene( int index )        => SceneManager.LoadScene( index );

    public void Exit() =>
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
}