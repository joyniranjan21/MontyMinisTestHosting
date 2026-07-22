using System.Collections;
using UnityEngine;

public class CameraPanToPinBoard : MonoBehaviour
{
    [Header("Camera & Targets")]
    [Tooltip("The camera that will move. Leave blank to auto-grab Main Camera.")]
    public Camera mainCamera;
    
    [Tooltip("The RectTransform of the Pinboard you want to zoom to.")]
    public RectTransform targetPinboard;

    [Header("Zoom Settings")]
    [Tooltip("The target orthographic size when zoomed in. Smaller = closer.")]
    public float zoomedInSize = 3f; 
    
    [Header("Animation Settings")]
    public float panDuration = 1.5f;
    public AnimationCurve panCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Interactivity")]
    [Tooltip("A CanvasGroup attached to a parent object holding all the paper buttons.")]
    public CanvasGroup papersCanvasGroup;

    private float _originalSize;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        _originalSize = mainCamera.orthographicSize;

        // Ensure the papers cannot be clicked or hovered when the scene starts
        LockPapers(true);
    }

    /// <summary>
    /// Trigger this when the player clicks the pinboard from afar.
    /// </summary>
    public void StartZoomToBoard()
    {
        if (targetPinboard == null)
        {
            Debug.LogError("No target pinboard assigned!");
            return;
        }

        // Get the exact world position of the pinboard, but keep the camera's Z distance
        Vector3 targetPosition = targetPinboard.position;
        targetPosition.z = mainCamera.transform.position.z;

        StartCoroutine(PanAndZoomCoroutine(targetPosition, zoomedInSize));
    }

    private IEnumerator PanAndZoomCoroutine(Vector3 targetPos, float targetZoom)
    {
        Vector3 startPos = mainCamera.transform.position;
        float startZoom = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < panDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / panDuration;
            float curveValue = panCurve.Evaluate(percentage);

            // Lerp both the position AND the camera zoom size
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, curveValue);
            mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, curveValue);
            
            yield return null;
        }

        // Snap precisely at the end
        mainCamera.transform.position = targetPos;
        mainCamera.orthographicSize = targetZoom;

        // The camera has arrived! Unlock the papers so the player can click them.
        LockPapers(false);
    }

    private void LockPapers(bool isLocked)
    {
        if (papersCanvasGroup != null)
        {
            // If locked is true, interactable becomes false.
            papersCanvasGroup.interactable = !isLocked;
            papersCanvasGroup.blocksRaycasts = !isLocked;
        }
    }
}