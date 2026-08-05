using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LabDialogueManager : GeneralDialogueManager
{
    [Header("Camera Control")]
    [SerializeField] private CameraPanner cameraPanner;

    private bool _isContinuePressed = false; // Add this flag

    // Add this new method for the UI Button
    public void OnLabContinueClick()
    {
        _isContinuePressed = true;
    }

    public void StartLabDialogue()
    {
        StartCoroutine(PlayLabDialogue());
    }

    private IEnumerator PlayLabDialogue()
    {
        int lineIndex = 1;
        while (true)
        {
            string key = $"{keyPrefix}_{lineIndex}";
            var entry = _stringTable.GetEntry(key);

            if (entry == null)
            {
                Debug.Log($"No more dialogue lines found for key: {key}");
                break; // Exit the loop if no more lines are found
            }

            string dialogueLine = entry.GetLocalizedString();
            yield return StartCoroutine(TypeLine(dialogueLine));

            // Wait for user input to continue
            if (continueButton != null)
            {
                continueButton.SetActive(true);
                // Wait exclusively for the UI continue button to be clicked
                yield return new WaitUntil(() => _isContinuePressed);
                continueButton.SetActive(false);
            }

            lineIndex++;
        }

        // Dialogue finished, hide the speech bubble
        speechBubble.SetActive(false);
    }

    public override void EndDialogue()
    {
        // Hide the background and button
        speechBubble.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);
        
        // Wipe the text and hide the text component just in case
        speechText.text = "";
        speechText.gameObject.SetActive(false); 
        
        // Trigger the camera pan to the second scene
        if (cameraPanner != null)
        {
            cameraPanner.StartPanToTarget();
        }
        else
        {
            Debug.LogWarning("Camera Panner is not assigned in the ForestDialogueManager!");
        }
    }
}