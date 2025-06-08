using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using Ink.Runtime;
using System.Collections;

/// <summary>
/// Manages the dialogue UI panel for conversations between the player and NPCs.
/// Handles displaying dialogue text, speaker names, character portraits,
/// presenting player choices, and input control during dialogue.
/// </summary>
public class DialogueUI : UIState
{
    // Specifies this UI panel as the Dialogue panel
    public override UIManager.EUIPanels State => UIManager.EUIPanels.Dialogue;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI speakerText; // Text UI for showing speaker's name
    [SerializeField] private TextMeshProUGUI dialogueText; // Text UI for showing dialogue lines

    [Space]
    [SerializeField] private RectTransform playerRect; // UI rect for player image position
    [SerializeField] private RectTransform npcRect;    // UI rect for NPC image position
    [SerializeField] private Image playerImage;        // Player character portrait image
    [SerializeField] private Image npcImage;           // NPC character portrait image

    [Space]
    [SerializeField] private RectTransform choicesPanel; // Parent panel for choice buttons
    [SerializeField] private GameObject choiceItemPrefab; // Prefab for individual choice UI buttons
    private LayoutElement choicePanelLayout;             // Layout element to control panel width dynamically

    [Header("Properties")]
    public Color playerColor = Color.green; // Color used for player speaker text
    public Color npcColor = Color.cyan;     // Color used for NPC speaker text

    private BaseNPC currentNPC;  // Currently talking NPC reference
    private Player player;       // Reference to the player character

    private bool canInput = true;   // Whether input to continue dialogue is accepted
    private bool choosing = false;  // Whether player is currently choosing a dialogue option

    /// <summary>
    /// Called when entering the dialogue UI state.
    /// Enables UI, pauses the game, subscribes to dialogue events,
    /// collapses choices panel, and caches choice panel layout component.
    /// </summary>
    public override void Enter(object data = null)
    {
        gameObject.SetActive(true);

        GameManager.Instance.RequestPause();

        if (player == null) player = GameManager.Instance.GetPlayer();

        DialogueManager.Instance.OnDialogueContinue += DisplayNextDialogue;
        DialogueManager.Instance.OnDialogueEnded += DialogueEnded;

        CollapseChoices();

        if (choicePanelLayout == null) choicePanelLayout = choicesPanel.GetComponent<LayoutElement>();
    }

    /// <summary>
    /// Called when exiting the dialogue UI state.
    /// Disables UI, unpauses the game, unsubscribes from dialogue events,
    /// resets dialogue state and hides character images.
    /// </summary>
    public override void Exit()
    {
        gameObject.SetActive(false);

        GameManager.Instance.RequestUnpause();

        DialogueManager.Instance.OnDialogueContinue -= DisplayNextDialogue;
        DialogueManager.Instance.OnDialogueEnded -= DialogueEnded;

        choosing = true;
        currentNPC = null;
        npcImage.enabled = false;
        playerImage.enabled = false;
    }

