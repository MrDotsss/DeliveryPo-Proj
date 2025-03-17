using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using System.Collections.Generic;
using System;

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
    [SerializeField] private PlayerCam playerCam;

    public event Action<Story> OnDialogueStarted;
    public event Action<Story> OnDialogueEnded;

    public bool IsDialogueActive { get; private set; } = false;

    private List<Button> choiceButtons = new List<Button>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playerCam = GameObject.FindGameObjectWithTag("PlayerCam").GetComponent<PlayerCam>();

        dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!IsDialogueActive) return;

        // Check if the player pressed the "Submit" button in the new Input System
        if (InputManager.Instance.input.actions["interact"].WasPressedThisFrame())
        {
            if (story.currentChoices.Count > 0) return; // Prevent skipping choices
            ContinueStory();
        }
    }

    /// <summary>
    /// Starts the dialogue with a given Ink JSON file.
    /// </summary>
    public void StartDialogue(TextAsset inkStory, Transform target = null)
    {
        if (IsDialogueActive) return;

        IsDialogueActive = true;
        dialoguePanel.SetActive(true);

        if (inkStory != null)
        {
            story = new Story(inkStory.text);
            OnDialogueStarted?.Invoke(story);
            SyncGameTrustWithInk(); // Sync trust level
            RegisterVariableObservers();
        }

        playerCam.FocusAt(target);
        InputManager.Instance.UIMode(true);
        ContinueStory();
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

        dialogueText.text = story.Continue();
        DisplayChoices();
    }

    /// <summary>
    /// Displays dialogue choices as buttons.
    /// </summary>
    private void DisplayChoices()
    {
        ClearChoices();

        if (story.currentChoices.Count == 0)
        {
            InputManager.Instance.UIMode(true);
            return;
        }

        InputManager.Instance.UIMode(false);

        for (int i = 0; i < story.currentChoices.Count; i++)
        {
            Button choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
            choiceButton.GetComponentInChildren<TextMeshProUGUI>().text = story.currentChoices[i].text;

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
        OnDialogueEnded?.Invoke(story);
        UnregisterVariableObservers();
        playerCam.StopFocus();
        InputManager.Instance.PlayerMode();
    }

    /// <summary>
    /// Syncs the trust level between Unity and Ink.
    /// </summary>
    private void SyncGameTrustWithInk()
    {
        if (story == null) return;

        int unityTrust = GameStatusManager.Instance.PlayerTrust;
        story.variablesState["trustLevel"] = unityTrust;

        Debug.Log($"[Sync] Unity -> Ink | Trust Level: {unityTrust}");
    }

    /// <summary>
    /// Registers observers for Ink variables.
    /// </summary>
    private void RegisterVariableObservers()
    {
        if (story == null) return;

        story.ObserveVariable("trustLevel", (string varName, object newValue) =>
        {
            int newTrustValue = (int)newValue;
            if (newTrustValue != GameStatusManager.Instance.PlayerTrust)
            {
                Debug.Log($"[Ink Variable Changed] {varName} -> {newValue}");
                GameStatusManager.Instance.DefineTrust(newTrustValue);
            }
        });
    }

    /// <summary>
    /// Unregisters variable observers when dialogue ends.
    /// </summary>
    private void UnregisterVariableObservers()
    {
        if (story == null) return;
        story.RemoveVariableObserver(null);
    }
}
