using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode] // Allows you to see the arc movement in the Scene view without pressing Play
public class ArcSliderVisualizer : MonoBehaviour
{
    [Header("Slider & Visuals")]
    public Slider standardSlider;
    public RectTransform visualBanana;

    [Header("Curve Points (RectTransforms)")]
    [Tooltip("Place this at the top-left attachment point of the vine")]
    public RectTransform startPoint;
    
    [Tooltip("Place this at the lowest hanging point of the vine (the dip)")]
    public RectTransform controlPoint;
    
    [Tooltip("Place this at the top-right attachment point of the vine")]
    public RectTransform endPoint;

    private void OnEnable()
    {
        if (standardSlider != null)
            standardSlider.onValueChanged.AddListener(UpdateBananaPosition);
    }

    private void OnDisable()
    {
        if (standardSlider != null)
            standardSlider.onValueChanged.RemoveListener(UpdateBananaPosition);
    }

    public void UpdateBananaPosition(float value)
    {
        if (startPoint == null || controlPoint == null || endPoint == null || visualBanana == null) return;

        // Convert the 0-100 slider value into a 0.0 to 1.0 percentage (t)
        float t = Mathf.InverseLerp(standardSlider.minValue, standardSlider.maxValue, value);

        // Quadratic Bezier Curve Math
        Vector2 p0 = startPoint.anchoredPosition;
        Vector2 p1 = controlPoint.anchoredPosition;
        Vector2 p2 = endPoint.anchoredPosition;

        Vector2 calculatedPosition = (Mathf.Pow(1 - t, 2) * p0) + 
                                     (2 * (1 - t) * t * p1) + 
                                     (Mathf.Pow(t, 2) * p2);

        visualBanana.anchoredPosition = calculatedPosition;
    }

    private void Update()
    {
        // Forces the banana to update in the Scene view while dragging the slider manually
        if (!Application.isPlaying && standardSlider != null)
        {
            UpdateBananaPosition(standardSlider.value);
        }
    }
}