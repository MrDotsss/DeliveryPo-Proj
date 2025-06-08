using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class QuestItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private Button button;
    [SerializeField] private Image rectImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [Header("Properties")]
    public Quest quest;

    private List<GameObject> itemsPool = new List<GameObject>();

    public void Initialize()
    {
        titleText.text = quest.data.questTitle;
        descriptionText.text = quest.data.questDescription;

        button.onClick.AddListener(ToggleItemList);

        CheckQuest();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void ToggleItemList()
    {
        if (itemsPool.Count == quest.data.items.Count)
        {
            foreach (GameObject item in itemsPool)
            {
                item.SetActive(!item.activeSelf);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x,
                    item.activeSelf ? rect.sizeDelta.y + 26 : rect.sizeDelta.y - 26);
            }
        }
        else
        {
            foreach (ItemData item in quest.data.items)
            {
                SpawnText(item);
            }
        }
    }

    private void SpawnText(ItemData data)
    {
        GameObject textMesh = new GameObject(data.itemName);
        textMesh.transform.SetParent(gameObject.transform, false);

        TextMeshProUGUI tmp = textMesh.AddComponent<TextMeshProUGUI>();

        tmp.text = data.itemName;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        RectTransform rectTransform = tmp.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 26);
        itemsPool.Add(textMesh);
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + 26);
    }

    private void CheckQuest()
    {

        titleText.color = quest.finished ? Color.black : Color.white;
        descriptionText.color = quest.finished ? Color.black : Color.white;
        rectImage.color = quest.finished ? Color.green : Color.red;
        button.enabled = !quest.finished;

    }
}
