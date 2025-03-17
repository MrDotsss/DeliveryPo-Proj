using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingNPC : MonoBehaviour, INPComponent
{
    [Header("Reference")]
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private Transform targetFocus;

    public virtual void Activate()
    {
        if (inkJSON != null)
        {
            DialogueManager.Instance.StartDialogue(inkJSON, targetFocus);
        }

    }
}
