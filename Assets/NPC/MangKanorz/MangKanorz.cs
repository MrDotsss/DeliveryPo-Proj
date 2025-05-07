using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class MangKanorz : BaseNPC, IInteractable
{
    [Header("References")]
    public Quest questData;
    public TalkingComponent talkingComponent;
    public CharacterController controller;
    private StateMachine<EKanorzState> stateMachine;

    [Header("Properties")]
    public float speed = 1f;
    public float acceleration = 2f;
    public float friction = 5f;

    private Vector3 velocity;

    public QuestItem questItem { get; private set; }
    public bool accepted { get; private set; }
    public bool tooFar { get; private set; }

    private void Start()
    {
        Initialize();

        DialogueManager.Instance.OnDialogueContinue += CheckDialogue;

        controller = GetComponent<CharacterController>();
        stateMachine = GetComponent<StateMachine<EKanorzState>>();
    }
    public string GetInteractionText()
    {
        if (questItem == null) return $"Press 'F' to Talk to {aliasName}";
        else return $"Press 'F' to Talk to {npcName}";
    }

    private void Update()
    {
        if (accepted) tooFar = Vector3.Distance(transform.position, GetPlayer().transform.position) > 5f;

        controller.Move(velocity * Time.deltaTime);
    }

    public void Interact()
    {
        talkingComponent.Activate();
        stateMachine.TransitionTo(EKanorzState.Talk);

        if (questItem != null)
        {
            DialogueManager.Instance.SetCustomVariable("accepted", accepted);
            DialogueManager.Instance.SetCustomVariable("tooFar", tooFar);
            DialogueManager.Instance.SetCustomVariable("questFinished", questItem.IsFinished);
        }

    }


    private void CheckDialogue(Story story, BaseNPC npc)
    {
        if (npc == null) return;

        if (npc.npcName == npcName)
        {
            if (story.currentTags.Contains("quest"))
            {
                questItem = QuestManager.Instance.RegisterQuest(questData);
                accepted = true;
            }

        }
    }

    public void DoMove(float speed, float amount)
    {
        Vector3 direction = GetPlayer().transform.position - transform.position;

        if (!controller.isGrounded) velocity.y += Physics.gravity.y * Time.deltaTime;
        else velocity.y = 0;

        velocity = Vector3.Lerp(velocity, direction.normalized * speed, amount * Time.deltaTime);

    }

    public void LookAt(Vector3 targetPosition, float speed = 16)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Optional: Ignore vertical rotation

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.name == "stairs" && !questItem.IsFinished)
        {
            questItem.IsFinished = true;
            Interact();
        }
    }

}
