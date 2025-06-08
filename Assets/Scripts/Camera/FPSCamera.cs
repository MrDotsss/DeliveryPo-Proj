using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// FPSCamera controls the player's first-person camera, including movement, interactions, and various effects.
/// </summary>
public class FPSCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam; // Reference to the main camera
    [SerializeField] private Transform bobbingHead; // The transform used for camera bobbing effects
    [SerializeField] private Transform orientation; // The orientation reference for player movement

    [Header("Properties")]
    public float reachDistance = 1.0f; // Maximum distance for interaction detection
    public LayerMask interactableMask; // Layer mask for interactable objects

    private Vector2 rotationAxis = Vector2.zero; // Tracks camera rotation input
    private Transform lookAtTarget = null; // Target to focus on during special interactions
    private Quaternion defaultRotation = Quaternion.identity; // Stores default rotation before focus effects
    private Coroutine focusCoroutine; // Coroutine for smooth focus transitions
    private float bobTimer = 0.0f; // Timer for bobbing calculations

    private bool canInteract = false; // Determines if an interactable object is within range
    private RaycastHit hitInfo; // Stores interaction raycast results

    private CursorUI cursorUI; // Reference to the cursor UI element

    /// <summary>
    /// Initializes interaction input and UI references.
    /// </summary>
    private void Start()
    {
        InputManager.Instance.Interact += OnInteract;
        cursorUI = UIManager.Instance.GetUIPanel(UIManager.EUIPanels.CursorPanel) as CursorUI;
    }

    /// <summary>
    /// Cleans up interactions when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        InputManager.Instance.Interact -= OnInteract;
    }

    /// <summary>
    /// Handles interaction detection and updates UI prompts.
    /// </summary>
    private void Update()
    {
        canInteract = Physics.Raycast(transform.position, transform.forward, out hitInfo, reachDistance, interactableMask);

        // Update cursor UI based on interaction availability.
        if (canInteract && hitInfo.collider.TryGetComponent(out IInteractable interactable))
        {
            cursorUI?.SetInteractCursor(interactable.InteractionText, true);
        }
        else
        {
            cursorUI?.SetInteractCursor(string.Empty, false);
        }
    }

    /// <summary>
    /// Triggers interaction with an object if available.
    /// </summary>
    private void OnInteract()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Paused) return;

        if (canInteract && hitInfo.collider.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
    }

    /// <summary>
    /// Handles first-person camera movement and rotation clamping.
    /// </summary>
    public void MoveCamera(float hLimit = 0, float vLimit = 90)
    {
        rotationAxis.y += InputManager.Instance.Look.x * GameSettings.Instance.MouseSensitivity;
        rotationAxis.x -= InputManager.Instance.Look.y * GameSettings.Instance.MouseSensitivity;

        rotationAxis.x = Mathf.Clamp(rotationAxis.x, -vLimit, vLimit);
        if (hLimit > 0) rotationAxis.y = Mathf.Clamp(rotationAxis.y, -hLimit, hLimit);

        transform.localRotation = Quaternion.Euler(rotationAxis.x, rotationAxis.y, 0);
        orientation.rotation = Quaternion.Euler(0, rotationAxis.y, 0);
    }

    /// <summary>
    /// Adjusts the camera's field of view to create a zoom effect.
    /// </summary>
    public void ZoomCam(float amount, float lerp = 32f)
    {
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, amount, lerp * Time.deltaTime);
    }

    /// <summary>
    /// Tilts the camera for added movement dynamics.
    /// </summary>
    public void TiltCamera(float tiltAmount, float tiltSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltAmount);
        bobbingHead.localRotation = Quaternion.Slerp(bobbingHead.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    /// <summary>
    /// Simulates a bobbing effect while the player moves.
    /// </summary>
    public void BobCamera(bool isMoving, float speed, float amplitude)
    {
        if (!isMoving)
        {
            bobTimer = 0f;
            bobbingHead.localPosition = Vector3.Lerp(bobbingHead.localPosition, Vector3.zero, speed * Time.deltaTime);
            return;
        }

        bobTimer += Time.deltaTime * speed;
        float bobOffset = Mathf.Sin(bobTimer) * amplitude;

        Vector3 localPos = bobbingHead.localPosition;
        bobbingHead.localPosition = new Vector3(-bobOffset * 0.5f, bobOffset, localPos.z);
    }

    /// <summary>
    /// Focuses the camera on a given target.
    /// </summary>
    public void FocusAt(Transform target)
    {
        if (focusCoroutine != null) StopCoroutine(focusCoroutine);
        focusCoroutine = StartCoroutine(FocusCoroutine(target));
    }

    /// <summary>
    /// Stops the focus effect and resets the camera orientation.
    /// </summary>
    public void StopFocus()
    {
        if (focusCoroutine != null) StopCoroutine(focusCoroutine);
        focusCoroutine = StartCoroutine(ResetFocusCoroutine());
    }

    /// <summary>
    /// Smoothly rotates the camera to look at a target.
    /// </summary>
    private IEnumerator FocusCoroutine(Transform target)
    {
        lookAtTarget = target;
        defaultRotation = transform.rotation;

        while (lookAtTarget != null)
        {
            Vector3 direction = (lookAtTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            yield return null;
        }
    }

    /// <summary>
    /// Gradually resets the camera's rotation after a focus effect.
    /// </summary>
    private IEnumerator ResetFocusCoroutine()
    {
        Quaternion startRotation = transform.rotation;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            transform.rotation = Quaternion.Slerp(startRotation, defaultRotation, t);
            yield return null;
        }

        lookAtTarget = null;
    }

    #region Debugging
    /// <summary>
    /// Visualizes interaction rays for debugging in the Unity editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        DebugDrawer drawer = new DebugDrawer();
        drawer.DrawRaySphereCheck(transform.position, transform.forward, reachDistance, 0.03f, canInteract ? Color.green : Color.red);
    }
    #endregion
}
