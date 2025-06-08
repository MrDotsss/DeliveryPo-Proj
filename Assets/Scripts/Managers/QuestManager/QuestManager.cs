using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a quest instance with unique ID, quest data, and state flags.
/// </summary>
[Serializable]
public class Quest
{
    public string id;
    public QuestData data;
    public bool finished = false;
    public bool accepted = false;

    public Quest(QuestData data)
    {
        id = Guid.NewGuid().ToString();
        this.data = data;
    }
}

/// <summary>
/// Manages the player's quests including adding, removing, updating, and querying quests.
/// </summary>
public class QuestManager : BaseManager<QuestManager>
{
    [SerializeField] private List<Quest> quests = new List<Quest>();

    public event Action<Quest> OnQuestAdded;
    public event Action<Quest> OnQuestRemoved;
    public event Action<Quest, Dictionary<string, object>> OnQuestUpdated;
    public event Action<Quest> OnQuestFinished;

    private void Start()
    {
        quests = SaveNLoadManager.Instance.LoadQuests();
    }

    /// <summary>
    /// Adds a quest instance if it does not already exist.
    /// </summary>
    public Quest AddQuest(Quest quest)
    {
        if (!quests.Contains(quest))
        {
            quests.Add(quest);
            OnQuestAdded?.Invoke(quest);
            return quest;
        }
        else
        {
            Debug.LogWarning($"{quest.data.questTitle} already exists with id: {quest.id}");
            return quests.Find(item => item.data == quest.data);
        }
    }

    /// <summary>
    /// Adds a new quest from QuestData if it does not already exist.
    /// </summary>
    public Quest AddQuest(QuestData data)
    {
        Quest quest = quests.Find(item => item.data == data);

        if (quest != null)
        {
            Debug.LogWarning($"{data.questTitle} already exists with id: {quest.id}");
            return quest;
        }
        else
        {
            quest = new Quest(data);
            quests.Add(quest);
            OnQuestAdded?.Invoke(quest);
            return quest;
        }
    }

    /// <summary>
    /// Removes a quest from the quest list.
    /// </summary>
    public void RemoveQuest(Quest quest)
    {
        quests.Remove(quest);
        OnQuestRemoved?.Invoke(quest);
    }

    /// <summary>
    /// Retrieves a quest by QuestData reference.
    /// </summary>
    public Quest GetQuest(QuestData data)
    {
        return quests.Find(item => item.data == data);
    }

    /// <summary>
    /// Updates quest state with additional data if the quest exists.
    /// </summary>
    public void UpdateQuest(Quest quest, Dictionary<string, object> data)
    {
        if (quests.Contains(quest))
        {
            OnQuestUpdated?.Invoke(quest, data);
        }
        else
        {
            Debug.Log($"'{quest.data.questTitle}' has not started yet.");
        }
    }

    /// <summary>
    /// Marks a quest as finished and triggers a checkpoint save.
    /// </summary>
    public void FinishQuest(Quest quest)
    {
        if (quest.finished)
        {
            Debug.LogError($"'{quest.data.questTitle}' is already finished.");
            return;
        }

        quest.finished = true;
        SaveNLoadManager.Instance.MarkCheckpoint();
        OnQuestFinished?.Invoke(quest);
    }

    /// <summary>
    /// Returns all active quests optionally filtered by quest type.
    /// </summary>
    public List<Quest> GetActiveQuest(QuestType type = QuestType.None)
    {
        if (type == QuestType.None)
        {
            return quests;
        }
        else
        {
            List<Quest> activeQuests = new List<Quest>();

            foreach (Quest quest in quests)
            {
                if (quest.data.questType == type)
                {
                    activeQuests.Add(quest);
                }
            }

            return activeQuests;
        }
    }

    /// <summary>
    /// Returns all finished quests optionally filtered by quest type.
    /// </summary>
    public List<Quest> GetFinishedQuest(QuestType type = QuestType.None)
    {
        List<Quest> finishedQuests = new List<Quest>();

        foreach (Quest quest in quests)
        {
            if (quest.finished)
            {
                switch (type)
                {
                    case QuestType.Main:
                    case QuestType.Delivery:
                    case QuestType.Favor:
                        if (quest.data.questType == type)
                            finishedQuests.Add(quest);
                        break;
                    default:
                        finishedQuests.Add(quest);
                        break;
                }
            }
        }

        return finishedQuests;
    }

    /// <summary>
    /// Returns all unfinished quests optionally filtered by quest type.
    /// </summary>
    public List<Quest> GetUnfinishedQuest(QuestType type = QuestType.None)
    {
        List<Quest> unfinishedQuests = new List<Quest>();

        foreach (Quest quest in quests)
        {
            if (!quest.finished)
            {
                switch (type)
                {
                    case QuestType.Main:
                    case QuestType.Delivery:
                    case QuestType.Favor:
                        if (quest.data.questType == type)
                            unfinishedQuests.Add(quest);
                        break;
                    default:
                        unfinishedQuests.Add(quest);
                        break;
                }
            }
        }

        return unfinishedQuests;
    }

    /// <summary>
    /// Clears all quests from the quest list.
    /// </summary>
    public void ClearQuests()
    {
        quests.Clear();
    }
}
