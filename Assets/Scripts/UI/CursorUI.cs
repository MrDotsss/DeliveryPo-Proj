using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIState implementation for the cursor panel.
/// Handles displaying the cursor image and interaction text,
/// as well as managing cursor lock state.
/// </summary>
public class CursorUI : UIState
{
    [Header("References")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private TextMeshProUGUI cursorText;

    [Header("Properties")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color interactColor;

    /// <summary>
    /// Identifies this UIState as the CursorPanel.
    /// </summary>
    public override UIManager.EUIPanels State => UIManager.EUIPanels.CursorPanel;

    /// <summary>
    /// Called when the cursor UI panel is entered.
    /// Enables the panel, locks the cursor,
    /// and resets cursor appearance and text.
    /// </summary>
    /// <param name="data">Optional data passed on panel enter (ignored).</param>
    public override void Enter(object data = null)
    {
        gameObject.SetActive(true);
        InputManager.Instance.SetCursorLock(true);

        cursorImage.color = defaultColor;
        cursorText.text = string.Empty;
    }

    /// <summary>
    /// Called when the cursor UI panel is exited.
    /// Disables the panel, unlocks the cursor,
    /// and resets cursor appearance and text.
    /// </summary>
    public override void Exit()
    {
        gameObject.SetActive(false);
        InputManager.Instance.SetCursorLock(false);

        cursorImage.color = defaultColor;
        cursorText.text = string.Empty;
    }

    /// <summary>
    /// Sets the cursor appearance to indicate interaction availability.
    /// Changes the cursor text and color based on whether interaction is active.
    /// </summary>
    /// <param name="text">Text to display when interacting.</param>
    /// <param name="active">Whether interaction is active or not.</param>
    public void SetInteractCursor(string text, bool active)
    {
        cursorText.text = active ? text : string.Empty;
        cursorImage.color = active ? interactColor : defaultColor;
    }
}
