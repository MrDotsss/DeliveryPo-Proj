using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIState for inspecting inventory items or images.
/// Handles displaying item info, rotating and zooming 3D models,
/// and UI interactions for equip, drop, and close buttons.
/// </summary>
public class InspectionUI : UIState
{
    public override UIManager.EUIPanels State => UIManager.EUIPanels.Inspection;

    [Header("Buttons")]
    public Button closeBtn;
    public Button equipBtn;
    public TextMeshProUGUI equipBtnText;
    public Button dropBtn;

    [Header("Texts")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemTypeText;
    public TextMeshProUGUI itemDescriptionText;

    [Header("Image")]
    public RawImage rawImage; // For displaying 2D images

    [Header("Properties")]
    public float zoomSpeed = 0.1f;
    public float zoomIn = 1f;
    public float zoomOut = 2f;
    private float zoomCount = 0;

    public float rotationSpeed = 0.2f;

    private InventoryItem currentItem;
    private GameObject currentObject;  // 3D model instance for inspection
    private GameObject currentLight;   // Point light for the inspected object

    private Player player;

    private bool imageMode = false; // True if inspecting an image instead of 3D model

    /// <summary>
    /// Called when inspection UI panel is entered.
    /// Initializes UI with the passed item or image data,
    /// sets up button listeners and pauses the game.
    /// </summary>
    /// <param name="data">InventoryItem or ImageData to inspect.</param>
    public override void Enter(object data = null)
    {
        if (data == null)
        {
            Debug.LogError($"There is no item data passed on Enter");
            return;
        }

        equipBtn.onClick.AddListener(ListenEquip);
        dropBtn.onClick.AddListener(ListenDrop);
        closeBtn.onClick.AddListener(ListenClose);
        GameManager.Instance.RequestPause();
        UIManager.Instance.canSwitch = false;
        gameObject.SetActive(true);
        currentLight?.SetActive(false);
        rawImage.transform.parent.gameObject.SetActive(false);

        if (data.GetType() == typeof(InventoryItem))
        {
            InstanceInspection((InventoryItem)data);
        }
        else if (data.GetType() == typeof(ImageData))
        {
            InstanceInspection((ImageData)data);
            imageMode = true;
            equipBtn.gameObject.SetActive(false);
            dropBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Called when exiting the inspection panel.
    /// Cleans up instantiated objects, removes listeners,
    /// unpauses the game, and resets UI.
    /// </summary>
    public override void Exit()
    {
        gameObject.SetActive(false);
        currentLight?.SetActive(false);
        GameManager.Instance.RequestUnpause();

        if (!imageMode)
        {
            equipBtn.onClick.RemoveAllListeners();
            dropBtn.onClick.RemoveAllListeners();
        }

        closeBtn.onClick.RemoveAllListeners();
        currentItem = null;
        Destroy(currentObject);
        rawImage.transform.parent.gameObject.SetActive(false);
        imageMode = false;
    }

    /// <summary>
    /// Called every frame while the inspection panel is active.
    /// Handles model rotation and zoom based on user input.
    /// </summary>
    public override void UpdateState()
    {
        base.UpdateState();

        RotateModel(InputManager.Instance.GetMouseDelta());
        ZoomModel(InputManager.Instance.Scroll.y);
    }

    /// <summary>
    /// Rotates the inspected 3D model based on mouse delta input.
    /// </summary>
    /// <param name="delta">Mouse movement delta.</param>
    private void RotateModel(Vector2 delta)
    {
        if (currentObject == null) return;

        float rotX = delta.y * rotationSpeed;
        float rotY = -delta.x * rotationSpeed;

        currentObject.transform.Rotate(Vector3.right, rotX, Space.Self);
        currentObject.transform.Rotate(Vector3.up, rotY, Space.World);
    }

    /// <summary>
    /// Zooms the inspected 3D model in or out based on scroll input.
    /// </summary>
    /// <param name="zoom">Scroll wheel input.</param>
    private void ZoomModel(float zoom)
    {
        if (currentObject == null || zoom == 0) return;

        zoomCount += zoom * zoomSpeed;
        zoomCount = Mathf.Clamp(zoomCount, zoomIn, zoomOut);

        currentObject.transform.localPosition = new Vector3(0, 0, zoomCount);
    }

    /// <summary>
    /// Called when Equip button is clicked.
    /// Equips the current inspected item and toggles back to inventory UI.
    /// </summary>
    private void ListenEquip()
    {
        InventoryManager.Instance.EquipItem(currentItem);

        UIManager.Instance.canSwitch = true;
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inspection);
    }

    /// <summary>
    /// Called when Drop button is clicked.
    /// Drops the current inspected item and toggles back to inventory UI.
    /// </summary>
    private void ListenDrop()
    {
        InventoryManager.Instance.DropItem(currentItem);

        UIManager.Instance.canSwitch = true;
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inspection);
    }

    /// <summary>
    /// Called when Close button is clicked.
    /// Switches UI back to Phone panel if inspecting an image,
    /// or back to Inventory panel otherwise.
    /// </summary>
    private void ListenClose()
    {
        UIManager.Instance.canSwitch = true;

        if (imageMode)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Phone);
        }
        else
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Inventory);
        }
    }

    /// <summary>
    /// Instantiates and sets up the 3D model for the inspected InventoryItem.
    /// Also spawns a point light for better visibility.
    /// </summary>
    /// <param name="item">The inventory item to inspect.</param>
    private void InstanceInspection(InventoryItem item)
    {
        Destroy(currentObject);

        if (item == null) return;

        currentItem = item;

        itemNameText.text = currentItem.data.itemName;
        itemTypeText.text = currentItem.data.itemType.ToString();
        itemDescriptionText.text = currentItem.data.description;

        if (player == null) player = GameManager.Instance.GetPlayer();

        currentObject = Instantiate(currentItem.data.itemPrefab, player.cam.transform);
        currentObject.transform.localPosition = Vector3.forward * zoomOut;
        currentObject.transform.localRotation = new Quaternion(90, 0, 0, 1);

        currentObject.GetComponent<MeshRenderer>().material.renderQueue = 4000;
        currentObject.layer = LayerMask.NameToLayer("Hand");

        SpawnPointLight(Color.white, 1.5f, 3f, player.cam.transform);
    }

    /// <summary>
    /// Sets up the UI to inspect an image instead of a 3D model.
    /// Displays image data and description.
    /// </summary>
    /// <param name="item">Image data to display.</param>
    private void InstanceInspection(ImageData item)
    {
        Destroy(currentObject);

        if (item == null) return;

        itemNameText.text = item.fileName;
        itemTypeText.text = "Image";
        itemDescriptionText.text = item.description;

        rawImage.texture = item.texture;
        rawImage.transform.parent.gameObject.SetActive(true);
    }

    /// <summary>
    /// Creates or activates a point light attached to the inspected object for better illumination.
    /// </summary>
    /// <param name="color">Light color.</param>
    /// <param name="intensity">Light intensity.</param>
    /// <param name="range">Light range.</param>
    /// <param name="parent">Transform to parent the light to.</param>
    public void SpawnPointLight(Color color, float intensity, float range, Transform parent)
    {
        if (currentLight == null)
        {
            currentLight = new GameObject("Point Light");
            currentLight.transform.SetParent(parent, worldPositionStays: false);
            currentLight.transform.localPosition = Vector3.zero;
            Light lightComp = currentLight.AddComponent<Light>();
            lightComp.type = LightType.Point;
            lightComp.color = color;
            lightComp.intensity = intensity;
            lightComp.range = range;
            lightComp.cullingMask = LayerMask.GetMask("Hand");
        }
        else
        {
            currentLight.SetActive(true);
        }
    }

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        DebugDrawer drawer = new DebugDrawer();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Transform origin = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().cam.transform;

            drawer.DrawRayCheck(origin.position, origin.forward, zoomIn, Color.blue);
        }
    }
    #endregion
}
