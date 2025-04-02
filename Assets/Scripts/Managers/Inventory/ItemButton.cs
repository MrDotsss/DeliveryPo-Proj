using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Image itemIcon; // Explicitly assigned via inspector

    private InventoryItem inventoryItem;

    private void Awake()
    {
        if (itemIcon != null) itemIcon.sprite = inventoryItem.itemData.icon;
    }

    public void SetItem(InventoryItem inventoryItem)
    {
        this.inventoryItem = inventoryItem;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TooltipManager.Instance.ShowOptions(Input.mousePosition, inventoryItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!TooltipManager.Instance.optionsPanel.activeSelf && inventoryItem != null)
        {
            TooltipManager.Instance.ShowTooltip(inventoryItem.itemData.itemName);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!TooltipManager.Instance.optionsPanel.activeSelf)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}
