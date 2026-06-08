using TMPro;
using UnityEngine;

/*
 * Author: Tom� Prachensk� (Trampod)
 * Used by: Joy Niranjan
 * Simple script that holds a reference in it's hierarchy
 */

public class SpeechBubble : MonoBehaviour
{
    [field: SerializeField]
    public TMP_Text SpeechText { get; private set; }

    private void Awake()
    {
        if (SpeechText == null)
        {
            SpeechText = GetComponentInChildren<TMP_Text>();

            if (SpeechText == null)
            {
                SpeechText = GetComponentInChildren<TMP_Text>(true);
            }
        }
    }
}
