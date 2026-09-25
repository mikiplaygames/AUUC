using UnityEngine;

public class ColorfullUiBar : UiBar
{
    [SerializeField] Gradient gradient;
    public override void BarChanged(float percentage)
    {
        base.BarChanged(percentage);
        bar.color = gradient.Evaluate(percentage);
    }
}
