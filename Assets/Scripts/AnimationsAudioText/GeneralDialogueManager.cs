using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public abstract class GeneralDialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] protected GameObject speechBubble;
    [SerializeField] protected TMP_Text speechText;
    [SerializeField] protected GameObject continueButton;

    [Header("Localization & Settings")]
    [SerializeField] protected string tableName = ""; // Change to your dialogue table name
    [SerializeField] protected string keyPrefix = "F"; // e.g., "F_1", "F_2" instead of "R1_1"
    [SerializeField] private float typingSpeed = 0.03f;

    [SerializeField] protected bool autoStartDialogue = true;
    protected StringTable _stringTable;
    private int scene = 1;
    
    // Typewriter state tracking
    private bool isTyping = false;
    protected string currentFullText = "";
    protected Coroutine typingCoroutine;

    private async void Awake()
    {
        // Hide the UI immediately upon loading the scene
        speechBubble.SetActive(false);
        if (continueButton != null) continueButton.SetActive(false);

        // Wait for the string table to load
        _stringTable = await LoadStringTableAsync(tableName);

        // MODIFIED: Only start automatically if the toggle is checked
        if (_stringTable != null && autoStartDialogue)
        {
            StartDialogue();
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
    public void StartDialogue()
    {
        scene = 1;
        
        // Turn the UI back on
        speechBubble.SetActive(true);
        speechText.gameObject.SetActive(true); // <-- This wakes the text back up!
        
        if (continueButton != null) 
        {
            continueButton.SetActive(true);
            
            Button btn = continueButton.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(onContinueClick);
            }
        }
        
        LoadDialogue(scene);
    }

    protected virtual void LoadDialogue(int currentScene)
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

    protected IEnumerator TypeLine(string line)
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

    public abstract void EndDialogue();

}