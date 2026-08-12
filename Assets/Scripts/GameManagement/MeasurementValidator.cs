using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;
using UnityEngine.Events; // Add this!

[System.Serializable]
public struct ValidMeasurement
{
    public string unitName;
    public float expectedValue;
}

public class MeasurementValidator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField measurementInput;
    public TMP_Dropdown measurementDropdown;
    public Button submitButton;

    [Header("Validation Settings")]
    public List<ValidMeasurement> validMeasurements;

    [Header("Tolerance Settings")]
    public float fullCreditTolerance = 5f;
    public float partialCreditTolerance = 10f;

    [Header("Camera & Events")]
    public CameraPanner cameraPanner;
    
    [Tooltip("Fires when the player gets full or partial credit.")]
    public UnityEvent onMeasurementComplete; // Add this event!

    private void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(CheckValues);
        }
    }

    private void CheckValues()
    {
        if (float.TryParse(measurementInput.text, out float parsedNumber))
        {
            string currentDropdownText = measurementDropdown.options[measurementDropdown.value].text;
            bool isRecognizedUnit = false;
            float expectedNumberForUnit = 0f;

            foreach (ValidMeasurement measurement in validMeasurements)
            {
                if (string.Equals(currentDropdownText, measurement.unitName, System.StringComparison.OrdinalIgnoreCase))
                {
                    isRecognizedUnit = true;
                    expectedNumberForUnit = measurement.expectedValue;
                    break; 
                }
            }

            if (!isRecognizedUnit)
            {
                Debug.Log($"Validation Failed: '{currentDropdownText}' is an incorrect unit.");
                return;
            }

            float difference = Mathf.Abs(parsedNumber - expectedNumberForUnit);

            // Grade the result and trigger the completion events
            if (difference <= fullCreditTolerance || difference <= partialCreditTolerance)
            {
                Debug.Log("Measurement Successful!");
                TriggerCameraPan();
                
                // Fire the event to tell the ToolManager to pick the next tool
                onMeasurementComplete?.Invoke(); 
            }
            else
            {
                Debug.Log("No Credit.");
            }
        }
        else
        {
            Debug.LogWarning("Validation Failed: The input box does not contain a valid number.");
        }
    }

    private void TriggerCameraPan()
    {
        if (cameraPanner != null)
        {
            cameraPanner.StartPanToTarget();
        }
    }

    private void OnDestroy()
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(CheckValues);
        }
    }
}