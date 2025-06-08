using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : BaseManager<InputManager>
{
    [SerializeField] private PlayerInput input;

    #region InputAction Cache
    //player movement
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;

    //triggers
    private InputAction interactAction;
    private InputAction phoneAction;
    private InputAction scrollAction;
    private InputAction inventoryAction;

    private InputAction pauseAction;

    //manual overridde
    public InputAction lmbAction {  get; private set; }
    public InputAction rmbAction {  get; private set; }

    public InputAction cursorAction { get; private set; }
    #endregion

    #region Getters and Events
    //getters
    public bool MouseLocked {  get; private set; }
    public Vector2 Move => moveAction.ReadValue<Vector2>();
    public Vector2 Look => MouseLocked ? lookAction.ReadValue<Vector2>() : Vector2.zero;

    public Vector2 Scroll => scrollAction.ReadValue<Vector2>();

    public bool Jump => jumpAction.WasPressedThisFrame();
    public bool Crouch => crouchAction.IsPressed();
    public bool Sprint => sprintAction.IsPressed();

    //events
    public event Action Interact;
    public event Action Inventory;
    public event Action Phone;
    public event Action Pause;
    #endregion

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

    public void SetCursorLock(bool locked)
    {
        MouseLocked = locked;

        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    public Vector2 GetMouseDelta()
    {
        return lookAction.ReadValue<Vector2>();
    }
}
