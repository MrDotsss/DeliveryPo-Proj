using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class QuestItem
{
    public string uniqueID;
    public Quest data;
    public bool IsFinished = false;

    public QuestItem(Quest quest)
    {
        uniqueID = Guid.NewGuid().ToString();
        data = quest;
    }
}


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [SerializeField] private List<QuestItem> activeQuests = new List<QuestItem>();

    public event Action<QuestItem> OnRegisterQuest;
    public event Action<QuestItem> OnUpdateQuest;
    public event Action<QuestItem> OnFinishedQuest;



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(Instance);
    }

    public QuestItem RegisterQuest(Quest quest)
    {
        QuestItem questItem = new QuestItem(quest);

        if (!activeQuests.Contains(questItem))
        {
            activeQuests.Add(questItem);
            OnRegisterQuest?.Invoke(questItem);

            return questItem;
        }
        else
        {
            Debug.LogError($"{quest.questTitle} already exist.");
            return null;
        }
    }

    public void UpdateQuest(QuestItem quest)
    {
        if (activeQuests.Contains(quest))
        {
            OnUpdateQuest?.Invoke(quest);
        }
        else
        {
            Debug.LogError($"{quest.data.questTitle} has not started yet.");
        }
    }

    public void FinishQuest(QuestItem quest)
    {
        if (activeQuests.Contains(quest))
        {
            quest.IsFinished = true;
            OnFinishedQuest?.Invoke(quest);
        }
        else
        {
            Debug.LogError($"{quest.data.questTitle} has not started yet.");
        }
    }

    private bool IsMainFinished()
    {
        int finishedCount = 0;

        foreach (QuestItem quest in activeQuests)
        {
            if (quest.data.questType == QuestType.Main || quest.data.questType == QuestType.Delivery)
            {
                if (quest.IsFinished) finishedCount++;
            }
        }

        if (finishedCount >= activeQuests.Count)
        {
            return true;
        }

        return false;
    }

    public void ClearQuests()
    {
        activeQuests.Clear();
    }

    public List<QuestItem> GetActiveQuests()
    {
        return activeQuests;
    }
}

