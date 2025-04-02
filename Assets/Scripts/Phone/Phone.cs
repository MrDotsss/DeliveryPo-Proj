using UnityEngine;

public class Phone : MonoBehaviour
{
    [Header("Reference")]
    public Player player;
    [SerializeField] private GameObject model;
    [Header("Flashlight")]
    [SerializeField] private GameObject flashLight;
    [Header("Camera")]
    public float maxZoom = 10;
    public float defaultZoom = 60;
    [SerializeField] private Vector3 targetCamAim;
    private Vector3 startPhonePos;

    [Header("Screens")]
    public RectTransform mainMenu;
    public RectTransform deliveryApp;
    public RectTransform todoApp;
    public RectTransform cameraApp;

    public EPhoneStates previousScreen;
    public string currentPressed = "home";

    private InventoryItem previousEquiped;

    public bool canInput = true;

    private void Start()
    {
        model.SetActive(false);

        startPhonePos = transform.localPosition;

        PlayerInventory.Instance.OnEquipItem += ((invention) =>
        {
            model.SetActive(false);
            InputManager.Instance.SetCursor(true, true);
        });

        DialogueManager.Instance.OnDialogueStarted += ((str, npc) =>
        {
            currentPressed = "home";
            previousEquiped = null;
            ClosePhone();
            InputManager.Instance.SetCursor(false, false);
        });
    }

    private void Update()
    {
        if (InputManager.Instance.input.actions["phone"].WasPressedThisFrame() && !DialogueManager.Instance.IsDialogueActive)
        {
            if (model.activeSelf)
            {
                ClosePhone();
            }
            else
            {
                OpenPhone();
            }

        }

        if (model.activeSelf && canInput)
        {
            if (PlayerInventory.Instance.inventoryUI.gameObject.activeSelf) return;

            if (InputManager.Instance.input.actions["mouse-lock"].IsPressed())
            {
                player.cam.canInput = false;
                InputManager.Instance.SetCursor(false, false);
            }
            else
            {
                player.cam.canInput = true;
                InputManager.Instance.SetCursor(true, true);
            }
        }

    }

    public void OpenPhone()
    {
        previousEquiped = PlayerInventory.Instance.equippedItem;
        PlayerInventory.Instance.UnequipItem();

        model.SetActive(true);
        InputManager.Instance.SetCursor(false, false);
    }

    public void ClosePhone()
    {
        if (previousEquiped != null) PlayerInventory.Instance.EquipItem(previousEquiped);

        model.SetActive(false);
        InputManager.Instance.SetCursor(true, true);
    }

    public void ScreenButtonPressed(string whatPressed)
    {
        currentPressed = whatPressed;
    }

    public void ToggleFlashlight()
    {
        flashLight.SetActive(!flashLight.activeSelf);
    }

    public EPhoneStates GetState()
    {
        switch (currentPressed)
        {
            case "home":
                return EPhoneStates.Home;
            case "back":
                return previousScreen;
            case "delivery":
                return EPhoneStates.Delivery;
            case "todo":
                return EPhoneStates.Todo;
            case "camera":
                return EPhoneStates.Camera;
            case "gallery":
                return EPhoneStates.Gallery;
            case "phone":
                return EPhoneStates.Phone;
            default:
                return EPhoneStates.Home;
        }
    }

    public void DoCamAim(bool isAim = true)
    {
        if (isAim)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, 90), 8 * Time.deltaTime);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetCamAim, 4 * Time.deltaTime);
        }
        else
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 8 * Time.deltaTime);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, startPhonePos, 4 * Time.deltaTime);
        }

    }
}
