using System.Collections;
using UnityEngine.Events;
using UnityEngine;

public class PopAnimation : HalfPopAnimation
{
    protected override IEnumerator Animation(float delay)
    {
        yield return new WaitForSeconds(delay);
        yield return RepeatLerp(transform, minScale, maxScale, duration, speed);
        yield return RepeatLerp(transform, maxScale, minScale, duration, speed);
        popFinished.Invoke();
    }
}
