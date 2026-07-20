using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ForestDialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject speechBubble;
    [SerializeField] private TMP_Text speechText;
    [SerializeField] private GameObject continueButton;

    [Header("Localization & Settings")]
    [SerializeField] private string tableName = "Forest"; // Change to your forest dialogue table name
    [SerializeField] private string keyPrefix = "F"; // e.g., "F_1", "F_2" instead of "R1_1"
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Camera Control")]
    [SerializeField] private CameraPanner cameraPanner;

    private StringTable _stringTable;
    private int scene = 1;
    
    // Typewriter state tracking
    private bool isTyping = false;
    private string currentFullText = "";
    private Coroutine typingCoroutine;

    private async void Awake()
    {
        // Hide the UI immediately upon loading the scene
        speechBubble.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);

        // Wait for the string table to load
        _stringTable = await LoadStringTableAsync(tableName);

        // Automatically start the dialogue once the text is ready
        if (_stringTable != null)
        {
            StartForestDialogue();
        }
    }

    private async Task<StringTable> LoadStringTableAsync(string targetTableName)
    {
        var handle = LocalizationSettings.StringDatabase.GetTableAsync(targetTableName);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;

        Debug.LogError($"Failed to load string table '{targetTableName}'");
        return null;
    }

    // Note: The Start() method was removed since initialization and hiding 
    // the UI is now handled completely within the async Awake() method.

    /// <summary>
    /// Call this when the forest scene starts (e.g., from an initial click)
    /// </summary>
    public void StartForestDialogue()
    {
        scene = 1;
        
        // Turn both the bubble and the button back on
        speechBubble.SetActive(true);
        if (continueButton != null) continueButton.SetActive(true);
        
        LoadDialogue(scene);
    }

    private void LoadDialogue(int currentScene)
    {
        if (_stringTable == null)
        {
            Debug.LogWarning("String table not loaded yet.");
            return;
        }

        string key = keyPrefix + currentScene;

        if (_stringTable[key] != null)
        {
            currentFullText = _stringTable[key].Value;
            
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine(currentFullText));
        }
        else
        {
            // If the key doesn't exist, we've reached the end of the dialogue
            EndDialogue();
        }
    }

    /// <summary>
    /// Hook this up to your speech continue button's OnClick event
    /// </summary>
    public void onContinueClick()
    {
        if (isTyping)
        {
            // The player clicked while text was still animating. 
            // Stop the coroutine and instantly show the full text.
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            speechText.text = currentFullText;
            isTyping = false;
        }
        else
        {
            // Text finished animating. Increment scene and load the next string.
            scene++;
            LoadDialogue(scene);
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        speechText.text = ""; // Clear the box

        foreach (char letter in line.ToCharArray())
        {
            speechText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
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