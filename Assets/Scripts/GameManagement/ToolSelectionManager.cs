using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class MeasurementTool
{
    [Tooltip("The exact name of the tool (e.g., 'Anemometer')")]
    public string toolName;
    
    [Tooltip("The event to fire when THIS tool is correctly selected (Hook up your CameraPanner here!)")]
    public UnityEvent onCorrectSelection;
}

public class ToolSelectionManager : MonoBehaviour
{
    [Header("Dialogue Setup")]
    public TextMeshProUGUI characterDialogueText;

    [Header("Tool Configuration")]
    [Tooltip("Add all your tools here. The script will pick from these randomly.")]
    public List<MeasurementTool> allTools;

    // Internal tracking
    private List<MeasurementTool> remainingTools;
    private MeasurementTool currentTargetTool;

    private void Start()
    {
        // Copy the master list into our active tracking list
        remainingTools = new List<MeasurementTool>(allTools);
        
        // Pick the first random tool
        PickNextTool();
    }

    /// <summary>
    /// Selects a random tool from the remaining unmeasured tools.
    /// Call this method when the player finishes measuring and returns to the forest.
    /// </summary>
    public void PickNextTool()
    {
        if (remainingTools.Count == 0)
        {
            characterDialogueText.text = "Great job! We've found all the tools.";
            return;
        }

        // Pick a random index from whatever is left in the list
        int randomIndex = Random.Range(0, remainingTools.Count);
        currentTargetTool = remainingTools[randomIndex];

        characterDialogueText.text = $"Find the {currentTargetTool.toolName}";
    }

    /// <summary>
    /// Fired by the UI Buttons. Checks if the clicked tool matches the current target.
    /// </summary>
    public void CheckToolSelection(string clickedToolName)
    {
        // If there is no active target right now, ignore the click
        if (currentTargetTool == null || string.IsNullOrEmpty(currentTargetTool.toolName)) return;

        if (string.Equals(clickedToolName, currentTargetTool.toolName, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log($"Correct! {clickedToolName} selected.");
            characterDialogueText.text = $"Great! Let's take a measurement with the {clickedToolName}.";
            
            currentTargetTool.onCorrectSelection?.Invoke();
            remainingTools.Remove(currentTargetTool);

            // FIX: Wipe the current target so it cannot be matched again!
            currentTargetTool = null;
        }
        else
        {
            Debug.Log($"Wrong tool. They clicked: {clickedToolName}");
            string article = GetIndefiniteArticle(clickedToolName);
            characterDialogueText.text = $"That's {article} {clickedToolName}... we need the {currentTargetTool.toolName}.";
        }
    }

    private string GetIndefiniteArticle(string word)
    {
        if (string.IsNullOrEmpty(word)) return "a";
        
        char firstLetter = char.ToLower(word[0]);
        if (firstLetter == 'a' || firstLetter == 'e' || firstLetter == 'i' || firstLetter == 'o' || firstLetter == 'u')
        {
            return "an";
        }
        return "a";
    }
}