    /// <summary>
    /// Called when dialogue ends.
    /// Switches UI back to the cursor panel.
    /// </summary>
    private void DialogueEnded(Story story, BaseNPC npc)
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.CursorPanel);
    }

    /// <summary>
    /// Updates the dialogue UI each frame.
    /// Advances dialogue on mouse click if allowed,
    /// and dynamically adjusts the choice panel width.
    /// </summary>
    public override void UpdateState()
    {
        if (canInput && !choosing && InputManager.Instance.lmbAction.WasPressedThisFrame())
            DialogueManager.Instance.ContinueDialogue();

        SetChoiceWidth(choosing ? 800f : 400f, choosing ? 500f : 1000f);
    }

    /// <summary>
    /// Displays the next dialogue line, updates speaker text, and character portraits.
    /// Also shows player choices if available.
    /// </summary>
    private void DisplayNextDialogue(Story story, BaseNPC npc)
    {
        if (npc != null) currentNPC = npc;

        dialogueText.text = GetDialogueLine(story.currentText);
        speakerText.text = GetSpeakerName(story.currentText);

        if (currentNPC != null && currentNPC.spriteImages.Count > 0)
        {
            npcImage.enabled = true;
            playerImage.enabled = true;
            npcImage.sprite = currentNPC.spriteImages[UnityEngine.Random.Range(0, currentNPC.spriteImages.Count)];
        }

        if (player != null && player.imageSprites.Count > 0)
        {
            playerImage.sprite = player.imageSprites[UnityEngine.Random.Range(0, player.imageSprites.Count)];
        }

        if (story.currentChoices.Count != 0)
        {
            DisplayChoices();
        }
    }

    /// <summary>
    /// Instantiates choice UI buttons for each available dialogue choice
    /// and sets up their click listeners to handle player selection.
    /// Disables input to continue dialogue while choosing.
    /// </summary>
    private void DisplayChoices()
    {
        List<Choice> choices = DialogueManager.Instance.GetChoices();

        for (int i = 0; i < choices.Count; i++)
        {
            ChoiceItem item = Instantiate(choiceItemPrefab, choicesPanel).GetComponent<ChoiceItem>();
            item.buttonText.text = choices[i].text;

            int choiceIndex = i;
            item.button.onClick.AddListener(() => MakeChoice(choiceIndex));
        }

        canInput = false;
        choosing = true;
    }

    /// <summary>
    /// Smoothly adjusts the minimum width of the choice panel to the desired width using lerp.
    /// </summary>
    private void SetChoiceWidth(float desiredWidth, float lerp)
    {
        float newWidth = Mathf.MoveTowards(choicePanelLayout.minWidth, desiredWidth, lerp * Time.unscaledDeltaTime);

        choicePanelLayout.minWidth = newWidth;
    }

    /// <summary>
    /// Clears all choice UI elements and resets input and choosing flags.
    /// </summary>
    private void CollapseChoices()
    {
        ClearChoices();
        choosing = false;
        canInput = true;
    }

    /// <summary>
    /// Destroys all child UI elements under the choices panel.
    /// </summary>
    private void ClearChoices()
    {
        for (int i = 0; i < choicesPanel.childCount; i++)
        {
            Destroy(choicesPanel.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Called when a choice is made by the player.
    /// Passes the selected choice index to the DialogueManager and collapses choices UI.
    /// </summary>
    private void MakeChoice(int choiceIndex)
    {
        DialogueManager.Instance.MakeChoice(choiceIndex);
        CollapseChoices();
    }

    /// <summary>
    /// Cleans up speaker tags ("you:", "alias:", "npc:") from dialogue text lines before display.
    /// </summary>
    private string GetDialogueLine(string line)
    {
        string result = line.Replace("you: ", "")
            .Replace("alias: ", "")
            .Replace("npc: ", "");

        return result;
    }

    /// <summary>
    /// Determines the speaker name and text alignment/color based on dialogue line prefix.
    /// Returns formatted speaker text with color tags.
    /// </summary>
    private string GetSpeakerName(string line)
    {
        string playerHex = playerColor.ToHexString();
        string npcHex = npcColor.ToHexString();

        if (line.StartsWith("you:", StringComparison.OrdinalIgnoreCase))
        {
            speakerText.alignment = TextAlignmentOptions.Left;
            return $"<color=#{playerHex}>Marimar:</color> ";
        }

        if (currentNPC != null && line.StartsWith("alias:", StringComparison.OrdinalIgnoreCase))
        {
            speakerText.alignment = TextAlignmentOptions.Right;
            return $"<color=#{npcHex}>{currentNPC.aliasName}:</color> ";
        }
        else if (currentNPC != null && line.StartsWith("npc:", StringComparison.OrdinalIgnoreCase))
        {
            speakerText.alignment = TextAlignmentOptions.Right;
            return $"<color=#{npcHex}>{currentNPC.npcName}:</color> ";
        }
        else
            return "";
    }
}
