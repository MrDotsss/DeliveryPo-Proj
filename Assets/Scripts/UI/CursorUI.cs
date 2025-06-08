using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorUI : UIState
{
    [Header("References")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private TextMeshProUGUI cursorText;

    [Header("Properties")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color interactColor;

    public override UIManager.EUIPanels State => UIManager.EUIPanels.CursorPanel;

    public override void Enter(object data = null)
    {
        gameObject.SetActive(true);
        InputManager.Instance.SetCursorLock(true);

        cursorImage.color = defaultColor;
        cursorText.text = string.Empty;

    }

    public override void Exit()
    {
        gameObject.SetActive(false);
        InputManager.Instance.SetCursorLock(false);

        cursorImage.color = defaultColor;
        cursorText.text = string.Empty;
    }

    public void SetInteractCursor(string text, bool active)
    {
        cursorText.text = active ? text : string.Empty;
        cursorImage.color = active ? interactColor : defaultColor;
    }
}
