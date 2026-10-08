using UnityEngine;
using TMPro; // Required for TextMeshPro text

public class QuestionDisplay : MonoBehaviour
{
    [Header("Script References")]
    [Tooltip("Drag the GameObject holding your GraphDisplayManager here")]
    public GraphDisplayManager graphManager; 

    [Header("UI References")]
    [Tooltip("Drag the Text (TMP) component for your dialogue box here")]
    public TextMeshProUGUI questionTextUI;   

    /// <summary>
    /// Call this method right when your camera finishes panning to the bottom canvas,
    /// or when the bottom canvas is activated.
    /// </summary>
    public void DisplaySavedQuestion()
    {
        if (graphManager != null && questionTextUI != null)
        {
            // Read the string saved in memory and push it to the UI text
            questionTextUI.text = graphManager.activeQuestion;
        }
        else
        {
            Debug.LogWarning("Missing references in QuestionDisplay!");
        }
    }
}