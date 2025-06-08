using System.Collections.Generic;
using UnityEngine;

public class UIManager : BaseManager<UIManager>
{
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

    public EUIPanels CurrentPanel => _currentPanel.State;

    public bool canSwitch = true;

    private void Update()
    {
        _currentPanel?.UpdateState();
    }

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

    public void RegisterUIPanel(UIState newPanel)
    {
        if (!panelDictionary.ContainsKey(newPanel.State))
        {
            panelDictionary[newPanel.State] = newPanel;
            Debug.Log($"{newPanel.State} registered to UIManager");
        }
    }

    public UIState GetUIPanel(EUIPanels uiState)
    {
        return panelDictionary[uiState];
    }
}