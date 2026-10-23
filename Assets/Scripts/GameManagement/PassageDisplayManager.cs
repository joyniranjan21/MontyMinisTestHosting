using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassageDisplayManager : MonoBehaviour
{
    [System.Serializable]
    public struct QuestionData
    {
        [TextArea(2, 4)]
        public string questionText;
        public string[] answers;
    }

    [System.Serializable]
    public struct PassageData
    {
        public string passageHeading;
        [TextArea(5, 10)]
        public string passageBody;
        public QuestionData[] questions;
    }

    [Header("Camera Control")]
    public CameraPanner passageViewPanner;
    
    [Header("Confidence Handoff")]
    public CameraPanner confidencePanner;
    public ConfidenceManager confidenceManager;
    public float defaultCameraSize = 535;

    [Header("Passage Database")]
    public PassageData[] allPassages;

    [Header("UI References")]
    public TextMeshProUGUI passageHeadingUI;
    public TextMeshProUGUI passageBodyUI;
    public TextMeshProUGUI questionTextUI;
    
    public TextMeshProUGUI[] answerChoiceUI;
    public Button[] answerChoiceButtons; 
    
    public Button backButton;
    public Button forwardButton; // NEW: We now track the forward button

    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.7f, 0.7f, 0.7f); 

    [Header("Saved State (Read-Only)")]
    public int totalCorrectPassageAnswers = 0;

    private int currentPassageIndex = 0;
    private int currentQuestionIndex = 0;
    
    private string[] userSelections;
    private List<string[]> shuffledAnswersCache = new List<string[]>();
    private string[] correctAnswersCache;

    public void LoadPassageByIndex(int index)
    {
        Debug.Log($"Loading passage at index: {index}");

        if (index < 0 || index >= allPassages.Length) return;

        currentPassageIndex = index;
        currentQuestionIndex = 0;
        
        PassageData selectedPassage = allPassages[currentPassageIndex];
        
        if (passageHeadingUI != null) passageHeadingUI.text = selectedPassage.passageHeading;
        if (passageBodyUI != null) passageBodyUI.text = selectedPassage.passageBody;

        int qCount = selectedPassage.questions.Length;
        userSelections = new string[qCount];
        correctAnswersCache = new string[qCount];
        shuffledAnswersCache.Clear();

        for (int i = 0; i < qCount; i++)
        {
            string[] original = selectedPassage.questions[i].answers;
            if (original != null && original.Length > 0)
            {
                correctAnswersCache[i] = original[0];
                shuffledAnswersCache.Add(ShuffleArray(original));
            }
            else
            {
                shuffledAnswersCache.Add(new string[0]);
            }
        }

        LoadQuestionUI();

        if (passageViewPanner != null)
        {
            passageViewPanner.mainCamera.orthographicSize = defaultCameraSize;
            passageViewPanner.StartPanToTarget();
        }
    }

    private string[] ShuffleArray(string[] array)
    {
        List<string> list = new List<string>(array);
        for (int i = 0; i < list.Count; i++)
        {
            string temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list.ToArray();
    }

    private void LoadQuestionUI()
    {
        PassageData currentPassage = allPassages[currentPassageIndex];
        QuestionData currentQ = currentPassage.questions[currentQuestionIndex];

        if (questionTextUI != null) questionTextUI.text = currentQ.questionText;

        string[] currentShuffled = shuffledAnswersCache[currentQuestionIndex];
        string previouslySelected = userSelections[currentQuestionIndex];

        for (int i = 0; i < answerChoiceUI.Length; i++)
        {
            if (i < currentShuffled.Length)
            {
                answerChoiceUI[i].text = currentShuffled[i];
                
                if (answerChoiceButtons.Length > i && answerChoiceButtons[i] != null)
                {
                    ColorBlock cb = answerChoiceButtons[i].colors;
                    if (!string.IsNullOrEmpty(previouslySelected) && currentShuffled[i] == previouslySelected)
                    {
                        cb.normalColor = selectedColor;
                        cb.selectedColor = selectedColor;
                    }
                    else
                    {
                        cb.normalColor = normalColor;
                        cb.selectedColor = normalColor;
                    }
                    answerChoiceButtons[i].colors = cb;
                }
            }
        }

        // Manage button interactability based on current state
        if (backButton != null) backButton.interactable = (currentQuestionIndex > 0);
        
        // NEW: Disable forward button if no answer is currently selected for this question
        if (forwardButton != null) forwardButton.interactable = !string.IsNullOrEmpty(previouslySelected);
    }

    public void SelectAnswer(int buttonIndex)
    {
        Debug.Log($"Answer selected for question {currentQuestionIndex}: Button {buttonIndex}");
        userSelections[currentQuestionIndex] = answerChoiceUI[buttonIndex].text;

        for (int i = 0; i < answerChoiceButtons.Length; i++)
        {
            if (answerChoiceButtons[i] != null)
            {
                ColorBlock cb = answerChoiceButtons[i].colors;
                if (i == buttonIndex)
                {
                    cb.normalColor = selectedColor;
                    cb.selectedColor = selectedColor;
                }
                else
                {
                    cb.normalColor = normalColor;
                    cb.selectedColor = normalColor;
                }
                answerChoiceButtons[i].colors = cb;
            }
        }

        // NEW: Instantly enable the forward button now that an answer has been chosen
        if (forwardButton != null) forwardButton.interactable = true;
    }

    public void NextQuestion()
    {
        // Secondary safety check just in case the button was somehow clicked
        if (string.IsNullOrEmpty(userSelections[currentQuestionIndex])) return;

        currentQuestionIndex++;

        if (currentQuestionIndex < allPassages[currentPassageIndex].questions.Length)
        {
            LoadQuestionUI();
        }
        else
        {
            GradePassageAndComplete();
        }
    }

    public void PreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            LoadQuestionUI();
        }
    }

    private void GradePassageAndComplete()
    {
        for (int i = 0; i < userSelections.Length; i++)
        {
            if (userSelections[i] == correctAnswersCache[i])
            {
                totalCorrectPassageAnswers++;
            }
        }

        Debug.Log($"Passage Complete! Total Passage Score: {totalCorrectPassageAnswers}");

        if (confidenceManager != null)
        {
            confidenceManager.SetupConfidenceMeter(ConfidenceManager.ReturnRoute.Passage);
        }

        if (confidencePanner != null)
        {
            confidencePanner.mainCamera.orthographicSize = defaultCameraSize;
            confidencePanner.StartPanToTarget();
        }
    }
}