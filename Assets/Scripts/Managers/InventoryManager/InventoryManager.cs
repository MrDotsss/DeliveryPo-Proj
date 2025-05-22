using System;
using System.Collections.Generic;
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

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    [SerializeField] private List<InventoryItem> parcels = new List<InventoryItem>();
    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    public event Action<InventoryItem> ItemAdded;
    public event Action<InventoryItem> ItemRemoved;
    public event Action<InventoryItem> OnEquip;
    public event Action<InventoryItem> OnDrop;

    public InventoryItem CurrentItem { get; private set; }

    public void AddItem(ItemData data, bool equip = false)
    {
        InventoryItem item = new InventoryItem(data);

        if (data.itemType == ItemType.Parcel)
        {
            parcels.Add(item);
            ItemAdded?.Invoke(item);
        }
        else
        {
            inventory.Add(item);
            ItemAdded?.Invoke(item);
        }

        if (equip) EquipItem(item);
    }

    public void AddItem(InventoryItem item, bool equip = false)
    {

        if (parcels.Contains(item) || inventory.Contains(item))
        {
            Debug.LogError($"{item.data.itemName} already existed with same id: {item.id}");
            return;
        }

        if (item.data.itemType == ItemType.Parcel)
        {
            parcels.Add(item);
            ItemAdded?.Invoke(item);
        }
        else
        {
            inventory.Add(item);
            ItemAdded?.Invoke(item);
        }

        if (equip) EquipItem(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        if (parcels.Contains(item) || inventory.Contains(item))
        {
            if (item == CurrentItem) UnEquipItem();
            ItemRemoved?.Invoke(item);
        }
    }

    public void EquipItem(InventoryItem item)
    {
        if (parcels.Contains(item) || inventory.Contains(item))
        {
            CurrentItem = item;
            OnEquip?.Invoke(item);
        }
    }

    public void UnEquipItem()
    {
        CurrentItem = null;
        OnEquip?.Invoke(null);
    }

    public void DropItem(InventoryItem item)
    {
        if (parcels.Contains(item) || inventory.Contains(item))
        {
            CurrentItem = null;
            OnDrop?.Invoke(item);
        }
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }

    public void ClearParcels()
    {
        parcels.Clear();
    }
}
