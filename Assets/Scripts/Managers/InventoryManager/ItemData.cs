using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration of different types of items.
/// </summary>
public enum ItemType { Regular, Parcel, Usable }

/// <summary>
/// ScriptableObject representing an item in the inventory.
/// Holds item information, description, type, and visual representation.
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    // The display name of the item
    public string itemName;

    // A detailed description of the item, editable with a multiline text area
    [TextArea(3, 5)] public string description;

    // The type/category of the item (Regular, Parcel, or Usable)
    public ItemType itemType;

    [Header("Visual")]
    // The prefab GameObject to instantiate for this item (e.g., 3D model)
    public GameObject itemPrefab;

    // The UI icon representing this item in inventory or UI
    public Sprite icon;
}
