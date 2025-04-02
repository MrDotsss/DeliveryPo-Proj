using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    Delivery,
    Main,
    Favor
}

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/New Quest")]
public class Quest : ScriptableObject
{
    public string questTitle;
    public QuestType questType;
    [TextArea(3, 5)] public string questDescription;
    [Header("Optional Item")]
    public List<ItemData> requiredItems = new List<ItemData>();
}
