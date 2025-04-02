using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InspectionManager : MonoBehaviour
{
    public static InspectionManager Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject inspectionPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button cancelButton;

    [Header("Controls")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float zoomMin = 0.5f;
    [SerializeField] private float zoomMax = 1f;

    private Transform currentModel;
    public bool isInspecting = false;
    private float currentZoom = 0.5f;

    private Player player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        inspectionPanel.SetActive(false);
    }

    private void Update()
    {
        if (isInspecting && currentModel != null)
        {
            RotateModel();
            ZoomModel();

            if (InputManager.Instance.input.actions["inventory"].WasPressedThisFrame())
            {
                CloseInspection();
                PlayerInventory.Instance.OpenInventory();
            }
        }
    }

    public void StartInspection(InventoryItem item)
    {
        if (isInspecting) return;
        isInspecting = true;

        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        PlayerInventory.Instance.CloseInventory();
        PlayerInventory.Instance.UnequipItem();

        // Freeze Player
        player.canInput = false;
        player.phone.ClosePhone();
        player.cam.canInput = false;

        InputManager.Instance.SetCursor(false, false);
        GameStatusManager.Instance.PauseGame();

        // Show UI
        inspectionPanel.SetActive(true);
        itemNameText.text = item.itemData.itemName;
        itemDescriptionText.text = item.itemData.description;
        itemTypeText.text = item.itemData.itemType.ToString();

        // Spawn or move item model
        if (currentModel != null) Destroy(currentModel.gameObject);
        currentModel = Instantiate(item.itemData.modelPrefab, player.modelPoint).transform;

        currentZoom = zoomMax;

        currentModel.SetParent(player.modelPoint);
        currentModel.localPosition = Vector3.zero;
        currentModel.localRotation = Quaternion.identity;
        currentModel.gameObject.layer = LayerMask.NameToLayer("Hand");

        // Set Button Actions
        equipButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        equipButton.onClick.AddListener(() => EquipItem(item));
        dropButton.onClick.AddListener(() => DropItem(item));
        cancelButton.onClick.AddListener(CloseInspection);
    }

    private void RotateModel()
    {
        if (Input.GetMouseButton(0)) // Left Click Drag
        {
            float xRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float yRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
            currentModel.Rotate(Vector3.up, -xRotation, Space.World);
            currentModel.Rotate(Vector3.right, yRotation, Space.World);
        }
    }

    private void ZoomModel()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            currentZoom = Mathf.Clamp(currentZoom + scroll * zoomSpeed, zoomMin, zoomMax);
            player.modelPoint.localPosition = new Vector3(0, 0f, currentZoom);
        }
    }

    private void EquipItem(InventoryItem item)
    {
        PlayerInventory.Instance.EquipItem(item);
        CloseInspection();
    }

    private void DropItem(InventoryItem item)
    {
        PlayerInventory.Instance.DropItem(item);
        CloseInspection();
    }

    private void CloseInspection()
    {
        isInspecting = false;

        GameStatusManager.Instance.ResumeGame();

        player.canInput = true;
        player.cam.canInput = true;

        currentZoom = zoomMax;

        InputManager.Instance.SetCursor(true, true);
        inspectionPanel.SetActive(false);

        if (currentModel != null)
        {
            Destroy(currentModel.gameObject);
            currentModel = null;
        }
    }
}
