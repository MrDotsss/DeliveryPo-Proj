using UnityEngine;

public enum ItemType { Usable, Parcel, Regular }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("General Info")]
    public string itemName;
    [TextArea(3, 5)] public string description;
    public GameObject modelPrefab; // 3D Model
    public ItemType itemType;
    public Sprite icon; // UI representation
}
