using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Types of quests available in the game.
/// </summary>
public enum QuestType
{
    Delivery,   // Quest involves delivering items
    Main,       // Main storyline quest
    Favor,      // Optional favor quest for NPCs
    None        // No specific type assigned
}

/// <summary>
/// ScriptableObject representing quest data including title,
/// description, type, and required items.
/// </summary>
[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest")]
public class QuestData : ScriptableObject
{
    [Header("Data")]
    /// <summary>
    /// The quest title shown to the player.
    /// </summary>
    public string questTitle;

    /// <summary>
    /// Detailed description or instructions for the quest.
    /// </summary>
    [TextArea(5, 8)]
    public string questDescription;

    /// <summary>
    /// The quest's category/type.
    /// </summary>
    public QuestType questType;

    [Header("Required Items 'Optional/Delivery'")]
    /// <summary>
    /// List of items required for completing the quest,
    /// typically used for delivery quests but optional otherwise.
    /// </summary>
    public List<ItemData> items;
}
