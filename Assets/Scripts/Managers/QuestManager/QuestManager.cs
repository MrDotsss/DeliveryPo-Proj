using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Quest
{
    public string id;
    public QuestData data;
    public bool finished = false;

    public Quest(QuestData data)
    {
        id = Guid.NewGuid().ToString();
        this.data = data;
    }
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    [SerializeField] private List<Quest> quests = new List<Quest>();

    public event Action<Quest> OnQuestAdded;
    public event Action<Quest> OnQuestRemoved;
    public event Action<Quest, Dictionary<string, object>> OnQuestUpdated;
    public event Action<Quest> OnQuestFinished;

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest))
        {
            quests.Add(quest);
            OnQuestAdded?.Invoke(quest);
        }
        else Debug.LogError($"{quest.data.questTitle} duplicated with id: {quest.id}");
    }

    public void AddQuest(QuestData data)
    {
        Quest quest = new Quest(data);

        quests.Add(quest);
        OnQuestAdded?.Invoke(quest);
    }

    public void RemoveQuest(Quest quest)
    {
        quests.Remove(quest);
        OnQuestRemoved?.Invoke(quest);
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
        OnQuestFinished?.Invoke(quest);
    }

    public List<Quest> GetActiveQuest(QuestType type = QuestType.None)
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
}
