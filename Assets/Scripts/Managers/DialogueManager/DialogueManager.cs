using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    [Header("References")]
    [SerializeField] private List<TextAsset> defaultDialogues = new List<TextAsset>();


    private Story story;

    public event Action<Story> OnDialogueStarted;
    public event Action<Story> OnDialogueContinue;
    public event Action<Story> OnDialogueEnded;

    public void StartDialogue(TextAsset ink, Transform targetAt = null)
    {
        story = new Story(ink.text);

        OnDialogueStarted?.Invoke(story);
        ContinueDialogue();
    }

    public string ContinueDialogue()
    {
        if(!story.canContinue && story.currentChoices.Count == 0)
        {
            EndDialogue();
            return null;
        }

        OnDialogueContinue?.Invoke(story);

        return story.Continue();
    }

    public List<Choice> GetChoices()
    {
        if (story.currentChoices.Count == 0) return null;

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

    public void EndDialogue()
    {
        OnDialogueEnded?.Invoke(story);
        story = null;
    }

    public TextAsset GetDefault(string fileName)
    {
        foreach (TextAsset textAsset in defaultDialogues)
        {
            if (textAsset.name == fileName) return textAsset;
        }

        return defaultDialogues[0];
    }
}
