using UnityEngine;
using System.Collections;

public class BackgroundDimmer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _dimSprite;
    [Tooltip("How dark the background gets (0 is invisible, 1 is pitch black)")]
    [SerializeField] private float _targetDarkness = 0.75f; 
    [Tooltip("How fast it fades to dark")]
    [SerializeField] private float _fadeSpeed = 3f;

    public void DimBackground()
    {
        StopAllCoroutines(); // Stops any ongoing fades
        StartCoroutine(FadeAlpha(_targetDarkness));
    }

    public void BrightenBackground()
    {
        StopAllCoroutines();
        StartCoroutine(FadeAlpha(0f)); // Fades back to invisible
    }

    private IEnumerator FadeAlpha(float targetAlpha)
    {
        Color color = _dimSprite.color;
        
        while (Mathf.Abs(color.a - targetAlpha) > 0.01f)
        {
            color.a = Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * _fadeSpeed);
            _dimSprite.color = color;
            yield return null;
        }
        
        color.a = targetAlpha;
        _dimSprite.color = color;
    }
}