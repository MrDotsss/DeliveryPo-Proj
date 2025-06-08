using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the overall inventory UI, including switching between parcel and inventory tabs,
/// building the item lists dynamically, and handling UI state enter/exit behaviors.
/// Listens to InventoryManager events to update the UI when items are added or removed.
/// </summary>
public class InventoryUI : UIState
{
    // Prefab for individual inventory item UI elements
    public GameObject inventoryItemUIPrefab;

    // Buttons to switch between Parcel and Inventory tabs
    public Button parcelTabButton;
    public Button inventoryTabButton;

    // Parent GameObjects where item UI elements are instantiated for each tab
    public GameObject parcelViewParent;
    public GameObject inventoryViewParent;

    // Current active tab index (0 for Parcel, 1 for Inventory)
    public int tabIndex = 0;

    // UIState implementation: identifies this UI as Inventory panel
    public override UIManager.EUIPanels State => UIManager.EUIPanels.Inventory;

    /// <summary>
    /// Called when entering this UI state.
    /// Activates the UI, pauses the game, subscribes to inventory update events,
    /// and switches to the default tab (Parcel).
    /// </summary>
    public override void Enter(object data = null)
    {
        gameObject.SetActive(true);

        GameManager.Instance.RequestPause();

        InventoryManager.Instance.ItemAdded += ListUpdated;
        InventoryManager.Instance.ItemRemoved += ListUpdated;

        SwitchTab(0);
    }

    /// <summary>
    /// Called when exiting this UI state.
    /// Deactivates the UI, unpauses the game, and unsubscribes from inventory events.
    /// </summary>
    public override void Exit()
    {
        gameObject.SetActive(false);

        GameManager.Instance.RequestUnpause();

        InventoryManager.Instance.ItemAdded -= ListUpdated;
        InventoryManager.Instance.ItemRemoved -= ListUpdated;
    }

    /// <summary>
    /// Switches the current tab and rebuilds the UI list for that tab.
    /// Updates button colors and activates the corresponding view.
    /// </summary>
    /// <param name="index">Index of tab to switch to (0=Parcel, 1=Inventory)</param>
    public void SwitchTab(int index)
    {
        tabIndex = index;

        bool isParcel = tabIndex == 0;

        BuildUIList(isParcel ? InventoryManager.Instance.GetParcelList() : InventoryManager.Instance.GetInventoryList(),
            isParcel ? parcelViewParent : inventoryViewParent);

        parcelViewParent.SetActive(isParcel);
        inventoryViewParent.SetActive(!isParcel);

        parcelTabButton.image.color = isParcel ? Color.red : Color.white;
        inventoryTabButton.image.color = isParcel ? Color.white : Color.red;
    }

    /// <summary>
    /// Called when items are added or removed from inventory.
    /// Refreshes the current tab's UI list.
    /// </summary>
    private void ListUpdated(InventoryItem item)
    {
        SwitchTab(tabIndex);
    }

    /// <summary>
    /// Builds the UI list of inventory items under the given parent.
    /// Clears existing UI elements before instantiating new ones.
    /// Sets up click listeners on each item to toggle its option panel.
    /// </summary>
    /// <param name="itemList">Collection of inventory items to display</param>
    /// <param name="parent">Parent GameObject to hold item UI elements</param>
    private void BuildUIList(IEnumerable<InventoryItem> itemList, GameObject parent)
    {
        List<InventoryItem> items = itemList.ToList<InventoryItem>();

        ClearUIList(parent);

        for (int i = 0; i < items.Count; i++)
        {
            InventoryItemUI itemUI = Instantiate(inventoryItemUIPrefab, parent.transform).GetComponent<InventoryItemUI>();
            itemUI.item = items[i];

            itemUI.itemName.text = items[i].data.itemName;

            Button itemBtn = itemUI.GetComponent<Button>();
            itemBtn.onClick.AddListener(() =>
            {
                itemUI.ToggleOptions();
            });
        }
    }

    /// <summary>
    /// Clears all child UI elements from the specified parent GameObject.
    /// </summary>
    /// <param name="parent">Parent GameObject whose children will be destroyed</param>
    private void ClearUIList(GameObject parent)
    {
        if (parent.transform.childCount <= 0) return;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }
}
