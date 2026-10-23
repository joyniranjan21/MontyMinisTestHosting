using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class GraphDisplayManager : MonoBehaviour
{
    [System.Serializable]
    public struct QuestionData
    {
        public Sprite graphSprite; 
        [TextArea(2, 4)]
        public string questionText;
        public string[] answers; 
    }

    [System.Serializable]
    public struct GraphData
    {
        public string label; 
        public QuestionData[] questions; 
    }

    [Header("Camera Control")]
    public CameraPanner graphViewPanner; 
    public CameraPanner questionViewPanner; 
    
    [Header("Confidence Handoff")]
    public CameraPanner confidencePanner; 
    public ConfidenceManager confidenceManager; 
    public float defaultCameraSize = 535;

    [Header("Graph Database")]
    public GraphData[] allScraps;

    [Header("UI References")]
    public Image zoomedGraphImage;       
    public Image amaviCanvasGraphImage; 
    public TextMeshProUGUI[] answerChoiceUI; 
    public TextMeshProUGUI questionTextUI; 

    [Header("Saved State (Read-Only)")]
    public string activeQuestion; 
    public string currentCorrectAnswer; 
    
    [Header("Manager to move to lab scene")]
    public ResearchProgressionManager researchProgressionManager;

    [Tooltip("Tracks the total number of correct graph questions answered across the entire game.")]
    public int totalCorrectGraphAnswers = 0; 
    
    private int currentScrapIndex = 0;
    private int currentQuestionIndex = 0;
    private int currentViewIndex = 0; 

    public void LoadScrapByIndex(int scrapIndex)
    {
        if (scrapIndex < 0 || scrapIndex >= allScraps.Length) return;

        currentScrapIndex = scrapIndex;
        currentQuestionIndex = 0; 
        currentViewIndex = 0; 

        GraphData selectedScrap = allScraps[currentScrapIndex];
        if (selectedScrap.questions != null && selectedScrap.questions.Length > 0)
        {
            if (zoomedGraphImage != null) zoomedGraphImage.sprite = selectedScrap.questions[0].graphSprite;
        }

        if (graphViewPanner != null)
        {
            graphViewPanner.mainCamera.orthographicSize = defaultCameraSize;
            graphViewPanner.StartPanToTarget();
        }
    }

    public void OnContinueButtonClicked()
    {
        GraphData currentScrap = allScraps[currentScrapIndex];
        currentViewIndex++;

        if (currentViewIndex < currentScrap.questions.Length)
        {
            if (zoomedGraphImage != null) zoomedGraphImage.sprite = currentScrap.questions[currentViewIndex].graphSprite;
        }
        else
        {
            LoadCurrentQuestionData();

            if (questionViewPanner != null)
            {
                questionViewPanner.mainCamera.orthographicSize = defaultCameraSize;
                questionViewPanner.StartPanToTarget();
            }
        }
    }

    private void LoadCurrentQuestionData()
    {
        GraphData currentScrap = allScraps[currentScrapIndex];
        if (currentScrap.questions == null || currentScrap.questions.Length == 0) return;

        QuestionData currentQ = currentScrap.questions[currentQuestionIndex];
        
        if (zoomedGraphImage != null) zoomedGraphImage.sprite = currentQ.graphSprite;
        if (amaviCanvasGraphImage != null) amaviCanvasGraphImage.sprite = currentQ.graphSprite;

        activeQuestion = currentQ.questionText;
        if (questionTextUI != null) questionTextUI.text = activeQuestion;

        if (currentQ.answers != null && currentQ.answers.Length > 0)
        {
            currentCorrectAnswer = currentQ.answers[0];
            RandomizeAndDisplayAnswers(currentQ.answers);
        }
    }

    private void RandomizeAndDisplayAnswers(string[] originalAnswers)
    {
        List<string> shuffledAnswers = new List<string>(originalAnswers);
        
        for (int i = 0; i < shuffledAnswers.Count; i++)
        {
            string temp = shuffledAnswers[i];
            int randomIndex = Random.Range(i, shuffledAnswers.Count);
            shuffledAnswers[i] = shuffledAnswers[randomIndex];
            shuffledAnswers[randomIndex] = temp;
        }

        for (int i = 0; i < answerChoiceUI.Length; i++)
        {
            if (i < shuffledAnswers.Count && answerChoiceUI[i] != null)
            {
                answerChoiceUI[i].text = shuffledAnswers[i];
            }
        }
    }

    public void CheckAnswer(int buttonIndex)
    {
        bool isCorrect = (answerChoiceUI[buttonIndex].text == currentCorrectAnswer);
        
        if (isCorrect) 
        {
            totalCorrectGraphAnswers++;
            Debug.Log($"Correct! Total Graph Score: {totalCorrectGraphAnswers}");
        }
        else 
        {
            Debug.Log("Incorrect!");
        }

        currentQuestionIndex++;
        
        if (currentQuestionIndex < allScraps[currentScrapIndex].questions.Length)
        {
            LoadCurrentQuestionData(); 
        }
        else
        {
            researchProgressionManager.CompleteScrapLoop();
            if (confidenceManager != null)
            {
                confidenceManager.SetupConfidenceMeter(ConfidenceManager.ReturnRoute.GraphPinboard);
            }

            if (confidencePanner != null)
            {
                confidencePanner.mainCamera.orthographicSize = defaultCameraSize;
                confidencePanner.StartPanToTarget();
            }
        }
    }
}