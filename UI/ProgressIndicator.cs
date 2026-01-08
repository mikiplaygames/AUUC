using UnityEngine;
using UnityEngine.Events;

public class ProgressIndicator : MonoBehaviour
{
    public static readonly int AmountIndex = Shader.PropertyToID("_Amount");
    
    public UnityEvent OnAnimationStarted = new();
    public UnityEvent OnAnimationFinished = new();
    public UnityEvent OnAnimationStopped = new();

    [SerializeField][Range(0, 1)] protected float animateFrom = 0;
    [SerializeField][Range(0, 1)] protected float animateTo = 1;
    [SerializeField] private bool hideOnFinish = true;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected Material spriteMaterial;
    private int animationId = -1;
    private bool isAnimating => animationId != -1;
    protected virtual void Awake()
    {
        spriteMaterial = spriteRenderer.material;
    }

    public void StartAnimation(float animationTime)
    {
        if (isAnimating)
            return;
        OnAnimationStarted?.Invoke();
        spriteRenderer.enabled = true;
        animationId = LeanTween.value(gameObject, animateFrom, animateTo, animationTime)
            .setOnUpdate(value => spriteMaterial.SetFloat(AmountIndex, value))
            .setOnComplete(() =>
            {
                spriteRenderer.enabled = !hideOnFinish;
                OnAnimationFinished?.Invoke();
                animationId = -1;
            }).id;
    }

    public void StopAnimation()
    {
        if (!isAnimating)
            return;
        OnAnimationStopped?.Invoke();
        LeanTween.cancel(animationId, true);
        animationId = -1;
    }
}