using UnityEngine;
using UnityEngine.Events;

public class TreePanelSwitch : MonoBehaviour , IGuiPanel
{
    public UnityEvent<bool> switched;
    public void Switch()
    {
        TreeActiveGuiPanel.OpenPanel(this);
    }
    public bool IsPanelOn() => gameObject.activeSelf;
    public virtual void SetPanelVisibility(bool state){
        gameObject.SetActive(state);
        switched?.Invoke(state);
    }
}
