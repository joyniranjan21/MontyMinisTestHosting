using UnityEngine;
using UnityEngine.Localization;
using TMPro;

public class ForestDialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Drag your Text (TMP) object here.")]
    public TMP_Text speechBubbleText;

    [Header("Localized Dialogue")]
    [Tooltip("Add your localized strings from your String Table here.")]
    public LocalizedString[] dialogueLines;

    private int currentIndex = 0;

    void Start()
    {
        // When the scene starts, load the very first localized line if we have any
        if (dialogueLines.Length > 0)
        {
            UpdateDialogueText();
        }
        else
        {
            speechBubbleText.text = "..."; 
        }
    }

    // This is the function your Speech Continue button will trigger!
    public void DisplayNextLine()
    {
        // Move to the next line in the array
        currentIndex++;

        // Check if we still have lines left to display
        if (currentIndex < dialogueLines.Length)
        {
            UpdateDialogueText();
        }
        else
        {
            // The conversation is over
            speechBubbleText.text = ""; 
            Debug.Log("End of localized conversation.");
            
            // Optional: You can disable the parent Speech Box here
            // transform.parent.gameObject.SetActive(false);
        }
    }

    // This handles the asynchronous loading of the translated text
    private void UpdateDialogueText()
    {
        // We ask the Localization system to fetch the current line's text
        var stringOperation = dialogueLines[currentIndex].GetLocalizedStringAsync();
        
        // We wait for the system to finish finding the translation, then apply it
        stringOperation.Completed += (handle) =>
        {
            speechBubbleText.text = handle.Result;
        };
    }
}