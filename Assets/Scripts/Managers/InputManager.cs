
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public PlayerInput input;
    [Header("Visual")]
    [SerializeField] private GameObject cursorUi;

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

        if (isUIMode) SetCursor(false);
    }

    public void SetCursor(bool cursorLocked = false, bool uiVisible = false)
    {
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
        cursorUi.SetActive(uiVisible);
    }

    public void PlayerCanInput(bool canInput)
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (player != null) player.canInput = canInput;
    }
}
