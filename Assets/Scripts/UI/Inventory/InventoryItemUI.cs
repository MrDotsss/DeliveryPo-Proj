using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the UI element for a single inventory item.
/// Handles displaying item info, toggling options (Equip, Inspect, Drop),
/// and responding to user input for these actions.
/// Updates button text based on equip state and manages option panel visibility.
/// Subscribes to InventoryManager events to stay in sync with item equip status.
/// </summary>
public class InventoryItemUI : MonoBehaviour
{
    // The InventoryItem this UI element represents
    public InventoryItem item;

    // Reference to RectTransform to adjust size when toggling option panel
    [SerializeField] private RectTransform rect;

    // The panel containing action buttons (Equip, Inspect, Drop)
    [SerializeField] private GameObject optionPanel;

    // Text displaying the item name
    public TextMeshProUGUI itemName;

    // Buttons for item actions
    [SerializeField] private Button EquipButton;
    [SerializeField] private TextMeshProUGUI equipButtonText;  // Text displayed on equip button

    [SerializeField] private Button InspectButton;
    [SerializeField] private Button DropButton;

    private void Start()
    {
        // Subscribe to equip event for UI updates
        InventoryManager.Instance.OnEquip += ListenInventoryEquip;

        // Setup button click listeners
        EquipButton.onClick.AddListener(ListenEquip);
        InspectButton.onClick.AddListener(ListenInspect);
        DropButton.onClick.AddListener(ListenDrop);

        // Set initial equip button text to item name (overwritten in UpdateButton)
        equipButtonText.text = item.data.itemName;

        UpdateButton();   // Update button text based on current equip state
        CollapseOptions(); // Hide option panel initially
    }

    private void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        InventoryManager.Instance.OnEquip -= ListenInventoryEquip;

        // Remove all listeners from buttons
        EquipButton.onClick.RemoveAllListeners();
        InspectButton.onClick.RemoveAllListeners();
        DropButton.onClick.RemoveAllListeners();
    }

    // Called when any item is equipped or unequipped to update this UI's button state
    private void ListenInventoryEquip(InventoryItem invItem)
    {
        UpdateButton();
    }

    // Called when Equip button is pressed
    private void ListenEquip()
    {
        if (InventoryManager.Instance.CurrentItem == item)
        {
            InventoryManager.Instance.UnEquipItem(); // Unequip if already equipped
        }
        else
        {
            InventoryManager.Instance.EquipItem(item); // Equip this item otherwise
        }

        UpdateButton(); // Update button text to match new state
    }

    // Called when Inspect button is pressed, switches to the inspection UI for this item
    private void ListenInspect()
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Inspection, item);
    }

    // Called when Drop button is pressed, drops the item and destroys this UI element shortly after
    private void ListenDrop()
    {
        InventoryManager.Instance.DropItem(item);
        Destroy(gameObject, 0.1f);
    }

    // Updates the equip button text based on whether this item is currently equipped
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

    // Toggles the option panel visibility and adjusts UI element height accordingly
    public void ToggleOptions()
    {
        if (!gameObject.activeSelf) return;

        optionPanel.SetActive(!optionPanel.activeSelf);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, optionPanel.activeSelf ? 200f : 100f);
    }

    // Collapses the option panel and resets UI element height
    public void CollapseOptions()
    {
        if (!gameObject.activeSelf) return;

        optionPanel.SetActive(false);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 100f);
    }
}
