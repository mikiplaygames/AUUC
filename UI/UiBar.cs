using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiBar : MonoBehaviour
{
    [SerializeField] protected Image bar;
    
    public UnityEvent OnBarChanged = new();
    
    protected virtual void Awake() {
        if (bar == null)
            bar = GetComponentInChildren<Image>(true);
    }
    public virtual void BarChanged(float percentage)
    {
        bar.fillAmount = percentage;
        OnBarChanged?.Invoke();
    }
}
