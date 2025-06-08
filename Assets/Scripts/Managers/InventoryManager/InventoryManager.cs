using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string id;
    public ItemData data;

    public InventoryItem(ItemData data)
    {
        id = Guid.NewGuid().ToString();
        this.data = data;
    }
}

[Serializable]
public class InventoryData
{
    public List<InventoryItem> items;

    public InventoryData(List<InventoryItem> items)
    {
        this.items = items;
    }
}

public class InventoryManager : BaseManager<InventoryManager>
{
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    public event Action<InventoryItem> ItemAdded;
    public event Action<InventoryItem> ItemRemoved;
    public event Action<InventoryItem> OnEquip;
    public event Action<InventoryItem> OnDrop;

    public InventoryItem CurrentItem { get; private set; }

    private void Start()
    {
        inventory = SaveNLoadManager.Instance.LoadInventory();
    }
  
    public InventoryItem AddItem(InventoryItem item, bool equip = false)
    {
        if (inventory.Contains(item))
        {
            Debug.LogError($"{item.data.itemName} already exists with the same ID: {item.id}");
            return item;
        }

        inventory.Add(item);
        ItemAdded?.Invoke(item);
        if(equip) EquipItem(item);

        return item;
    }

    public InventoryItem AddItem(ItemData data, bool equip = false)
    {
        InventoryItem item = new InventoryItem(data);

        inventory.Add(item);
        ItemAdded?.Invoke(item);
        if (equip) EquipItem(item);

        return item;
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == CurrentItem) UnEquipItem();

        inventory.Remove(item);
        ItemRemoved?.Invoke(item);
    }

    public void EquipItem(InventoryItem item)
    {
        if (inventory.Contains(item))
        {
            CurrentItem = item;
            OnEquip?.Invoke(item);
        }
    }

    public void UnEquipItem()
    {
        if (CurrentItem != null)
        {
            CurrentItem = null;
            OnEquip?.Invoke(null);
        }
    }

    public void DropItem(InventoryItem item)
    {
        if (inventory.Contains(item))
        {
            RemoveItem(item);

            OnDrop?.Invoke(item);
        }
    }

    public InventoryItem FindInventory(ItemData data)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.data == data) return item;
        }

        return null;
    }

    public InventoryItem FindInventory(string itemName)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.data.itemName == itemName) return item;
        }

        return null;
    }

    public IEnumerable<InventoryItem> GetInventoryList()
    {
        return inventory.Where(item => item.data.itemType != ItemType.Parcel);
    }

    public IEnumerable<InventoryItem> GetParcelList()
    {
        return inventory.Where(item => item.data.itemType == ItemType.Parcel);
    }

    public IEnumerable<InventoryItem> GetAllInventoryList()
    {
        return inventory;
    }

    public void ClearInventory(ItemType type)
    {
        foreach (InventoryItem item in inventory)
        {
            if (item.data.itemType == type) inventory.Remove(item);
        }
    }
}
