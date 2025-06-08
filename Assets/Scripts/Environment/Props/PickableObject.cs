using UnityEngine;

/// <summary>
/// Represents an interactable object that can be picked up by the player.
/// Implements IInteractable interface for interaction functionality.
/// </summary>
public class PickableObject : MonoBehaviour, IInteractable
{
    public ItemData itemData;

    public InventoryItem Item { get; private set; }

    /// <summary>
    /// Text displayed to indicate the interaction prompt to pick up the item.
    /// </summary>
    public string InteractionText => $"Press 'F' to Pickup {itemData.itemName}";

    /// <summary>
    /// Handles interaction: adds the item to the inventory and destroys the game object.
    /// </summary>
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

    /// <summary>
    /// Sets the item data and InventoryItem reference for this pickable object.
    /// </summary>
    /// <param name="item">InventoryItem to assign.</param>
    public void SetItemData(InventoryItem item)
    {
        this.Item = item;
        this.itemData = item.data;
    }
}
