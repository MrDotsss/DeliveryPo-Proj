using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Singleton InputManager that handles player input actions using the Unity Input System.
/// Provides cached InputActions for movement, looking, jumping, crouching, sprinting, and other interactions.
/// Manages cursor locking and exposes input events for key gameplay actions.
/// </summary>
public class InputManager : BaseManager<InputManager>
{
    [SerializeField] private PlayerInput input;

    #region InputAction Cache
    // Player movement and look input actions
    private InputAction moveAction;
    private InputAction lookAction;
    public InputAction jumpAction { get; private set; }
    private InputAction crouchAction;
    private InputAction sprintAction;

    // Interaction related actions
    private InputAction interactAction;
    private InputAction phoneAction;
    private InputAction scrollAction;
    private InputAction inventoryAction;

    private InputAction pauseAction;

    // Manual override input actions
    public InputAction lmbAction { get; private set; }
    public InputAction rmbAction { get; private set; }

    public InputAction cursorAction { get; private set; }
    #endregion

    #region Getters and Events
    // Indicates if the cursor is locked (usually during gameplay)
    public bool MouseLocked { get; private set; }

    // Current movement input vector (WASD / joystick)
    public Vector2 Move => moveAction.ReadValue<Vector2>();

    // Look input vector (mouse delta), returns zero if cursor is unlocked
    public Vector2 Look => MouseLocked ? lookAction.ReadValue<Vector2>() : Vector2.zero;

    // Scroll wheel input
    public Vector2 Scroll => scrollAction.ReadValue<Vector2>();

    // Jump button pressed this frame
    public bool Jump => jumpAction.WasPressedThisFrame();

    // Crouch button currently pressed
    public bool Crouch => crouchAction.IsPressed();

    // Sprint button currently pressed
    public bool Sprint => sprintAction.IsPressed();

    // Input events to subscribe for various triggers
    public event Action Interact;
    public event Action Inventory;
    public event Action Phone;
    public event Action Pause;
    #endregion

    /// <summary>
    /// Initializes InputActions by fetching them from the PlayerInput component and
    /// hooks up performed callbacks for interaction-related actions.
    /// Also sets the cursor to locked state by default.
    /// </summary>
    public void Start()
    {
        moveAction = input.actions["move"];
        lookAction = input.actions["look"];
        jumpAction = input.actions["jump"];
        crouchAction = input.actions["crouch"];
        sprintAction = input.actions["sprint"];

        interactAction = input.actions["interact"];
        inventoryAction = input.actions["inventory"];
        phoneAction = input.actions["phone"];
        scrollAction = input.actions["ui-scroll"];
        pauseAction = input.actions["pause"];

        interactAction.performed += _ => Interact?.Invoke();
        inventoryAction.performed += _ => Inventory?.Invoke();
        phoneAction.performed += _ => Phone?.Invoke();
        pauseAction.performed += _ => Pause?.Invoke();

        lmbAction = input.actions["lmb"];
        rmbAction = input.actions["rmb"];
        cursorAction = input.actions["cursor"];

        SetCursorLock(true);
    }

    /// <summary>
    /// Locks or unlocks the mouse cursor and updates its visibility.
    /// </summary>
    /// <param name="locked">If true, locks the cursor and hides it; otherwise unlocks and shows it.</param>
    public void SetCursorLock(bool locked)
    {
        MouseLocked = locked;

        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    /// <summary>
    /// Returns the current mouse delta movement vector from the look input action.
    /// </summary>
    /// <returns>Mouse delta as Vector2</returns>
    public Vector2 GetMouseDelta()
    {
        return lookAction.ReadValue<Vector2>();
    }
}
