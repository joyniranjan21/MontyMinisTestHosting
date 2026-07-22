using UnityEngine;

public class PinboardClicker : MonoBehaviour
{
    [Tooltip("Drag your Main Camera here.")]
    public CameraPanToPinBoard cameraPanner;

    // This built-in Unity function automatically fires when a mouse clicks a 2D Collider!
    private void OnMouseDown()
    {

        Debug.Log("The Pinboard was successfully clicked!"); // ADD THIS LINE

        if (cameraPanner != null)
        {
            // Trigger the zoom
            cameraPanner.StartZoomToBoard();
            
            // Turn off this collider so the player can't accidentally click the board again while zoomed in
            GetComponent<Collider2D>().enabled = false; 
        }
        else
        {
            Debug.LogWarning("Camera Panner not assigned in the PinboardClicker!");
        }
    }

    
}