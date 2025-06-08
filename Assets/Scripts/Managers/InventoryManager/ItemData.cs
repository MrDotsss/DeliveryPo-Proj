using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Regular, Parcel, Usable}

[CreateAssetMenu(fileName = "NewItem", menuName ="Inventory")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string itemName;
    [TextArea(3, 5)] public string description;
    public ItemType itemType;

    [Header("Visual")]
    public GameObject itemPrefab;
    public Sprite icon;
}
