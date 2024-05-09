using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugInteraction : MonoBehaviour
{
    private OVRInput.Controller controller = OVRInput.Controller.RTouch;
    private GameManager gameManager; // Reference to GameManager script

    void Start()
    {
        // Find and store reference to the GameManager script
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        // Check if the primary button (index trigger) is pressed on the right Oculus Touch controller
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the mouse position into the scene
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray hits something
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object hit is tagged as "Bug"
                if (hit.collider.CompareTag("Bug"))
                {
                    // If it is a bug, make it disappear
                    Destroy(hit.collider.gameObject);
                    // Increase the game score
                    gameManager.IncreaseScore();
                }
            }
        }
    }
}