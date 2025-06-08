using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton UI Manager responsible for managing UI panels and their states.
/// Handles switching between UI panels and updating the current active panel.
/// </summary>
public class UIManager : BaseManager<UIManager>
{
    /// <summary>
    /// Enum representing all possible UI panels managed by UIManager.
    /// </summary>
    public enum EUIPanels
    {
        CursorPanel,
        PauseMenu,
        OptionMenu,
        MainMenu,
        SaveLoadMenu,
        Inventory,
        Inspection,
        Dialogue,
        Phone,
    }

    [Header("References")]
    private Dictionary<EUIPanels, UIState> panelDictionary = new Dictionary<EUIPanels, UIState>();
    private UIState _currentPanel;

    /// <summary>
    /// Gets the currently active UI panel enum.
    /// </summary>
    public EUIPanels CurrentPanel => _currentPanel.State;

    /// <summary>
    /// Flag to control whether UI panels can be switched.
    /// </summary>
    public bool canSwitch = true;

    /// <summary>
    /// Updates the current active panel's state every frame.
    /// </summary>
    private void Update()
    {
        _currentPanel?.UpdateState();
    }

    /// <summary>
    /// Switches from the current panel to the specified panel.
    /// Calls Exit() on the old panel and Enter() on the new panel.
    /// </summary>
    /// <param name="nextState">The panel to switch to.</param>
    /// <param name="data">Optional data to pass to the new panel.</param>
    public void SwitchPanel(EUIPanels nextState, object data = null)
    {
        if (!canSwitch) return;

        if (!panelDictionary.ContainsKey(nextState))
        {
            Debug.LogError($"{nextState} is not yet registered to UIManager");
            return;
        }

        _currentPanel?.Exit();
        _currentPanel = panelDictionary[nextState];
        _currentPanel?.Enter(data);
    }

    /// <summary>
    /// Toggles between the CursorPanel and a specified panel.
    /// If currently in CursorPanel, switches to the specified panel; otherwise switches back to CursorPanel.
    /// </summary>
    /// <param name="state">The panel to toggle to/from CursorPanel.</param>
    /// <param name="data">Optional data to pass when switching panels.</param>
    public void ToggleCurrentPanel(EUIPanels state, object data = null)
    {
        if (_currentPanel.State == EUIPanels.CursorPanel)
        {
            SwitchPanel(state, data);
        }
        else
        {
            SwitchPanel(EUIPanels.CursorPanel, data);
        }
    }

    /// <summary>
    /// Registers a new UI panel into the manager's dictionary for management.
    /// </summary>
    /// <param name="newPanel">The UIState panel instance to register.</param>
    public void RegisterUIPanel(UIState newPanel)
    {
        if (!panelDictionary.ContainsKey(newPanel.State))
        {
            panelDictionary[newPanel.State] = newPanel;
            Debug.Log($"{newPanel.State} registered to UIManager");
        }
    }

    /// <summary>
    /// Gets the UIState panel instance for the given panel enum.
    /// </summary>
    /// <param name="uiState">The enum of the UI panel to retrieve.</param>
    /// <returns>The UIState instance of the requested panel.</returns>
    public UIState GetUIPanel(EUIPanels uiState)
    {
        return panelDictionary[uiState];
    }
}
