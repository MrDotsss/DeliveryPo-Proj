using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the player enters the exit zone and triggers dialogue based on delivery quest completion.
/// </summary>
public class ExitZoneChek : MonoBehaviour
{
    public TextAsset finishedDialogue;   // Dialogue to play if all delivery quests are finished
    public TextAsset unfinishedDialogue; // Dialogue to play if delivery quests are still unfinished

    private int unfinishedCount = 0;

    /// <summary>
    /// Triggered when another collider enters this trigger zone.
    /// Checks the number of unfinished delivery quests and starts appropriate dialogue.
    /// </summary>
    /// <param name="other">Collider entering the zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Get count of unfinished delivery quests
        unfinishedCount = QuestManager.Instance.GetUnfinishedQuest(QuestType.Delivery).Count;

        // If no unfinished delivery quests, play finished dialogue and disable this zone
        if (unfinishedCount == 0)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(finishedDialogue);
            gameObject.SetActive(false);
        }
        // Otherwise, play unfinished dialogue
        else
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(unfinishedDialogue);
        }
    }
}
