using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles displaying a single quest item in the UI,
/// showing quest title, description, and the checklist of required items.
/// Allows toggling the visibility of the checklist items.
/// </summary>
public class QuestItemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rect;           // RectTransform for resizing the UI element
    [SerializeField] private Button button;                 // Button to toggle item list visibility
    [SerializeField] private Image rectImage;                // Background image to indicate quest status
    [SerializeField] private TextMeshProUGUI titleText;      // Text for the quest title
    [SerializeField] private TextMeshProUGUI descriptionText;// Text for the quest description

    [Header("Properties")]
    public Quest quest;                                      // The quest this UI represents

    private List<GameObject> itemsPool = new List<GameObject>(); // Pool of instantiated item texts for checklist

    /// <summary>
    /// Initializes the UI with quest data and sets up the button listener.
    /// Also updates the visual state based on quest completion.
    /// </summary>
    public void Initialize()
    {
        titleText.text = quest.data.questTitle;
        descriptionText.text = quest.data.questDescription;

        button.onClick.AddListener(ToggleItemList);

        CheckQuest();
    }

    /// <summary>
    /// Clean up button listeners when this UI element is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Toggles the checklist items visibility.
    /// If the items are not yet spawned, spawns them.
    /// Also resizes the UI rect to fit the checklist items.
    /// </summary>
    public void ToggleItemList()
    {
        if (itemsPool.Count == quest.data.items.Count)
        {
            // Toggle active state of existing item UI elements
            foreach (GameObject item in itemsPool)
            {
                item.SetActive(!item.activeSelf);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x,
                    item.activeSelf ? rect.sizeDelta.y + 26 : rect.sizeDelta.y - 26);
            }
        }
        else
        {
            // Spawn item UI elements for each quest item
            foreach (ItemData item in quest.data.items)
            {
                SpawnText(item);
            }
        }
    }

    /// <summary>
    /// Spawns a TextMeshProUGUI element to represent a single checklist item,
    /// adds it to the pool and adjusts the parent rect size.
    /// </summary>
    /// <param name="data">Item data for the checklist entry.</param>
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

    /// <summary>
    /// Updates the UI colors and button interactability
    /// based on whether the quest is finished or not.
    /// </summary>
    private void CheckQuest()
    {
        titleText.color = quest.finished ? Color.black : Color.white;
        descriptionText.color = quest.finished ? Color.black : Color.white;
        rectImage.color = quest.finished ? Color.green : Color.red;
        button.enabled = !quest.finished;
    }
}
