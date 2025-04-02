using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestText : MonoBehaviour
{
    public QuestItem questData;
    [Space]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Space]
    [SerializeField] private Image checkBox;

    [SerializeField] private Sprite uncheckedSprite;
    [SerializeField] private Sprite checkedSprite;

    private void Start()
    {
        checkBox.sprite = questData.IsFinished ? checkedSprite : uncheckedSprite;

        InitializeQuestText();

        QuestManager.Instance.OnFinishedQuest += MarkedAsChecked;
    }

    private void OnDestroy()
    {
        QuestManager.Instance.OnFinishedQuest -= MarkedAsChecked;
    }

    public void InitializeQuestText()
    {
        titleText.text = questData.data.questTitle;
        descriptionText.text = questData.data.questDescription;
    }

    private void MarkedAsChecked(QuestItem quest)
    {
        if (quest == questData)
        {
            checkBox.sprite = checkedSprite;
        }
    }
}
