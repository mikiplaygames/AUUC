using UnityEngine;
using UnityEngine.Events;

public class SpriteFadeout : MonoBehaviour
{
    [SerializeField] private float duration = .5f;
    [SerializeField] private LeanTweenType fadeInEaseType = LeanTweenType.easeInQuad;
    [SerializeField] private LeanTweenType fadeOutEaseOutType = LeanTweenType.easeOutQuad;
    [SerializeField] private bool hideOnAwake = true; 
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    public UnityEvent OnFadeInFinished = new();
    public UnityEvent OnFadeOutFinished = new();

    private void Awake()
    {
        if (hideOnAwake)
            Hide();
    }
    
    public void PlayFadeIn(bool reset = false)
    {
        if (reset)
            Hide();
        LeanTween.value(gameObject, 0, 1, duration)
            .setEase(fadeInEaseType)
            .setOnUpdate(UpdateAlpha)
            .setOnComplete(() => OnFadeInFinished?.Invoke());
    }

    public void PlayFadeOut()
    {
        LeanTween.value(gameObject, 1, 0, duration)
            .setEase(fadeOutEaseOutType)
            .setOnUpdate(UpdateAlpha)
            .setOnComplete(() => OnFadeOutFinished?.Invoke());
    }

    public void Hide()
    {
        UpdateAlpha(0);
    }

    private void UpdateAlpha(float alpha)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }
}
