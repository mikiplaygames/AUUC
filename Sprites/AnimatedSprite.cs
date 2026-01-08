using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class AnimatedSprite : MonoBehaviour
{
    [SerializeField] protected AnimatedSpriteAsset spriteList; // Used only in inspector, copied to sprites
    [SerializeField] protected bool autoPlay = true;
    public UnityEvent OnAnimationFinished = new();
    public UnityEvent OnReverseAnimationFinished = new();
    private Coroutine currentAnimationCoroutine;
    public bool IsPlaying => currentAnimationCoroutine != null;
    bool CanPlay => spriteList != null && !IsPlaying && spriteList.Length > 0;
    protected int CurrentFrame { get; private set; }
    bool CanAutoPlay => spriteList != null && autoPlay;
    private int animDirection = 1;

    protected abstract Sprite sprite { get; set; }

    private void OnEnable()
    {
        if (CanAutoPlay)
            Play();
    }
    private void OnDisable()
    {
        Stop();
    }
    public void Play(AnimatedSpriteAsset newAsset, bool forceStart = false)
    {
        if (newAsset == null) return;
        spriteList = newAsset;
        Play(forceStart);
    }
    public void Play(bool forceStart = false)
    {
        animDirection = 1;
        _Play(forceStart);
    }
    public void PlayReverse(AnimatedSpriteAsset newAsset, bool forceStart = false)
    {
        if (newAsset == null) return;
        spriteList = newAsset;
        PlayReverse(forceStart);
    }
    public void PlayReverse(bool forceStart = false)
    {
        animDirection = -1;
        _Play(forceStart);
    }
    private void _Play(bool forceStart = false)
    {
        if (!CanPlay && !forceStart) return;

        CurrentFrame = animDirection == 1 ? 0 : spriteList.Length - 1;

        if (currentAnimationCoroutine != null)
            StopCoroutine(currentAnimationCoroutine);

        if (spriteList.Length > 1)
            currentAnimationCoroutine = StartCoroutine(AnimateSprite());
        else
            SetSprite(CurrentFrame);
    }
    public void Resume()
    {
        if (!CanPlay) return;

        currentAnimationCoroutine ??= StartCoroutine(AnimateSprite());
    }
    public void Next(int howMany = 1)
    {
        if (spriteList == null || spriteList.Length == 0) return;
        SetSprite((CurrentFrame += howMany) % spriteList.Length);
    }
    public void Stop()
    {
        if (!IsPlaying) return;
        StopCoroutine(currentAnimationCoroutine);
        currentAnimationCoroutine = null;
    }
    public void ResetAnimation(bool stop = true)
    {
        if (stop)
            Stop();
        ResetSprite();
    }
    private IEnumerator AnimateSprite()
    {
        float timePassed = 0;
        while (CurrentFrame < spriteList.Length && CurrentFrame >= 0)
        {
            SetSprite(CurrentFrame);
            CurrentFrame += animDirection;
            if (spriteList.Loop)
            {
                if (CurrentFrame >= spriteList.Length)
                    CurrentFrame = 0;
                else if (CurrentFrame < 0)
                    CurrentFrame = spriteList.Length - 1;
            }
            else if (CurrentFrame >= spriteList.Length || CurrentFrame <= -1)
            {
                currentAnimationCoroutine = null;
                if (animDirection == 1)
                    OnAnimationFinished.Invoke();
                else
                    OnReverseAnimationFinished.Invoke();
                yield break;
            }
            while (timePassed < spriteList.Frames[CurrentFrame].duration)
            {
                timePassed += Time.deltaTime;
                yield return null;
            }
            timePassed -= spriteList.Frames[CurrentFrame].duration;
        }
        currentAnimationCoroutine = null;
    }
    public void SetSprite(int index)
    {
        sprite = spriteList.Frames[index].sprite;
    }
    public void ResetSprite()
    {
        sprite = spriteList.Frames[0].sprite;
    }
}
