using UnityEngine;
using UnityEngine.Events; // Required for UnityEvents

public class HighlightHover : MonoBehaviour
{
    [Header("BG Highlight Image")]
    [Tooltip("Hook this up to the Master Dialogue Controller in the Inspector")]
    
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private bool _isZoomedIn = false;

    static public void SetTransparency(SpriteRenderer sprite, float alpha) {
        Color color = sprite.color;
        color.a = alpha;
        sprite.color = color;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void HoverOn()
    {
        if (_spriteRenderer != null) 
        {
            SetTransparency(_spriteRenderer, 1f);
        }
    }

    public void HoverOff()
    {
        if (_spriteRenderer != null)
        {
            SetTransparency(_spriteRenderer, 0f);
        }
    }

    public void LockHighlight()
    {
        _isZoomedIn = true;
        HoverOff();
    }

    public void UnlockHighlight()
    {
        _isZoomedIn = false;
    }
}