using UnityEngine;
using UnityEngine.Events;

public class PopIn : MonoBehaviour
{
    [SerializeField] private float duration = .5f;
    [SerializeField] private float pullOutScaleMultiplier = 2f;
    [SerializeField] private LeanTweenType popInEaseType = LeanTweenType.easeOutBack;
    [SerializeField] private LeanTweenType popOutEaseOutType = LeanTweenType.easeInBack;
    [SerializeField] private LeanTweenType pullInEaseType = LeanTweenType.easeInQuad;
    [SerializeField] private LeanTweenType pullOutEaseOutType = LeanTweenType.easeInQuad;
    [SerializeField] private bool hideOnAwake = true; 
    [SerializeField] private bool overrideDefaultScale = false;
    [SerializeField] private Vector3 overrideScale = Vector3.one;
    private Vector3 defaultScale = Vector3.one;
    
    public UnityEvent OnPopInFinished = new();
    public UnityEvent OnPopOutFinished = new();

    private void Awake()
    {
        defaultScale = overrideDefaultScale ? overrideScale : transform.localScale;
        if (hideOnAwake)
            Reset();
    }
    
    public void PlayPopIn(bool reset = false)
    {
        if (reset)
            Reset();
        LeanTween.scale(gameObject, defaultScale, duration)
            .setEase(popInEaseType)
            .setOnComplete(() => OnPopInFinished?.Invoke());
    }

    public void PlayPopOut()
    {
        LeanTween.scale(gameObject, Vector3.zero, duration)
            .setEase(popOutEaseOutType)
            .setOnComplete(() => OnPopOutFinished?.Invoke());
    }

    public void PlayPullIn()
    {
        transform.localScale = defaultScale * pullOutScaleMultiplier;
        LeanTween.scale(gameObject, defaultScale, duration)
            .setEase(pullOutEaseOutType)
            .setOnComplete(() => OnPopOutFinished?.Invoke());
    }
    
    public void PlayPullOut()
    {
        LeanTween.scale(gameObject, defaultScale * pullOutScaleMultiplier, duration)
            .setEase(pullOutEaseOutType)
            .setOnComplete(() => OnPopOutFinished?.Invoke());
    }

    public void Reset()
    {
        transform.localScale = Vector3.zero;
    }
}
