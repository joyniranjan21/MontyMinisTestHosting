using UnityEngine;

public class CameraPanner : MonoBehaviour
{
    [Header("Camera & Targets")]
    [Tooltip("The camera that will move. Leave blank to auto-grab Main Camera.")]
    public Camera mainCamera;
    
    [Tooltip("The RectTransform of the scene you want to snap to.")]
    public RectTransform targetCanvas;

    private void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    /// <summary>
    /// Call this from your Dialogue Manager when the dialogue ends.
    /// </summary>
    public void StartPanToTarget()
    {
        if (targetCanvas == null)
        {
            Debug.LogError("No target assigned to snap to!");
            return;
        }

        // Calculate and instantly apply the new position
        mainCamera.transform.position = CalculateTargetPosition();
    }

    private Vector3 CalculateTargetPosition()
    {
        // Reusing your exact math to handle aspect ratios
        float camHeight = 2f * mainCamera.orthographicSize;
        float camLength = camHeight * mainCamera.aspect;

        float canvasLength = targetCanvas.rect.width * targetCanvas.localScale.x;
        Vector3 canvasCenter = targetCanvas.TransformPoint(targetCanvas.rect.center);

        if (canvasLength <= camLength) 
        {
            return new Vector3(canvasCenter.x, canvasCenter.y, mainCamera.transform.position.z);
        }
        else 
        {
            return new Vector3(canvasCenter.x - (canvasLength - camLength) / 2, canvasCenter.y, mainCamera.transform.position.z);
        }
    }
}