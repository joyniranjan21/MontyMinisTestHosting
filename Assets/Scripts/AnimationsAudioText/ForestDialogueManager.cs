using UnityEngine;

public class ForestDialogueManager : GeneralDialogueManager
{
    [Header("Camera Control")]
    [SerializeField] private CameraPanner cameraPanner;
    [SerializeField] private ArchiveDialogueManager archiveDM;

    // We don't need custom typing coroutines. The base class handles all of that. 
    // We only need to tell the game what to do when the base class finishes.

    public override void EndDialogue()
    {
        // 1. Run the base class clean-up (hides UI and clears text)
        // If you don't have this in your base class yet, just put the hiding logic here.
        speechBubble.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);
        speechText.text = "";
        speechText.gameObject.SetActive(false); 
        
        // 2. Trigger the camera pan to the second scene
        if (cameraPanner != null)
        {
            cameraPanner.StartPanToTarget();
        }
        else
        {
            Debug.LogWarning("Camera Panner is not assigned in the ForestDialogueManager!");
        }

        // 3. Manually start the base class logic on the Archive Manager
        archiveDM.StartDialogue(); 
    }
}