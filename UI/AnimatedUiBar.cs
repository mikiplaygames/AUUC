using UnityEngine;

public class AnimatedUiBar : UiBar {
    Coroutine animateCoroutine;
    public void AnimateProgress(float duration)
    {
        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateProgressCoroutine(duration));
    }
    private System.Collections.IEnumerator AnimateProgressCoroutine(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = Mathf.Clamp01(elapsedTime / duration);
            bar.fillAmount = Mathf.Lerp(0, 1f, percentage);
            yield return null;
        }
        animateCoroutine = null;
        bar.fillAmount = 1f;
    }
}