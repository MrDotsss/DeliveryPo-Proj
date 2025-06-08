using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    public InventoryItem Item { get; private set; }
    public string InteractionText => $"Press 'F' to Pickup {itemData.itemName}";

    public void Interact()
    {
        if (Item == null)
        {
            InventoryManager.Instance.AddItem(itemData, true);
        }
        else
        {
            InventoryManager.Instance.AddItem(Item, true);
        }
        Destroy(gameObject);
    }

    public void SetItemData(InventoryItem item)
    {
        this.Item = item;
        this.itemData = item.data;
    }
}
