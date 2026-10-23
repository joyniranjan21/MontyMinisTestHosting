using UnityEngine;

public class SpaceStationDialogueManager : GeneralDialogueManager
{
    [Header("Camera Control")]
    [SerializeField] private CameraPanner cameraPanner;

    [Header("Forest DM")]
    [SerializeField] private ForestDialogueManager forestDM;

    [Header("Space Station Assets")]
    [SerializeField] private GameObject callScreen;
    [SerializeField] private GameObject HRadar;
    [SerializeField] private GameObject SSbackground;
    [SerializeField] private GameObject Monty;

    protected override void LoadDialogue(int currentScene)
    {
        // 1. Swap your visual assets first
        if (currentScene == 2)
        {
            callScreen.SetActive(false);
            HRadar.SetActive(true);
            SSbackground.SetActive(false);
            Monty.SetActive(false);
        }
        else if (currentScene == 3)
        {
            callScreen.SetActive(false);
            HRadar.SetActive(false);
            SSbackground.SetActive(true);
            Monty.SetActive(true);
        }

        // 2. Tell the base class to handle fetching and typing the text!
        base.LoadDialogue(currentScene);
    }
    public override void EndDialogue()
    {
        // Run the clean up
        speechBubble.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);
        speechText.text = "";
        speechText.gameObject.SetActive(false); 

        Debug.Log("Monty dialogue is completely done!");

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
        forestDM.StartDialogue(); 
        
    }
}