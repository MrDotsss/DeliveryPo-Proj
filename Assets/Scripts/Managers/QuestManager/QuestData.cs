using System.Collections.Generic;
using UnityEngine;

public enum QuestType { Delivery, Main, Favor, None }

[CreateAssetMenu(fileName ="NewQuest", menuName = "Quest")]
public class QuestData : ScriptableObject
{
    [Header("Data")]
    public string questTitle;
    [TextArea(5, 8)] public string questDescription;
    public QuestType questType;

    [Header("Required Items 'Optional/Delivery'")]
    public List<ItemData> items;
}
