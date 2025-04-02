using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJSON;
    private Story story;

    [Header("Player")]
    private BaseNPC currentNPC;
    private Player player;

    [Header("Defaults")]
    [SerializeField] private List<TextAsset> defaultDialogues = new List<TextAsset>();
   
    public event Action<Story, BaseNPC> OnDialogueStarted;
    public event Action<Story, BaseNPC> OnDialogueContinue;
    public event Action<Story, BaseNPC> OnDialogueEnded;

    public bool IsDialogueActive { get; private set; } = false;

    private List<Button> choiceButtons = new List<Button>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsDialogueActive) return;

        // Check if the player pressed the "Submit" button in the new Input System
        if (InputManager.Instance.input.actions["interact"].WasPressedThisFrame() ||
            InputManager.Instance.input.actions["left-action"].WasPressedThisFrame())
        {
            if (story.currentChoices.Count > 0) return; // Prevent skipping choices
            ContinueStory();
        }
    }

    /// <summary>
    /// Starts the dialogue with a given Ink JSON file.
    /// </summary>
    public void StartDialogue(TextAsset inkStory, BaseNPC targetNPC = null, Transform targetAt = null)
    {
        if (IsDialogueActive) return;

        story = new Story(inkStory.text);

        currentNPC = targetNPC;

        // Sync trust level if the NPC exists
        if (currentNPC != null)
        {
            SyncGameTrustWithInk();
        }

        RegisterVariableObservers();

        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.phone.canInput = false;

        if (currentNPC != null)
        {
            player.cam.FocusAt(currentNPC.lookAt);
        }
        else
        {
            player.cam.FocusAt(targetAt);
        }

        InputManager.Instance.SetCursor(false, false);
        PlayerInventory.Instance.CloseInventory();

        OnDialogueStarted?.Invoke(story, currentNPC);

        StartCoroutine(DelayedContinueStory());
    }


    private IEnumerator DelayedContinueStory()
    {
        yield return null; // Wait for a frame to ensure the story is set up
        if (story != null && story.canContinue)
        {
            ContinueStory();
        }
    }


    /// <summary>
    /// Continues the Ink story and updates the dialogue text.
    /// </summary>
    public void ContinueStory()
    {
        if (!story.canContinue && story.currentChoices.Count == 0)
        {
            EndDialogue();
            return;
        }

        dialogueText.text = FormatLine(story.Continue());
        OnDialogueContinue?.Invoke(story, currentNPC);
        DisplayChoices();
    }

    private string FormatLine(string line)
    {
        string playerHex = GameSetting.Instance.playerColor.ToHexString();
        string npcHex = GameSetting.Instance.npcColor.ToHexString();

        return line.Replace("alias: ", $"<color=#{npcHex}>{currentNPC.aliasName}:</color> ")
                .Replace("npc: ", $"<color=#{npcHex}>{currentNPC.npcName}:</color> ") // Gold color for NPC
                .Replace("you: ", $"<color=#{playerHex}>Marimar:</color> "); // Gold color for Player
    }

    /// <summary>
    /// Displays dialogue choices as buttons.
    /// </summary>
    private void DisplayChoices()
    {
        ClearChoices();

        if (story.currentChoices.Count == 0)
        {
            return;
        }


        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);

            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = FormatLine(story.currentChoices[i].text);

            int choiceIndex = i;
            choiceButton.onClick.AddListener(() => MakeChoice(choiceIndex));
            choiceButtons.Add(choiceButton);
        }
    }

    /// <summary>
    /// Handles player choice selection.
    /// </summary>
    private void MakeChoice(int choiceIndex)
    {
        if (choiceIndex < 0 || choiceIndex >= story.currentChoices.Count)
        {
            Debug.LogError($"Invalid choice index: {choiceIndex}");
            return;
        }

        story.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    /// <summary>
    /// Clears previous choices from the UI.
    /// </summary>
    private void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            Destroy(button.gameObject);
        }
        choiceButtons.Clear();
    }

    /// <summary>
    /// Ends the dialogue session.
    /// </summary>
    public void EndDialogue()
    {
        IsDialogueActive = false;
        dialoguePanel.SetActive(false);
        UnregisterVariableObservers();
        player.cam.StopFocus();
        player.phone.canInput = true;
        InputManager.Instance.SetCursor(true, true);
        OnDialogueEnded?.Invoke(story, currentNPC);
        currentNPC = null;

        Debug.Log($"Dialogue ended with grade: {NPCManager.Instance.GetTrustGrade()}");
    }
    /// <summary>
    /// Syncs the trust level between Unity and Ink safely.
    /// </summary>
    private void SyncGameTrustWithInk()
    {
        if (currentNPC == null) return;

        string trustVariable = "trustLvl"; // Ink variable name

        if (story.variablesState.Contains(trustVariable))
        {
            // Set the Ink trust level to the NPC's trust level
            story.variablesState[trustVariable] = currentNPC.TrustLevel;
        }
    }


    /// <summary>
    /// Registers observers for Ink variables safely.
    /// </summary>
    private void RegisterVariableObservers()
    {
        if (story == null) return;

        // Check if the Ink file contains the "trustLvl" variable before observing
        if (story.variablesState.Contains("trustLvl"))
        {
            story.ObserveVariable("trustLvl", (string varName, object newValue) =>
            {
                float newTrustValue = (float)newValue;
                NPCManager.Instance.UpdateTrustLevel(currentNPC.npcName, newTrustValue);
            });
        }
    }

    public void SetCustomVariable(string varName, object value)
    {
        if (story.variablesState.Contains(varName))
        {
            story.variablesState[varName] = value;
        }
    }

    /// <summary>
    /// Unregisters variable observers when dialogue ends.
    /// </summary>
    private void UnregisterVariableObservers()
    {
        if (story == null) return;
        story.RemoveVariableObserver(null);
    }

    public TextAsset GetDefaultDialogue(string fileName)
    {
        foreach (TextAsset textAsset in defaultDialogues)
        {
            if (textAsset.name == fileName) return textAsset;
        }

        return defaultDialogues[0];
    }
}
