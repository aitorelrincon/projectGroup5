using UnityEngine;
using UnityEngine.XR;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.LogWarning( $"[ExitGame] - Called from {gameObject.name}" );
#else
        // If running in the build
        Application.Quit();
#endif
    }
}
