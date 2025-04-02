using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PhoneCameraState : PhoneState
{
    public override EPhoneStates StateType => EPhoneStates.Camera;

    [SerializeField] private float captureDistance;
    [SerializeField] private LayerMask whatToCapture;
    [SerializeField] private TextMeshProUGUI captureText;
    [SerializeField] private Camera phoneCam;

    private RaycastHit captureHit;

    public override void Enter(Dictionary<string, object> data = null)
    {
        phoneCam.enabled = true;
        phone.cameraApp.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        phoneCam.enabled = false;
        phone.cameraApp.gameObject.SetActive(false);
        phone.canInput = true;
    }

    public override void PhysicsUpdate()
    {

    }

    public override void UpdateState()
    {
        if (phone.currentPressed != "camera")
        {
            stateMachine.TransitionTo(phone.GetState());
            return;
        }

        if (DialogueManager.Instance.IsDialogueActive)
        {
            phone.canInput = false;
            phone.DoCamAim(false);
            return;
        }

        if (InputManager.Instance.input.actions["right-action"].IsPressed())
        {
            phone.canInput = false;
            phone.DoCamAim(true);
            InputManager.Instance.SetCursor(true, false);

            if (Physics.Raycast(phone.transform.position, phone.transform.forward, out captureHit, captureDistance, whatToCapture))
            {

                if (captureHit.transform.TryGetComponent(out CaptureBox captureBox))
                {
                    captureText.gameObject.SetActive(true);

                    if (captureHit.distance > captureBox.captureDistance)
                    {
                        captureText.text = "Too far";
                    }
                    else if (captureHit.distance < 0.5f)
                    {
                        captureText.text = "Too close";
                    }
                    else
                    {
                        captureText.text = "Good";

                        if (InputManager.Instance.input.actions["left-action"].WasPressedThisFrame())
                        {
                            captureBox.Capture(captureHit.transform.parent.name);

                            Texture2D snapshot = CaptureSnapshot();
                            Texture2D landspace = RotateTexture(snapshot);
                            SaveLoadManager.Instance.SavePhoto(landspace,  $"{captureBox.fileName}");
                        }
                    }
                }
            }
            else
            {
                captureText.gameObject.SetActive(false);
            }

            Vector2 zoom = InputManager.Instance.input.actions["ui-scroll"].ReadValue<Vector2>().normalized;

            if (zoom.magnitude != 0)
            {
                phoneCam.fieldOfView -= (zoom.y * 4);
                phoneCam.fieldOfView = Mathf.Clamp(phoneCam.fieldOfView, phone.maxZoom, phone.defaultZoom);
            }
        }
        else
        {
            phone.canInput = true;
            phone.DoCamAim(false);
            captureText.gameObject.SetActive(false);
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
