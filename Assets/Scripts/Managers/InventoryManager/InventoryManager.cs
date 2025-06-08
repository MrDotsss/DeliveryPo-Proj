using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Represents a single item instance in the inventory.
/// Contains a unique ID and reference to its ItemData (definition).
/// </summary>
[Serializable]
public class InventoryItem
{
    public string id;         // Unique identifier for this inventory item instance
    public ItemData data;     // Reference to the item data (definition)

    public InventoryItem(ItemData data)
    {
        id = Guid.NewGuid().ToString();
        this.data = data;
    }
}

/// <summary>
/// Container class for serializing a list of InventoryItems.
/// </summary>
[Serializable]
public class InventoryData
{
    public List<InventoryItem> items;

    public InventoryData(List<InventoryItem> items)
    {
        this.items = items;
    }
}

/// <summary>
/// Manager responsible for handling the player's inventory.
/// Supports adding, removing, equipping, dropping items and querying inventory contents.
/// </summary>
public class InventoryManager : BaseManager<InventoryManager>
{
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    // Events fired when items are added, removed, equipped, or dropped
    public event Action<InventoryItem> ItemAdded;
    public event Action<InventoryItem> ItemRemoved;
    public event Action<InventoryItem> OnEquip;
    public event Action<InventoryItem> OnDrop;

    public InventoryItem CurrentItem { get; private set; }

    private void Start()
    {
        // Load inventory data on start
        inventory = SaveNLoadManager.Instance.LoadInventory();
    }

    /// <summary>
    /// Adds an existing InventoryItem to the inventory.
    /// Optionally equips the item immediately.
    /// </summary>
    public InventoryItem AddItem(InventoryItem item, bool equip = false)
    {
        if (inventory.Contains(item))
        {
            Debug.LogError($"{item.data.itemName} already exists with the same ID: {item.id}");
            return item;
        }

        inventory.Add(item);
        ItemAdded?.Invoke(item);
        if (equip) EquipItem(item);

        return item;
    }

    /// <summary>
    /// Creates a new InventoryItem from ItemData and adds it to the inventory.
    /// Optionally equips the new item.
    /// </summary>
    public InventoryItem AddItem(ItemData data, bool equip = false)
    {
        InventoryItem item = new InventoryItem(data);

        inventory.Add(item);
        ItemAdded?.Invoke(item);
        if (equip) EquipItem(item);

        return item;
    }

    /// <summary>
    /// Removes an InventoryItem from the inventory.
    /// Automatically unequips it if it was equipped.
    /// </summary>
    public void RemoveItem(InventoryItem item)
    {
        if (item == CurrentItem) UnEquipItem();

        inventory.Remove(item);
        ItemRemoved?.Invoke(item);
    }

    /// <summary>
    /// Sets the given item as the currently equipped item.
    /// </summary>
    public void EquipItem(InventoryItem item)
    {
        if (inventory.Contains(item))
        {
            CurrentItem = item;
            OnEquip?.Invoke(item);
        }
    }

    /// <summary>
    /// Unequips the currently equipped item (if any).
    /// </summary>
    public void UnEquipItem()
    {
        if (CurrentItem != null)
        {
            CurrentItem = null;
            OnEquip?.Invoke(null);
        }
    }

    /// <summary>
    /// Drops an item by removing it from inventory and firing drop event.
    /// </summary>
    public void DropItem(InventoryItem item)
    {
        if (inventory.Contains(item))
        {
            RemoveItem(item);
            OnDrop?.Invoke(item);
        }
    }

    /// <summary>
    /// Finds an inventory item by matching its ItemData reference.
    /// </summary>
    public InventoryItem FindInventory(ItemData data)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.data == data) return item;
        }

        return null;
    }

    /// <summary>
    /// Finds an inventory item by matching the item's name.
    /// </summary>
    public InventoryItem FindInventory(string itemName)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.data.itemName == itemName) return item;
        }

        return null;
    }

    /// <summary>
    /// Returns an enumerable of inventory items excluding parcels.
    /// </summary>
    public IEnumerable<InventoryItem> GetInventoryList()
    {
        return inventory.Where(item => item.data.itemType != ItemType.Parcel);
    }

    /// <summary>
    /// Returns an enumerable of only parcel-type inventory items.
    /// </summary>
    public IEnumerable<InventoryItem> GetParcelList()
    {
        return inventory.Where(item => item.data.itemType == ItemType.Parcel);
    }

    /// <summary>
    /// Returns an enumerable of all inventory items.
    /// </summary>
    public IEnumerable<InventoryItem> GetAllInventoryList()
    {
        return inventory;
    }

    /// <summary>
    /// Removes all items of the specified type from the inventory.
    /// </summary>
    public void ClearInventory(ItemType type)
    {
        // Removing items inside foreach will cause issues; consider collecting first then removing.
        foreach (InventoryItem item in inventory)
        {
            if (item.data.itemType == type) inventory.Remove(item);
        }
    }
}
