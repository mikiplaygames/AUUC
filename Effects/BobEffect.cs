using System.Collections;
using UnityEngine;

public class BobEffect : MonoBehaviour
{
    [SerializeField] private float bobAmount;
    [SerializeField] private float duration = .5f;
    [SerializeField] private bool playOnAwake;

    private float defaultY;
    public bool IsBobbing => bobCoroutine != null;
    private Coroutine bobCoroutine;
    private void Awake()
    {
        defaultY = transform.localPosition.y;
        if (playOnAwake) StartBob();
    }
    public void StartBob()
    {
        if (IsBobbing) return;
        bobCoroutine ??= StartCoroutine(BobAnimation());
    }
    public void StopBob()
    {
        if (!IsBobbing) return;
        StopCoroutine(bobCoroutine);
        bobCoroutine = null;
        LeanTween.cancel(gameObject);
        LeanTween.moveLocalY(gameObject, defaultY, .5f);
    }

    private IEnumerator BobAnimation()
    {
        while (true)
        {
            // Bob up
            LeanTween.moveLocalY(gameObject, bobAmount * 0.1f, duration)
                .setEaseInOutSine();
            
            yield return new WaitForSeconds(duration); // Wait for bob up to complete
            
            // Bob down
            LeanTween.moveLocalY(gameObject, defaultY, duration)
                .setEaseInOutSine();
            
            yield return new WaitForSeconds(duration); // Wait for bob down to complete
        }
    }
}
