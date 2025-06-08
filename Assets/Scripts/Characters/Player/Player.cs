using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// <summary>
/// Player class inherits from Character and manages player-specific mechanics including movement, camera, states, and input.
/// </summary>
public class Player : Character
{
    [Header("References")]
    public Transform orientation;          // Reference for movement orientation
    public FPSCamera cam;                  // Reference to player's camera component
    public PlayerStateMachine StateMachine { get; private set; } // Player's state machine
    public PlayerHand Hand { get; private set; }                 // Player's hand component for interactions

    [Header("Mobility")]
    public float walkSpeed = 10.0f;        // Walking speed
    public float runSpeed = 15.0f;         // Running speed
    public float crouchSpeed = 5.0f;       // Crouch speed modifier for transition
    public float crouchHeightFactor = 0.5f;// Factor to reduce height while crouching
    public float jumpStrength = 10.0f;     // Jump strength force

    public float acceleration = 3.0f;      // Acceleration rate for movement
    public float friction = 25.0f;         // Friction applied to velocity

    private Coroutine crouchCoroutine;     // Coroutine handle for crouch transitions

    public List<Sprite> imageSprites = new List<Sprite>(); // List of sprites for player UI or inventory

    /// <summary>
    /// Initialization: Cache components, set original height, subscribe to input events, load player position.
    /// </summary>
    private void Start()
    {
        originalHeight = controller.height;
        cam = GetComponentInChildren<FPSCamera>();
        Hand = GetComponentInChildren<PlayerHand>();
        StateMachine = gameObject.GetComponent<PlayerStateMachine>();

        InputManager.Instance.Inventory += ToggleInventory;
        InputManager.Instance.Phone += TogglePhone;

        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.CursorPanel);

        transform.position = SaveNLoadManager.Instance.LoadPlayer();
    }

    /// <summary>
    /// Cleanup: Unsubscribe from input events on destruction.
    /// </summary>
    private void OnDestroy()
    {
        InputManager.Instance.Inventory -= ToggleInventory;
    }

    /// <summary>
    /// Toggles the inventory UI panel.
    /// </summary>
    private void ToggleInventory()
    {
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inventory);
    }

    /// <summary>
    /// Toggles the phone UI panel.
    /// </summary>
    private void TogglePhone()
    {
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Phone);
    }

    /// <summary>
    /// Called every frame: Performs ground/ceiling checks, updates camera position, applies camera bob and tilt based on movement and input.
    /// </summary>
    private void Update()
    {
        PerformCheckers();

        cam.MoveCamera();

        float speed = new Vector2(velocity.x, velocity.z).magnitude;
        float tiltInput = InputManager.Instance.Move.x;

        if (speed > 0.5f)
            cam.BobCamera(true, speed / 2, 0.08f);
        else
            cam.BobCamera(true, 1.5f, 0.05f);

        cam.TiltCamera(-tiltInput * 3f, 10f);
    }

    /// <summary>
    /// Gets normalized move direction vector based on input and player orientation.
    /// </summary>
    /// <returns>Normalized movement direction</returns>
    public Vector3 GetMoveDirection()
    {
        Vector2 inputDir = InputManager.Instance.Move;
        Vector3 dir = orientation.forward * inputDir.y + orientation.right * inputDir.x;
        return dir.normalized;
    }

    /// <summary>
    /// Applies vertical jump velocity.
    /// </summary>
    /// <param name="strength">Jump strength value</param>
    public void DoJump(float strength)
    {
        velocity.y = strength;
    }

    /// <summary>
    /// Starts or stops crouching by starting the crouch coroutine.
    /// </summary>
    /// <param name="isCrouch">If true, crouch; otherwise stand</param>
    public void DoCrouch(bool isCrouch)
    {
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
        }

        crouchCoroutine = StartCoroutine(Crouching(isCrouch));
    }

    /// <summary>
    /// Coroutine to smoothly change controller height when crouching or standing.
    /// </summary>
    /// <param name="crouch">Whether to crouch or stand</param>
    /// <returns>IEnumerator for coroutine</returns>
    IEnumerator Crouching(bool crouch)
    {
        float crouchHeight = originalHeight * crouchHeightFactor;

        float startHeight = controller.height;
        float targetHeight = crouch ? crouchHeight : originalHeight;

        float startCenterY = controller.center.y;
        float targetCenterY = startCenterY + (targetHeight - startHeight) / 2f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * crouchSpeed;

            controller.height = Mathf.Lerp(startHeight, targetHeight, t);
            //controller.center = new Vector3(0, Mathf.Lerp(startCenterY, targetCenterY, t), 0);

            yield return null;
        }

        controller.height = targetHeight;
        //controller.center = new Vector3(0, targetCenterY, 0);
    }
}
