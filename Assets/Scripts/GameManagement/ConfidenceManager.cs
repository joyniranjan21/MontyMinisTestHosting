using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConfidenceManager : MonoBehaviour
{
    // Define the possible systems that can call this meter
    public enum ReturnRoute { GraphPinboard, Passage }

    [Header("UI References")]
    public Slider confidenceSlider;
    public TextMeshProUGUI confidenceText;

    [Header("Camera Return Panners")]
    public CameraPanner pinboardReturnPanner;
    public CameraPanToPinBoard pinboardPanTo;
    public CameraPanner passageReturnPanner;
    public float defaultCameraSize = 535f;

    private ReturnRoute currentRoute;

    // External managers call this to prep the meter before panning to it
    public void SetupConfidenceMeter(ReturnRoute route)
    {
        currentRoute = route;
        
        if (confidenceSlider != null) confidenceSlider.value = 0;
        UpdateConfidenceText(0);
    }

    // Hook this to the Slider's "On Value Changed" event in the Inspector
    public void UpdateConfidenceText(float value)
    {
        if (confidenceText != null)
        {
            confidenceText.text = Mathf.RoundToInt(value) + "%";
        }
    }

    // Hook this to your "Submit" button on the confidence screen
    public void SubmitConfidenceAndReturn()
    {
        float finalConfidence = confidenceSlider != null ? confidenceSlider.value : 0f;
        Debug.Log($"Confidence submitted: {finalConfidence}%");

        // Fire the correct return panner based on who called us
        if (currentRoute == ReturnRoute.GraphPinboard && pinboardReturnPanner != null)
        {
            pinboardReturnPanner.mainCamera.orthographicSize = defaultCameraSize;
            pinboardReturnPanner.StartPanToTarget();
            pinboardPanTo.StartZoomToBoard(); // Ensure we zoom back to the pinboard after panning
        }
        else if (currentRoute == ReturnRoute.Passage && passageReturnPanner != null)
        {
            passageReturnPanner.mainCamera.orthographicSize = defaultCameraSize;
            passageReturnPanner.StartPanToTarget();
        }
    }
}