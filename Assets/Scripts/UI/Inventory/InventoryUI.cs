using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class InventoryUI : UIState
{
    public GameObject inventoryItemUIPrefab;

    public Button parcelTabButton;
    public Button inventoryTabButton;

    public GameObject parcelViewParent;
    public GameObject inventoryViewParent;

    public int tabIndex = 0;

    public override UIManager.EUIPanels State => UIManager.EUIPanels.Inventory;

    public override void Enter(object data = null)
    {
        gameObject.SetActive(true);

        GameManager.Instance.RequestPause();

        InventoryManager.Instance.ItemAdded += ListUpdated;
        InventoryManager.Instance.ItemRemoved += ListUpdated;

        SwitchTab(0);
    }

    public override void Exit()
    {
        gameObject.SetActive(false);

        GameManager.Instance.RequestUnpause();

        InventoryManager.Instance.ItemAdded -= ListUpdated;
        InventoryManager.Instance.ItemRemoved -= ListUpdated;
    }

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

    private void ListUpdated(InventoryItem item)
    {
        SwitchTab(tabIndex);
    }

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

    private void ClearUIList(GameObject parent)
    {
        if (parent.transform.childCount <= 0) return;

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Destroy(parent.transform.GetChild(i).gameObject);
        }
    }
}
