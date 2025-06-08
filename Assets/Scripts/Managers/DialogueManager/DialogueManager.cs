using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

public class DialogueManager : BaseManager<DialogueManager>
{
    [Header("References")]
    [SerializeField] private List<TextAsset> defaultDialogues = new List<TextAsset>();

    private Story story;

    public event Action<Story, BaseNPC> OnDialogueStarted;
    public event Action<Story, BaseNPC> OnDialogueContinue;
    public event Action<Story, BaseNPC> OnDialogueEnded;

    public BaseNPC CurrentNPC { get; private set; }
    public bool canTalk = true;

    public void StartDialogue(TextAsset ink, BaseNPC npc)
    {
        if (!canTalk) return;

        CurrentNPC = npc;

        story = new Story(ink.text);

        SyncNPCTrustLevel();
        RegisterVaribleObservers();

        OnDialogueStarted?.Invoke(story, npc);
        ContinueDialogue();
    }

    public void StartDialogue(TextAsset ink)
    {
        if (!canTalk) return;

        CurrentNPC = null;

        story = new Story(ink.text);

        OnDialogueStarted?.Invoke(story, null);
        ContinueDialogue();
    }

    public void ContinueDialogue()
    {
        if (story != null && !story.canContinue && story.currentChoices.Count == 0)
        {
            EndDialogue();
            return;
        }

        story.Continue();

        OnDialogueContinue?.Invoke(story, CurrentNPC);
    }

    public void EndDialogue()
    {
        UnregisterVariableObservers();
        OnDialogueEnded?.Invoke(story, CurrentNPC);
        story = null;
        CurrentNPC = null;
    }

    public List<Choice> GetChoices()
    {
        return story.currentChoices;
    }

    public void MakeChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= story.currentChoices.Count)
        {
            Debug.LogError($"Invalid choice index: {choiceIndex}");
            return;
        }

        story.ChooseChoiceIndex(choiceIndex);
        ContinueDialogue();
    }

    public TextAsset GetDefault(string fileName)
    {
        foreach (TextAsset textAsset in defaultDialogues)
        {
            if (textAsset.name == fileName) return textAsset;
        }

        return defaultDialogues[0];
    }

    #region NPC Trust Sync
    private void SyncNPCTrustLevel()
    {
        if (CurrentNPC == null) return;

        if(story.variablesState.Contains("trustLvl"))
        {
            story.variablesState["trustLvl"] = CurrentNPC.TrustLevel;
        }

        if (story.variablesState.Contains("trustGrade"))
        {
            story.variablesState["trustGrade"] = NPCManager.Instance.GetTrustGrade();
        }
    }

    private void RegisterVaribleObservers()
    {
        if (story == null) return;

        if (story.variablesState.Contains("trustLvl"))
        {
            story.ObserveVariable("trustLvl", (string varName, object newValue) =>
            {
                float newTrustValue = (float)newValue;
                NPCManager.Instance.UpdateTrustLevel(CurrentNPC.npcName, newTrustValue);
                CurrentNPC.TrustLevel = newTrustValue;
            });
        }
    }

    private void UnregisterVariableObservers()
    {
        if (story == null) return;
        story.RemoveVariableObserver(null);
    }

    public void SetCustomVariable(string varName, object value)
    {
        if (story == null) return;

        if(story.variablesState.Contains(varName))
        {
            story.variablesState[varName] = value;
        }
    }
    #endregion
}
