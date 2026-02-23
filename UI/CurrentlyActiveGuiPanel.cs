using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public static class CurrentlyActiveGuiPanel
{
    public static UnityEvent OnEscClose = new();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialization()
    {
        SceneManager.activeSceneChanged += ActiveSceneChanged;
        //!!!! nie zapomniec ze cos takego istanieje
        // BabushkaContainer.OnPickedUp.AddListener(() => CloseCurrentlyActivePanels());
        /// TO ZAMYKA GDY X
    }
    private static void ActiveSceneChanged(Scene _, Scene __)
    {
        activePanelOnTop = null;
        currentlyActivePanel = null; //todo for later: jesli kiedykolwiek bedzie jakies bardziej zaawansowane guwno zwiazane z ladowaniem scen to bedzie problem chyba
    }
    private static IGuiPanel activePanelOnTop;
    private static IGuiPanel currentlyActivePanel;
    public static bool EscButtonUsed()
    {
        if (HasActivePanelOnTop())
        {
            OnEscClose.Invoke();
            activePanelOnTop.SetPanelVisibility(false);
            activePanelOnTop = null;
            return true;
        }
        if (HasActivePanel())
        {
            OnEscClose.Invoke();
            currentlyActivePanel.SetPanelVisibility(false);
            currentlyActivePanel = null;
            return true;
        }
        return false;
    }
    public static void SetActivePanelOnTop(IGuiPanel panel) => ChangePanels(panel, ref activePanelOnTop);
    public static void SetCurrentlyActivePanel(IGuiPanel panel) => ChangePanels(panel, ref currentlyActivePanel);
    static void ChangePanels(IGuiPanel newPanel, ref IGuiPanel sourcePanel)
    {
        if (sourcePanel == newPanel)
        {
            sourcePanel.SetPanelVisibility(false);
            sourcePanel = null;
            return;
        }
        newPanel.SetPanelVisibility(true);
        sourcePanel = newPanel;
    }
/// <summary>
/// use this when you will close the panel somewhere else and will <br/>
/// not rely on this CurrentlyActivePanel script to close it for ya
/// </summary>
/// <param name="supposedPanel"></param>
    public static void SelfCloseActivePanel(IGuiPanel supposedPanel)
    {
        if (currentlyActivePanel == supposedPanel)
            currentlyActivePanel = null;
    }
    public static void CloseCurrentlyActivePanel(IGuiPanel panel)
    {
        if (currentlyActivePanel != panel) return;
        currentlyActivePanel.SetPanelVisibility(false);
        currentlyActivePanel = null;
    }
    public static void CloseCurrentlyActivePanels(bool includeOnTop = true)
    {
        if (includeOnTop && HasActivePanelOnTop())
        {
            activePanelOnTop.SetPanelVisibility(false);
            activePanelOnTop = null;
        }
        if (!HasActivePanel())
            return;
        currentlyActivePanel.SetPanelVisibility(false);
        currentlyActivePanel = null;
    }
    public static bool HasActivePanel() => currentlyActivePanel != null;
    public static bool HasActivePanelOnTop() => activePanelOnTop != null;
}
public interface IGuiPanel // stare sluzy do paneli ingame
{
    /// <summary>
    /// here should be code that will literally show/hide the panel
    /// </summary>
    /// <param name="state">visible or not</param>
    public void SetPanelVisibility(bool state);
    /// <summary>
    /// returns true if the panel is currently visible (if close animation is playing it will return false)
    /// </summary>
    /// <returns></returns>
    public bool IsPanelOn();
}