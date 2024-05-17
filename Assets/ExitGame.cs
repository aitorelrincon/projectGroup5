using UnityEngine;
using UnityEngine.XR;

public class ExitGame : MonoBehaviour
{
    public void vittusaatana()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running in the build
        Application.Quit();
#endif
    }
}
