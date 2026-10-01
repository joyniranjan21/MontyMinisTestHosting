using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class MeasurementTool
{
    [Tooltip("The exact name of the tool (e.g., 'Anemometer')")]
    public string toolName;

    [Tooltip("The physical GameObject in the scene to hide when selected")]
    public GameObject toolObject; 
    
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

    [Header("Phase Completion")]
    [Tooltip("Fires when all tools are found. Hook your CameraPanner up here to move to the table!")]
    public UnityEvent onAllToolsFound;

    // Internal tracking
    private List<MeasurementTool> remainingTools;
    private MeasurementTool currentTargetTool;

    private void Start()
    {
        remainingTools = new List<MeasurementTool>(allTools);
        PickNextTool();
    }

    public void PickNextTool()
    {
        // When the list is empty, phase 1 is complete
        if (remainingTools.Count == 0)
        {
            characterDialogueText.text = "Great job! Now let's go measure them.";
            
            // Trigger the transition to Phase 2
            onAllToolsFound?.Invoke(); 
            return;
        }

        int randomIndex = Random.Range(0, remainingTools.Count);
        currentTargetTool = remainingTools[randomIndex];
        characterDialogueText.text = $"Find the {currentTargetTool.toolName}";
    }

    public void CheckToolSelection(string clickedToolName)
    {
        if (currentTargetTool == null || string.IsNullOrEmpty(currentTargetTool.toolName)) return;

        if (string.Equals(clickedToolName, currentTargetTool.toolName, System.StringComparison.OrdinalIgnoreCase))
        {
            if (currentTargetTool.toolObject != null)
            {
                currentTargetTool.toolObject.SetActive(false);
            }

            currentTargetTool.onCorrectSelection?.Invoke();
            remainingTools.Remove(currentTargetTool);
            currentTargetTool = null;

            PickNextTool();
        }
        else
        {
            string article = GetIndefiniteArticle(clickedToolName);
            characterDialogueText.text = $"That's {article} {clickedToolName}... we need the {currentTargetTool.toolName}.";
        }
    }

    private string GetIndefiniteArticle(string word)
    {
        if (string.IsNullOrEmpty(word)) return "a";
        char firstLetter = char.ToLower(word[0]);
        if (firstLetter == 'a' || firstLetter == 'e' || firstLetter == 'i' || firstLetter == 'o' || firstLetter == 'u') return "an";
        return "a";
    }
}