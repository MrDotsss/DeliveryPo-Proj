using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Player : Character
{
    [Header("References")]
    public Transform orientation;
    public FPSCamera cam;
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerHand Hand { get; private set; }

    [Header("Mobility")]
    public float walkSpeed = 10.0f;
    public float runSpeed = 15.0f;
    public float crouchSpeed = 5.0f;
    public float jumpStrength = 10.0f;

    public float acceleration = 3.0f;
    public float friction = 25.0f;

    private Coroutine crouchCoroutine;

    public List<Sprite> imageSprites = new List<Sprite>();

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = GetComponentInChildren<FPSCamera>();
        Hand = GetComponentInChildren<PlayerHand>();
        StateMachine = gameObject.GetComponent<PlayerStateMachine>();

        InputManager.Instance.Inventory += ToggleInventory;
        InputManager.Instance.Phone += TogglePhone;

        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.CursorPanel);

        transform.position = SaveNLoadManager.Instance.LoadPlayer();
    }

    private void OnDestroy()
    {
        InputManager.Instance.Inventory -= ToggleInventory;
    }

    private void ToggleInventory()
    {
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inventory);
    }

    private void TogglePhone()
    {
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Phone);
    }

    private void Update()
    {
        PerformCheckers();

        cam.MoveCamera();

        float speed = new Vector2(velocity.x, velocity.z).magnitude;
        float tiltInput = InputManager.Instance.Move.x;
        if (speed > 0.5f) cam.BobCamera(true, speed, 0.08f);
        else cam.BobCamera(true, 1.5f, 0.05f);

        cam.TiltCamera(-tiltInput * 3f, 10f);
    }

    public Vector3 GetMoveDirection()
    {
        Vector2 inputDir = InputManager.Instance.Move;

        Vector3 dir = orientation.forward * inputDir.y + orientation.right * inputDir.x;

        return dir.normalized;
    }

    public void DoJump(float strength)
    {
        velocity.y = strength;
    }

    public void DoCrouch(bool isCrouch)
    {
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
        }

        crouchCoroutine = StartCoroutine(Crouching(isCrouch));
    }

    private IEnumerator Crouching(bool crouch)
    {
        float startingHeight = controller.height;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * crouchSpeed;
            controller.height = Mathf.Lerp(startingHeight, (crouch ? baseHeight / 2 : baseHeight), t);

            yield return null;
        }

        controller.height = crouch ? baseHeight / 2 : baseHeight;
    }
}
