using UnityEngine;
using UnityEngine.UI;

/*
 * Author: Tom� Prachensk� (Trampod)
 * Used by: Joy Niranjan
 * Simple script that holds a reference in it's hierarchy
 */

public class UserUISpeechBubble : SpeechBubble
{
    [field: SerializeField]
    public RectTransform ContinueButtonTransform { get; private set; }

    public Button ContinueButton => ContinueButtonTransform.GetComponent<Button>();

    public void SetCustomContinueListener(UnityEngine.Events.UnityAction action)
    {
        ContinueButton.onClick.AddListener(action);
    }
}
