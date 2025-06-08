using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PhoneCam : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image camBorder;
    [Header("Zooming")]
    [SerializeField] private Camera phoneCam;
    [SerializeField] private float zoomIn = 30;
    [SerializeField] private float zoomOut = 80;
    [Header("Aiming")]
    [SerializeField] private float aimDistance = 100f;
    [SerializeField] private LayerMask aimMask;
    [SerializeField] private Vector3 aimPosition;
    [SerializeField] private Quaternion aimRotation;
    [Space]
    [SerializeField] private float aimSpeed = 12f;

    private Vector3 defaultPosition;
    private Quaternion defaultRotation;

    private InputAction rmbAction;
    private InputAction lmbAction;
    private Vector2 zoom;

    private RaycastHit hitInfo;

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

    private void OnEnable()
    {
        InventoryManager.Instance.UnEquipItem();
        InventoryManager.Instance.OnEquip += DisablePhone;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.EquipItem(InventoryManager.Instance.CurrentItem);
        InventoryManager.Instance.OnEquip -= DisablePhone;
    }

    public void DisablePhone(InventoryItem item = null)
    {
        gameObject.SetActive(false);
    }

    public void EnablePhone()
    {
        gameObject.SetActive(true);
    }

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

        zoom = InputManager.Instance.Scroll;

        if (zoom.magnitude != 0)
        {
            phoneCam.fieldOfView -= (zoom.y * 0.1f);
            phoneCam.fieldOfView = Mathf.Clamp(phoneCam.fieldOfView, zoomIn, zoomOut);
        }
    }

    private void CheckOnCapture()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, aimDistance, aimMask))
        {
            if (hitInfo.transform.TryGetComponent(out CaptureBox capture))
            {
                if (hitInfo.distance < capture.minDistance || hitInfo.distance > capture.maxDistance)
                {
                    camBorder.color = Color.red;
                }
                else
                {
                    camBorder.color = Color.green;

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
        }else
        {
            camBorder.color = Color.black;
        }
    }

    private Texture2D CaptureSnapshot()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = phoneCam.targetTexture;

        Texture2D capturedTexture = new Texture2D(phoneCam.targetTexture.width, phoneCam.targetTexture.height, TextureFormat.ARGB32, false);
        capturedTexture.ReadPixels(new Rect(0, 0, phoneCam.targetTexture.width, phoneCam.targetTexture.height), 0, 0);
        capturedTexture.Apply();

        RenderTexture.active = activeRenderTexture;

        // Apply gamma correction manually
        ApplyGammaCorrection(capturedTexture);

        return capturedTexture;
    }

    private Texture2D RotateTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;
        Texture2D rotatedTexture = new Texture2D(height, width); // Swap width & height

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
