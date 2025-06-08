using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void RemoveQuest(Quest quest)
    {
        quests.Remove(quest);
        OnQuestRemoved?.Invoke(quest);
    }

    public Quest GetQuest(QuestData data)
    {
        Quest quest = quests.Find(item => item.data == data);

        return quest;
    }

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

    public List<Quest> GetActiveQuest(QuestType type = QuestType.None)
    {
        if(type == QuestType.None)
        {
            return quests;
        } else
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

    public List<Quest> GetFinishedQuest(QuestType type = QuestType.None)
    {
        List<Quest> activeQuests = new List<Quest>();

        foreach (Quest quest in quests)
        {
            if (quest.finished)
            {
                switch (type)
                {
                    case QuestType.Main: activeQuests.Add(quest); break;
                    case QuestType.Delivery: activeQuests.Add(quest); break;
                    case QuestType.Favor: activeQuests.Add(quest); break;
                    default: activeQuests.Add((Quest)quest); break;
                }

            }
        }

        return activeQuests;
    }

    public List<Quest> GetUnfinishedQuest(QuestType type = QuestType.None)
    {
        List<Quest> activeQuests = new List<Quest>();

        foreach (Quest quest in quests)
        {
            if (!quest.finished)
            {
                switch (type)
                {
                    case QuestType.Main: activeQuests.Add(quest); break;
                    case QuestType.Delivery: activeQuests.Add(quest); break;
                    case QuestType.Favor: activeQuests.Add(quest); break;
                    default: activeQuests.Add((Quest)quest); break;
                }

            }
        }

        return activeQuests;
    }

    public void ClearQuests()
    {
        quests.Clear();
    }
}
