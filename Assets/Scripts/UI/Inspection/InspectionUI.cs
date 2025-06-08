using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public RawImage rawImage; // The Content RectTransform

    [Header("Properties")]
    public float zoomSpeed = 0.1f;
    public float zoomIn = 1f;
    public float zoomOut = 2f;
    private float zoomCount = 0;

    public float rotationSpeed = 0.2f;

    private InventoryItem currentItem;
    private GameObject currentObject;
    private GameObject currentLight;

    private Player player;

    private bool imageMode = false;

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
    }

    public override void UpdateState()
    {
        base.UpdateState();

        RotateModel(InputManager.Instance.GetMouseDelta());
        ZoomModel(InputManager.Instance.Scroll.y);
    }
    private void RotateModel(Vector2 delta)
    {
        if (currentObject == null) return;

        float rotX = delta.y * rotationSpeed;
        float rotY = -delta.x * rotationSpeed;

        currentObject.transform.Rotate(Vector3.right, rotX, Space.Self);
        currentObject.transform.Rotate(Vector3.up, rotY, Space.World);
    }

    private void ZoomModel(float zoom)
    {
        if (currentObject == null || zoom == 0) return;

        zoomCount += zoom * zoomSpeed;
        zoomCount = Mathf.Clamp(zoomCount, zoomIn, zoomOut);

        currentObject.transform.localPosition = new Vector3(0, 0, zoomCount);
    }

    private void ListenEquip()
    {
        InventoryManager.Instance.EquipItem(currentItem);

        UIManager.Instance.canSwitch = true;
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inspection);
    }

    private void ListenDrop()
    {
        InventoryManager.Instance.DropItem(currentItem);

        UIManager.Instance.canSwitch = true;
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inspection);
    }

    private void ListenClose()
    {
        UIManager.Instance.canSwitch = true;

        if(imageMode)
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Phone);
        } else
        {
            UIManager.Instance.SwitchPanel(UIManager.EUIPanels.Inventory);
        }
    }
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
