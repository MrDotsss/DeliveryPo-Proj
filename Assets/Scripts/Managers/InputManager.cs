
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }

    public PlayerInput input;
    [Header("Visual")]
    [SerializeField] private GameObject cursorUi;

    private InputActionAsset action;

    public bool isUIMode = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (input == null)
        {
            Debug.LogError("Cannot find 'Player Input' component");
            return;
        }

        action = input.actions;

        if (isUIMode) UIMode();
    }

    public void UIMode(bool cursorLocked = false)
    {
        action.FindActionMap("Player")?.Disable();
        action.FindActionMap("UI")?.Enable();

        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;

        cursorUi.SetActive(false);

    }

    public void PlayerMode(bool cursorLocked = true)
    {
        action.FindActionMap("Player")?.Enable();
        action.FindActionMap("UI")?.Disable();

        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;

        cursorUi.SetActive(true);
    }
}
