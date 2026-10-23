using UnityEngine;
using UnityEngine.UI;

public class ResearchProgressionManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("The GameObject of the Continue Button on the Pinboard")]
    public GameObject pinboardContinueButton;

    private bool loopCompleted = false;

    private void Start()
    {
        // Ensure the button is physically hidden when the scene starts
        if (pinboardContinueButton != null)
        {
            pinboardContinueButton.SetActive(false);
        }
    }

    // Call this method whenever a graph/passage loop finishes
    public void CompleteScrapLoop()
    {
        if (!loopCompleted)
        {
            loopCompleted = true;
            if (pinboardContinueButton != null)
            {
                pinboardContinueButton.SetActive(true);
            }
        }
    }
}