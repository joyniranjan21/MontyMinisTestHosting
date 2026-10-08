using UnityEngine;
using UnityEngine.UI;

public class GraphDisplayManager : MonoBehaviour
{
    // 1. Create a custom container for your paired data
    [System.Serializable]
    public struct GraphData
    {
        public string label; // Just to keep your Inspector organized
        public Sprite graphSprite;
        [TextArea(2, 4)]
        public string questionText; // Can be raw text or a key for your dialogue/localization manager
    }

    [Header("Camera Control")]
    public CameraPanner panner;

    [Tooltip("Set this to your camera's original zoomed-out size (often 5)")]
    public float defaultCameraSize = 535;

    [Header("Graph Database")]
    [Tooltip("Add all 6 scraps and their questions here")]
    public GraphData[] allScraps;

    [Header("UI References")]
    public Image zoomedGraphImage;       // The Image component in the right canvas
    public Image amaviCanvasGraphImage; // The Image component in the bottom question canvas
    
    [Header("Saved State (Read-Only)")]
    public string activeQuestion; // Your dialogue script will read from this variable later

    // 2. Call this from the OnClick event using an integer
    public void LoadScrapByIndex(int scrapIndex)
    {
        if (scrapIndex < 0 || scrapIndex >= allScraps.Length) return;

        // Fetch the data paired with the clicked button
        GraphData selectedScrap = allScraps[scrapIndex];

        // Update the visual sprites on both canvases at once
        if (zoomedGraphImage != null) zoomedGraphImage.sprite = selectedScrap.graphSprite;
        if (amaviCanvasGraphImage != null) amaviCanvasGraphImage.sprite = selectedScrap.graphSprite;

        // SAVE THE QUESTION IN MEMORY
        activeQuestion = selectedScrap.questionText;
        
        if (panner != null)
        {
            panner.mainCamera.orthographicSize = defaultCameraSize;
            panner.StartPanToTarget();
        }
    }
}