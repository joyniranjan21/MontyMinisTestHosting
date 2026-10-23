using UnityEngine;

public class ArchiveDialogueManager : GeneralDialogueManager
{
    // Make sure autoStartDialogue is false on this script so it waits for Forest to call StartDialogue()

    public override void EndDialogue()
    {
        // Run the clean up
        speechBubble.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);
        speechText.text = "";
        speechText.gameObject.SetActive(false); 
        
        // Put whatever happens after the Archive scene is completely finished here!
        Debug.Log("Archive dialogue is completely done!");
    }
}