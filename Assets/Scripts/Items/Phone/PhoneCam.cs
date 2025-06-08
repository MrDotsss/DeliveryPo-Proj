using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// PhoneCam handles a simulated phone camera functionality, 
/// allowing zooming, aiming, and capturing images.
/// </summary>
public class PhoneCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image camBorder; // UI border to indicate capture status

    [Header("Zooming")]
    [SerializeField] private Camera phoneCam; // Camera used for capturing images
    [SerializeField] private float zoomIn = 30; // Minimum zoom level
    [SerializeField] private float zoomOut = 80; // Maximum zoom level

    [Header("Aiming")]
    [SerializeField] private float aimDistance = 100f; // Maximum aiming distance
    [SerializeField] private LayerMask aimMask; // Layer mask for aiming at specific objects
    [SerializeField] private Vector3 aimPosition; // Desired position while aiming
    [SerializeField] private Quaternion aimRotation; // Desired rotation while aiming
    [Space]
    [SerializeField] private float aimSpeed = 12f; // Speed of aim transition

    private Vector3 defaultPosition; // Stores the default position of the phone cam
    private Quaternion defaultRotation; // Stores the default rotation of the phone cam

    private InputAction rmbAction; // Right mouse button input action for aiming
    private InputAction lmbAction; // Left mouse button input action for capturing
    private Vector2 zoom; // Stores scroll wheel input for zooming

    private RaycastHit hitInfo; // Stores information about raycast collisions

    /// <summary>
    /// Initializes default values and input actions.
    /// </summary>
    private void Start()
    {
        defaultPosition = transform.localPosition;
        defaultRotation = transform.localRotation;

        rmbAction = InputManager.Instance.rmbAction;
        lmbAction = InputManager.Instance.lmbAction;

        if (rmbAction == null)
        {
            Debug.LogError("rmbAction does not exist");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the PhoneCam is enabled.
    /// Unequips the current inventory item and subscribes to inventory events.
    /// </summary>
    private void OnEnable()
    {
        InventoryManager.Instance.UnEquipItem();
        InventoryManager.Instance.OnEquip += DisablePhone;
    }

    /// <summary>
    /// Called when the PhoneCam is disabled.
    /// Re-equips the current inventory item and unsubscribes from inventory events.
    /// </summary>
    private void OnDisable()
    {
        InventoryManager.Instance.EquipItem(InventoryManager.Instance.CurrentItem);
        InventoryManager.Instance.OnEquip -= DisablePhone;
    }

    /// <summary>
    /// Disables the phone camera.
    /// </summary>
    public void DisablePhone(InventoryItem item = null)
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enables the phone camera.
    /// </summary>
    public void EnablePhone()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Handles aiming, zooming, and updating the UI.
    /// </summary>
    private void Update()
    {
        if (rmbAction.IsPressed())
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, aimPosition, aimSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, aimRotation, aimSpeed * Time.deltaTime);

            CheckOnCapture();
        }
        else
        {
            camBorder.color = Color.black;

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, defaultPosition, aimSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultRotation, aimSpeed * Time.deltaTime);
        }

        // Handle camera zoom based on mouse scroll input
        zoom = InputManager.Instance.Scroll;
        if (zoom.magnitude != 0)
        {
            phoneCam.fieldOfView -= (zoom.y * 0.1f);
            phoneCam.fieldOfView = Mathf.Clamp(phoneCam.fieldOfView, zoomIn, zoomOut);
        }
    }

    /// <summary>
    /// Checks if the player is capturing an object and handles UI feedback.
    /// </summary>
    private void CheckOnCapture()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, aimDistance, aimMask))
        {
            if (hitInfo.transform.TryGetComponent(out CaptureBox capture))
            {
                // Provide feedback based on the capture distance.
                camBorder.color = (hitInfo.distance < capture.minDistance || hitInfo.distance > capture.maxDistance) ? Color.red : Color.green;

                if (lmbAction.WasPressedThisFrame())
                {
                    capture.Capture();

                    Texture2D snapShot = CaptureSnapshot();
                    Texture2D landscape = RotateTexture(snapShot);

                    ImageData data = new ImageData(capture.fileName, capture.description, landscape);
                    SaveNLoadManager.Instance.SavePhoto(data);
                }
            }
        }
        else
        {
            camBorder.color = Color.black;
        }
    }

    /// <summary>
    /// Captures an image from the phone camera and applies gamma correction.
    /// </summary>
    private Texture2D CaptureSnapshot()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = phoneCam.targetTexture;

        Texture2D capturedTexture = new Texture2D(phoneCam.targetTexture.width, phoneCam.targetTexture.height, TextureFormat.ARGB32, false);
        capturedTexture.ReadPixels(new Rect(0, 0, phoneCam.targetTexture.width, phoneCam.targetTexture.height), 0, 0);
        capturedTexture.Apply();

        RenderTexture.active = activeRenderTexture;

        ApplyGammaCorrection(capturedTexture);

        return capturedTexture;
    }

    /// <summary>
    /// Rotates the captured texture to match the expected orientation.
    /// </summary>
    private Texture2D RotateTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D rotatedTexture = new Texture2D(height, width);

        Color32[] originalPixels = texture.GetPixels32();
        Color32[] rotatedPixels = new Color32[originalPixels.Length];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                rotatedPixels[x * height + (height - y - 1)] = originalPixels[y * width + x];
            }
        }

        rotatedTexture.SetPixels32(rotatedPixels);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    /// <summary>
    /// Applies gamma correction to the captured texture.
    /// </summary>
    private void ApplyGammaCorrection(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(
                (byte)(Mathf.Pow(pixels[i].r / 255f, 1 / 2.2f) * 255),
                (byte)(Mathf.Pow(pixels[i].g / 255f, 1 / 2.2f) * 255),
                (byte)(Mathf.Pow(pixels[i].b / 255f, 1 / 2.2f) * 255),
                pixels[i].a
            );
        }
        texture.SetPixels32(pixels);
        texture.Apply();
    }
}
