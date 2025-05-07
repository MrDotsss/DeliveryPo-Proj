using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public string uniqueID;
    public ItemData itemData;

    public InventoryItem(ItemData data)
    {
        uniqueID = Guid.NewGuid().ToString();
        itemData = data;
    }
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    [Header("References")]
    public InventoryUI inventoryUI;

    [Header("Inventory Settings")]
    public int gridSize = 10;

    [Header("Current Equipped Item")]
    public InventoryItem equippedItem = new InventoryItem(null);

    [Header("Inventory Storage")]
    public List<InventoryItem> parcels = new List<InventoryItem>();
    public List<InventoryItem> inventory = new List<InventoryItem>();

    public event Action<InventoryItem> OnEquipItem;
    public event Action<InventoryItem> OnDropItem;

    public bool inventoryMode = false;

    private Player player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        inventoryUI.gameObject.SetActive(inventoryMode);
    }

    private void Update()
    {
        if (DialogueManager.Instance.IsDialogueActive) return;

        if (InputManager.Instance.input.actions["inventory"].WasPressedThisFrame())
        {
            inventoryMode = !inventoryMode;

            if (inventoryMode) OpenInventory();
            else CloseInventory();
        }
        else if (!inventoryMode && InputManager.Instance.input.actions["drop"].WasPressedThisFrame())
        {
            DropItem(equippedItem);
        }
    }

    public void OpenInventory()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.canInput = false;
        player.cam.canInput = false;

        inventoryMode = true;
        inventoryUI.gameObject.SetActive(true);
    }

    public void CloseInventory()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.canInput = true;
        player.cam.canInput = true;

        inventoryMode = false;
        inventoryUI.gameObject.SetActive(false);
    }

    public bool AddItem(ItemData item, bool equip = false)
    {
        InventoryItem newItem = new InventoryItem(item);

        if (item.itemType == ItemType.Parcel)
        {
            parcels.Add(newItem);
            if (equip) EquipItem(newItem);
        }
        else if (inventory.Count < gridSize)
        {
            inventory.Add(newItem);
            if (equip) EquipItem(newItem);
        }
        else return false; // Inventory full

        inventoryUI.PopulateGrid();
        return true;
    }

    public void RemoveItem(InventoryItem item)
    {
        parcels.Remove(item);
        inventory.Remove(item);
    }

    public void EquipItem(InventoryItem item)
    {
        if (equippedItem == item)
        {
            //Debug.LogWarning($"{item.itemData.itemName} is already equipped");
            return;
        }

        if (inventory.Contains(item) || parcels.Contains(item))
        {
            equippedItem = item;
            OnEquipItem?.Invoke(item);
            //Debug.Log($"Equipped: {equippedItem.itemData.itemName}");
        }
        else
        {
            //Debug.LogWarning("Item not in inventory!");
        }

        CloseInventory();
    }

    public void EquipItem(ItemData itemData)
    {
        if (equippedItem != null) return;

        foreach (InventoryItem item in inventory)
        {
            if (item.itemData == itemData)
            {
                equippedItem = item;
                OnEquipItem?.Invoke(item);
                //Debug.Log($"Equipped: {equippedItem.itemData.itemName}");
            }
            else
            {
                //Debug.LogWarning("Item not in inventory!");
            }
        }
        CloseInventory();
    }

    public void UnequipItem()
    {
        if (equippedItem != null)
        {
            equippedItem = null;
            OnEquipItem?.Invoke(null);
        }

        CloseInventory();
    }


    public void DropItem(InventoryItem item)
    {
        if (item == null) return;

        if (equippedItem == item) equippedItem = null;
        RemoveItem(item);
        OnDropItem?.Invoke(item);
        //Debug.Log($"Dropped: {item.itemData.itemName}");

        inventoryUI.PopulateGrid();

        CloseInventory();
    }

    public InventoryItem FindInventory(ItemData item)
    {
        foreach (InventoryItem inv in inventory)
        {
            if (inv.itemData == item)
            {
                return inv;
            }
        }

        return null;
    }

    public InventoryItem FindParcel(ItemData item)
    {
        foreach (InventoryItem inv in parcels)
        {
            if (inv.itemData == item)
            {
                return inv;
            }
        }

        return null;
    }

    public void ClearInventory()
    {
        inventory.Clear();
    }
}
