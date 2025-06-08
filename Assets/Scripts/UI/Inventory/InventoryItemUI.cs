using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    public InventoryItem item;

    [SerializeField] private RectTransform rect;
    [SerializeField] private GameObject optionPanel;

    public TextMeshProUGUI itemName;

    [SerializeField] private Button EquipButton;
    [SerializeField] private TextMeshProUGUI equipButtonText;

    [SerializeField] private Button InspectButton;
    [SerializeField] private Button DropButton;


    private void Start()
    {
        InventoryManager.Instance.OnEquip += ListenInventoryEquip;

        EquipButton.onClick.AddListener(ListenEquip);
        InspectButton.onClick.AddListener(ListenInspect);
        DropButton.onClick.AddListener(ListenDrop);

        equipButtonText.text = item.data.itemName;

        UpdateButton();
        CollapseOptions();
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnEquip -= ListenInventoryEquip;

        EquipButton.onClick.RemoveAllListeners();
        InspectButton.onClick.RemoveAllListeners();
        DropButton.onClick.RemoveAllListeners();
    }

    private void ListenInventoryEquip(InventoryItem invItem)
    {
        UpdateButton();
    }

    private void ListenEquip()
    {
        if (InventoryManager.Instance.CurrentItem == item)
        {
            InventoryManager.Instance.UnEquipItem();
        }
        else
        {
            InventoryManager.Instance.EquipItem(item);
        }

        UpdateButton();
    }

    private void ListenInspect()
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Inspection,
            item);
    }

    private void ListenDrop()
    {
        InventoryManager.Instance.DropItem(item);
        Destroy(gameObject, 0.1f);
    }

    private void UpdateButton()
    {
        if (InventoryManager.Instance.CurrentItem == item)
        {
            equipButtonText.text = "UnEquip";
        }
        else
        {
            equipButtonText.text = "Equip";
        }
    }

    public void ToggleOptions()
    {
        if (!gameObject.activeSelf) return;

        optionPanel.SetActive(!optionPanel.activeSelf);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, optionPanel.activeSelf ? 200f : 100f);
    }

    public void CollapseOptions()
    {
        if (!gameObject.activeSelf) return;

        optionPanel.SetActive(false);

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 100f);
    }
}
