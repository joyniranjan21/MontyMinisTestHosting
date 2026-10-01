using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class MeasurementToolMS
{
    [Tooltip("The exact name of the tool (e.g., 'Anemometer')")]
    public string toolName;
    
    [Tooltip("The event to fire when this tool is selected (Hook up your CameraPanner here!)")]
    public UnityEvent onToolSelected; // Renamed from onCorrectSelection since any choice is now valid
}

public class MeasurementSelectionManager : MonoBehaviour
{
    [Header("Dialogue Setup")] 
    public TextMeshPro characterDialogueText;

    [Header("Tool Configuration")]
    [Tooltip("Add all your tools here. The player can select them in any order.")]
    public List<MeasurementToolMS> allTools;

    // Internal tracking
    private List<MeasurementToolMS> remainingTools;

    private void Start()
    {
        // Copy the master list into our active tracking list
        remainingTools = new List<MeasurementToolMS>(allTools);
    }

    /// <summary>
    /// Call this method via UnityEvent to begin Phase 2.
    /// </summary>
    public void StartMeasurementPhase()
    {
        if (characterDialogueText != null)
        {
            characterDialogueText.text = "Click on any tool on the table to enter its measurement!";
        }
    }

    /// <summary>
    /// Fired by the UI Buttons. Checks if the clicked tool is still available.
    /// </summary>
    public void SelectTool(string clickedToolName)
    {
        // Find the tool in our remaining list based on the name passed by the button
        MeasurementToolMS selectedTool = remainingTools.Find(t => t.toolName == clickedToolName);

        // If the tool is found, it means it hasn't been used yet
        if (selectedTool != null)
        {
            Debug.Log($"{clickedToolName} selected.");
            
            if (characterDialogueText != null)
            {
                characterDialogueText.text = $"Great! Let's take a measurement with the {clickedToolName}.";
            }
            
            // Fire the tool's specific event
            selectedTool.onToolSelected?.Invoke();

            // Remove it from the remaining tools so it cannot be selected again
            remainingTools.Remove(selectedTool);

            // Optional: Check if the player has finished all tools
            if (remainingTools.Count == 0)
            {
                Debug.Log("All measurements have been completed!");
                // You can fire an "OnAllToolsFinished" event here if needed
            }
        }
        else
        {
            // The tool was either already used or the name has a typo in the button's OnClick event
            Debug.LogWarning($"Tool '{clickedToolName}' has already been used or does not exist!");
        }
    }
}