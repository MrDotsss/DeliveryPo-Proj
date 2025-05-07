using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    public GameObject optionsPanel;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button inspectButton;
    [SerializeField] private Button dropButton;

    [SerializeField] private Vector2 offset = new Vector2(10f, -10f);

    private RectTransform tooltipRect;
    private InventoryItem currentItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        tooltipPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (tooltipPanel == null) return;

        if (tooltipPanel.activeSelf && !optionsPanel.activeSelf)
        {
            // Move tooltip with the mouse
            Vector2 mousePosition = Input.mousePosition;
            tooltipRect.position = mousePosition + offset;
        }
    }

    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }

    public void ShowOptions(Vector3 position, InventoryItem item)
    {
        currentItem = item;
        optionsPanel.SetActive(true);
        tooltipPanel.transform.position = position;
        tooltipText.text = item.itemData.itemName;

        // Hide all buttons initially
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(true);
        inspectButton.gameObject.SetActive(true);

        // Check if the item is equipped
        if (PlayerInventory.Instance.equippedItem == item)
        {
            unequipButton.gameObject.SetActive(true);
        }
        else
        {
            equipButton.gameObject.SetActive(true);
        }

        // Assign button functions
        equipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();
        inspectButton.onClick.RemoveAllListeners();
        dropButton.onClick.RemoveAllListeners();

        equipButton.onClick.AddListener(EquipItem);
        unequipButton.onClick.AddListener(UnequipItem);
        inspectButton.onClick.AddListener(InspectItem);
        dropButton.onClick.AddListener(DropItem);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    private void EquipItem()
    {
        PlayerInventory.Instance.EquipItem(currentItem);
        HideTooltip();
    }

    private void UnequipItem()
    {
        PlayerInventory.Instance.UnequipItem(); // Implement this method in PlayerInventory
        HideTooltip();
    }

    private void InspectItem()
    {
        InspectionManager.Instance.StartInspection(currentItem);
        HideTooltip();
    }

    private void DropItem()
    {
        PlayerInventory.Instance.DropItem(currentItem);
        HideTooltip();
    }
}
