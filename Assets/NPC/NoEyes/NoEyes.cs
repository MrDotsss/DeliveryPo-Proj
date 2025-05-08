using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEyes : BaseNPC
{
    [SerializeField] private CharacterController controller;
    public LineOfSight lineOfSight;
    [Header("Properties")]
    public float speed = 3f;
    public float acceleration = 2f;
    public float friction = 5f;

    private Vector3 velocity;

    private void Start()
    {
        Initialize();

        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (controller.enabled)
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public void DoAttack()
    {
        Player player = GetPlayer();

        if (player != null)
        {
            DialogueManager.Instance.canInput = false;
            DialogueManager.Instance.StartDialogue(DialogueManager.Instance.GetDefaultDialogue("Caught"));
            PlayerInventory.Instance.UnequipItem();

            controller.enabled = false;
            player.canInput = false;
            player.phone.canInput = false;
            player.phone.ClosePhone();

            transform.SetParent(player.cam.head);
            transform.localPosition = new Vector3(0, -0.5f, 0.5f);
            transform.transform.localRotation = Quaternion.Euler(0, 180, 0);

            DoAnimate("Attack");

        }
    }




    public void SetVelocity(float x, float y, float z)
    {
        velocity = new Vector3(x, y, z);
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
}
