using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingComponent : MonoBehaviour, INPComponent
{
    [Header("Reference")]
    [SerializeField] private TextAsset inkJSON;

    public BaseNPC Owner {  get; private set; }

    private void Start()
    {
        Owner = GetComponent<BaseNPC>();
    }

    public void Activate()
    {
        if (inkJSON != null)
        {
            DialogueManager.Instance.StartDialogue(inkJSON, Owner);
        }

    }
}
