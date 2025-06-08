using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitZoneChek : MonoBehaviour
{
    public TextAsset finishedDialogue;
    public TextAsset unfinishedDialogue;

    private int unfinishedCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        unfinishedCount = QuestManager.Instance.GetUnfinishedQuest(QuestType.Delivery).Count;


        if (unfinishedCount == 0)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(finishedDialogue);
            gameObject.SetActive(false);
        }
        else
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Dialogue);
            DialogueManager.Instance.StartDialogue(unfinishedDialogue);
        }
    }
}
