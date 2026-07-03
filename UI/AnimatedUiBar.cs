using UnityEngine;

public class AnimatedUiBar : UiBar {
    Coroutine animateCoroutine;
    public void AnimateProgress(float duration, float timeAlreadyElapsed = 0)
    {
        if (!gameObject.activeInHierarchy)
            return;
        if (animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateProgressCoroutine(duration, timeAlreadyElapsed));
    }
    private System.Collections.IEnumerator AnimateProgressCoroutine(float duration, float elapsedTime)
    {
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