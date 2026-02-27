using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class TreeActiveGuiPanel {
    static Stack<IGuiPanel> openedPanels = new(4);
    public static UnityEvent<bool> OnPanelOpened = new();
    public static UnityEvent<bool> OnPanelClosed = new();
    public static bool HasActivePanel() => openedPanels.Count > 0;
#region INITIALIZATION
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialization()
    {
        SceneManager.activeSceneChanged += ActiveSceneChanged;
    }
    private static void ActiveSceneChanged(Scene _, Scene __)
    {
        openedPanels.Clear();
    }
#endregion
    public static void OpenPanel(IGuiPanel panel)
    {
        bool isSame = HasActivePanel() && openedPanels.Peek() == panel;
        CloseAllPanels();

        OnPanelOpened.Invoke(!isSame);

        if (isSame) return;
        panel.SetPanelVisibility(true);
        openedPanels.Push(panel);
    }
    public static void OpenPanelOnTop(IGuiPanel panel)
    {
        if (HasActivePanel())
            openedPanels.Peek().SetPanelVisibility(false);
        panel.SetPanelVisibility(true);
        openedPanels.Push(panel);
        OnPanelOpened.Invoke(true);
    }
    public static bool GoBack()
    {
        if (!HasActivePanel()) return false;

        openedPanels.Pop().SetPanelVisibility(false);
        
        var hasPanel = HasActivePanel();
        OnPanelClosed.Invoke(hasPanel);
        if (hasPanel)
            openedPanels.Peek().SetPanelVisibility(true);
        return true;
    }
    public static void CloseAllPanels()
    {
        foreach (var panel in openedPanels)
            panel.SetPanelVisibility(false);
        openedPanels.Clear();
    }
}