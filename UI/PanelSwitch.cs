using UnityEngine;
using UnityEngine.Events;

public class PanelSwitch : MonoBehaviour , IGuiPanel
{
    public UnityEvent<bool> switched;
    public void Switch()
    {
        CurrentlyActiveGuiPanel.SetCurrentlyActivePanel(this);
    }
    public bool IsPanelOn() => gameObject.activeSelf;
    public virtual void SetPanelVisibility(bool state){
        gameObject.SetActive(state);
        switched?.Invoke(state);
    }
}
