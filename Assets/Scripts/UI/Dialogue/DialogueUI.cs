using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;
using Ink.Runtime;
using System.Collections;

public class DialogueUI : UIState
{
    public override UIManager.EUIPanels State => UIManager.EUIPanels.Dialogue;
    [Header("References")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [Space]
    [SerializeField] private RectTransform playerRect;
    [SerializeField] private RectTransform npcRect;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image npcImage;
    [Space]
    [SerializeField] private RectTransform choicesPanel;
    [SerializeField] private GameObject choiceItemPrefab;
    private LayoutElement choicePanelLayout;

    [Header("Properties")]
    public Color playerColor = Color.green;
    public Color npcColor = Color.cyan;

    private BaseNPC currentNPC;
    private Player player;

    private bool canInput = true;
    private bool choosing = false;

    public override void Enter(object data = null)
    {
        gameObject.SetActive(true);

        GameManager.Instance.RequestPause();

        if (player == null) player = GameManager.Instance.GetPlayer();

        DialogueManager.Instance.OnDialogueContinue += DisplayNextDialogue;
        DialogueManager.Instance.OnDialogueEnded += DialogueEnded;

        CollapseChoices();

        if(choicePanelLayout == null) choicePanelLayout = choicesPanel.GetComponent<LayoutElement>();
    }

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

    private void DialogueEnded(Story story, BaseNPC npc)
    {
        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.CursorPanel);
    }

    public override void UpdateState()
    {
        if (canInput && !choosing && InputManager.Instance.lmbAction.WasPressedThisFrame())
            DialogueManager.Instance.ContinueDialogue();

        SetChoiceWidth(choosing ? 800f : 400f, choosing ? 500f : 1000f);
    }

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
    
    private void SetChoiceWidth(float desiredWidth, float lerp)
    {
        float newWidth = Mathf.MoveTowards(choicePanelLayout.minWidth, desiredWidth, lerp * Time.unscaledDeltaTime);

        choicePanelLayout.minWidth = newWidth;
    }

    private void CollapseChoices()
    {
        ClearChoices();
        choosing = false;
        canInput = true;
    }

    private void ClearChoices()
    {
        for (int i = 0; i < choicesPanel.childCount; i++)
        {
            Destroy(choicesPanel.GetChild(i).gameObject);
        }
    }

    private void MakeChoice(int choiceIndex)
    {
        DialogueManager.Instance.MakeChoice(choiceIndex);
        CollapseChoices();
    }

    private string GetDialogueLine(string line)
    {

        string result = line.Replace("you: ", "")
            .Replace("alias: ", "")
                .Replace("npc: ", "");

        return result;
    }

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
        else return "";
    }
}
