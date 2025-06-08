using System;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;

/// <summary>
/// Manages Ink dialogue stories, NPC interactions, and dialogue flow.
/// Handles starting, continuing, ending dialogues, and syncing NPC trust variables.
/// </summary>
public class DialogueManager : BaseManager<DialogueManager>
{
    [Header("References")]
    [SerializeField] private List<TextAsset> defaultDialogues = new List<TextAsset>();

    private Story story;

    /// <summary>
    /// Event invoked when a dialogue starts.
    /// </summary>
    public event Action<Story, BaseNPC> OnDialogueStarted;

    /// <summary>
    /// Event invoked when a dialogue continues.
    /// </summary>
    public event Action<Story, BaseNPC> OnDialogueContinue;

    /// <summary>
    /// Event invoked when a dialogue ends.
    /// </summary>
    public event Action<Story, BaseNPC> OnDialogueEnded;

    /// <summary>
    /// The NPC currently involved in the dialogue, or null for non-NPC dialogues.
    /// </summary>
    public BaseNPC CurrentNPC { get; private set; }

    /// <summary>
    /// Indicates whether dialogue can be started or continued.
    /// </summary>
    public bool canTalk = true;

    /// <summary>
    /// Starts a dialogue with a specific NPC using the given Ink story text asset.
    /// </summary>
    /// <param name="ink">The Ink story text asset.</param>
    /// <param name="npc">The NPC involved in the dialogue.</param>
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

    /// <summary>
    /// Starts a dialogue without an NPC using the given Ink story text asset.
    /// </summary>
    /// <param name="ink">The Ink story text asset.</param>
    public void StartDialogue(TextAsset ink)
    {
        if (!canTalk) return;

        CurrentNPC = null;

        story = new Story(ink.text);

        OnDialogueStarted?.Invoke(story, null);
        ContinueDialogue();
    }

    /// <summary>
    /// Continues the current dialogue if possible, or ends it if no more content exists.
    /// </summary>
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

    /// <summary>
    /// Ends the current dialogue, unregistering variable observers and clearing references.
    /// </summary>
    public void EndDialogue()
    {
        UnregisterVariableObservers();
        OnDialogueEnded?.Invoke(story, CurrentNPC);
        story = null;
        CurrentNPC = null;
    }

    /// <summary>
    /// Returns the current list of choices available in the dialogue.
    /// </summary>
    /// <returns>List of Ink story choices.</returns>
    public List<Choice> GetChoices()
    {
        return story.currentChoices;
    }

    /// <summary>
    /// Makes a choice in the dialogue by index and continues the story.
    /// </summary>
    /// <param name="choiceIndex">Index of the choice to select.</param>
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

    /// <summary>
    /// Retrieves a default dialogue text asset by name.
    /// Returns the first default dialogue if no match is found.
    /// </summary>
    /// <param name="fileName">Name of the dialogue file.</param>
    /// <returns>TextAsset for the dialogue.</returns>
    public TextAsset GetDefault(string fileName)
    {
        foreach (TextAsset textAsset in defaultDialogues)
        {
            if (textAsset.name == fileName) return textAsset;
        }

        return defaultDialogues[0];
    }

    #region NPC Trust Sync

    /// <summary>
    /// Syncs the NPC’s trust level variables into the Ink story state.
    /// </summary>
    private void SyncNPCTrustLevel()
    {
        if (CurrentNPC == null) return;

        if (story.variablesState.Contains("trustLvl"))
        {
            story.variablesState["trustLvl"] = CurrentNPC.TrustLevel;
        }

        if (story.variablesState.Contains("trustGrade"))
        {
            story.variablesState["trustGrade"] = NPCManager.Instance.GetTrustGrade();
        }
    }

    /// <summary>
    /// Registers observers for Ink variables that affect NPC trust.
    /// Updates NPC trust levels when variables change in the story.
    /// </summary>
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

    /// <summary>
    /// Removes all registered variable observers from the Ink story.
    /// </summary>
    private void UnregisterVariableObservers()
    {
        if (story == null) return;
        story.RemoveVariableObserver(null);
    }

    /// <summary>
    /// Sets a custom variable in the Ink story variables state if it exists.
    /// </summary>
    /// <param name="varName">Variable name.</param>
    /// <param name="value">Value to set.</param>
    public void SetCustomVariable(string varName, object value)
    {
        if (story == null) return;

        if (story.variablesState.Contains(varName))
        {
            story.variablesState[varName] = value;
        }
    }

    #endregion
}
