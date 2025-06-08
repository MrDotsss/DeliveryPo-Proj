using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the Delivery state of the Phone UI,
/// displaying the active delivery quests in a list.
/// </summary>
public class PhoneDelivery : PhoneUIState
{
    public override PhoneUIStateMachine.PhoneStates StateType => PhoneUIStateMachine.PhoneStates.Delivery;

    public GameObject itemUIPrefab;  // Prefab used for each quest item UI element
    public GameObject content;       // Parent object to hold quest item UI elements

    /// <summary>
    /// Called when entering the Delivery state.
    /// Activates the UI and builds the list of active delivery quests.
    /// </summary>
    public override void EnterState(Dictionary<string, object> data = null)
    {
        gameObject.SetActive(true);
        BuildUIList();
    }

    /// <summary>
    /// Called when exiting the Delivery state.
    /// Deactivates the Delivery UI.
    /// </summary>
    public override void ExitState()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called every frame while in the Delivery state.
    /// No update logic needed currently.
    /// </summary>
    public override void UpdateState()
    {
    }

    /// <summary>
    /// Builds the UI list by instantiating quest item prefabs for all active delivery quests.
    /// Clears any existing UI list before building.
    /// </summary>
    private void BuildUIList()
    {
        List<Quest> items = QuestManager.Instance.GetActiveQuest(QuestType.Delivery);

        ClearUIList();

        for (int i = 0; i < items.Count; i++)
        {
            QuestItemUI itemUI = Instantiate(itemUIPrefab, content.transform).GetComponent<QuestItemUI>();
            itemUI.quest = items[i];
            itemUI.Initialize();
        }
    }

    /// <summary>
    /// Clears all children UI elements under the content parent.
    /// </summary>
    private void ClearUIList()
    {
        if (content.transform.childCount <= 0) return;

        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }
    }
}
