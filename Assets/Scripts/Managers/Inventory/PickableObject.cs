using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour, IInteractable
{
    public InventoryItem inventoryItem;

 
    private void Start()
    {
        if (inventoryItem == null)
        {
            Debug.LogError($"There is no corresponding data to the {inventoryItem}");
        }
    }

    public void Interact()
    {
        if (inventoryItem == null) return;
        PlayerInventory.Instance.AddItem(inventoryItem.itemData, true);
        Destroy(gameObject);
    }

    public string GetInteractionText()
    {
        return $"Pickup {inventoryItem.itemData.itemName}";
    }
}
