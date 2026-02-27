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