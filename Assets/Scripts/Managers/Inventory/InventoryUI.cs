using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect itemContainer;
    [SerializeField] private RectTransform parcelGrid;
    [SerializeField] private RectTransform inventoryGrid;
    [SerializeField] private Button parcelBtnTab;
    [SerializeField] private Button inventoryBtnTab;
    [SerializeField] private ItemButton itemButtonPrefab;

    private void Start()
    {
        SwitchTab(false);
        PlayerInventory.Instance.OnEquipItem += PopulateGrid;
        PlayerInventory.Instance.OnDropItem += PopulateGrid;
        PopulateGrid();
    }

    private void OnEnable()
    {
        GameStatusManager.Instance.PauseGame();

        InputManager.Instance.SetCursor(false, false);
    }

    private void OnDestroy()
    {
        PlayerInventory.Instance.OnEquipItem -= PopulateGrid;
        PlayerInventory.Instance.OnDropItem -= PopulateGrid;
    }

    private void OnDisable()
    {
        GameStatusManager.Instance.ResumeGame();

        TooltipManager.Instance.HideTooltip();
        if (!InspectionManager.Instance.isInspecting)
        {
            InputManager.Instance.SetCursor(true, true);
        }
       
    }

    public void PopulateGrid(InventoryItem _ = null)
    {
        RemoveGridChildren();

        foreach (InventoryItem data in PlayerInventory.Instance.parcels)
        {
            ItemButton item = Instantiate(itemButtonPrefab, parcelGrid);
            item.SetItem(data);
        }

        foreach (InventoryItem data in PlayerInventory.Instance.inventory)
        {
            ItemButton item = Instantiate(itemButtonPrefab, inventoryGrid);
            item.SetItem(data);
        }
    }

    private void RemoveGridChildren()
    {
        foreach (Transform child in parcelGrid) Destroy(child.gameObject);
        foreach (Transform child in inventoryGrid) Destroy(child.gameObject);
    }

    public void SwitchTab(bool isInventory)
    {
        TooltipManager.Instance.HideTooltip();

        inventoryGrid.gameObject.SetActive(isInventory);
        parcelGrid.gameObject.SetActive(!isInventory);

        itemContainer.content = isInventory ? inventoryGrid : parcelGrid;
        (isInventory ? inventoryBtnTab : parcelBtnTab).Select();
    }
}
