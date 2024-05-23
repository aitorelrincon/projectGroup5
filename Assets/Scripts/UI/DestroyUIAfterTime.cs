using UnityEngine;
using UnityEngine.UI;

public class DestroyUIAfterTime : MonoBehaviour
{
    public GameObject imageObject; // Reference to the Image UI element
    public GameObject textObject;  // Reference to the Text UI element
    public float destroyTime = 30f; // Time in seconds after which the UI elements should be destroyed

    void Start()
    {
        // Destroy the image and text objects after 'destroyTime' seconds
        Destroy(imageObject, destroyTime);
        Destroy(textObject, destroyTime);
    }
}