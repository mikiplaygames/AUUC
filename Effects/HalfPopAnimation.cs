using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HalfPopAnimation : MonoBehaviour
{
    protected Vector3 minScale;
    [SerializeField] public Vector3 maxScale;
    [SerializeField] protected float speed = 2f;
    [SerializeField] protected float duration = 0.1f;
    [SerializeField] protected bool autoPlay = false;

    public UnityEvent popFinished;
    private Vector3 initialScale = Vector3.one;

    private void Awake()
    {
        initialScale = transform.localScale;
        SetMaxScale(maxScale);
        if (autoPlay)
            Pop();
    }
    
    public void Pop(float delay = 0)
    {
        if (gameObject.activeInHierarchy) StartCoroutine(Animation(delay));
    }

    protected virtual IEnumerator Animation(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return RepeatLerp(transform, minScale, maxScale, duration, speed);
        popFinished.Invoke();
    }
    public static IEnumerator RepeatLerp(Transform objectTransform, Vector3 startScale, Vector3 stopScale, float time, float speed)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            objectTransform.localScale = Vector3.Lerp(startScale, stopScale, i);
            yield return null;
        }
        objectTransform.localScale = stopScale;
    }
    public void SetMaxScale(Vector3 newScale)
    {
        maxScale = new Vector3(initialScale.x * newScale.x, initialScale.y * newScale.y, initialScale.z);
        maxScale += initialScale;
        minScale = initialScale;
    }

    public void ResetScale()
    {
        transform.localScale = initialScale;
    }
}
