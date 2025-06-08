using System;
using System.Collections;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform bobbingHead;
    [SerializeField] private Transform orientation;

    [Header("Properties")]
    public float reachDistance = 1.0f;
    public LayerMask interactableMask;

    private Vector2 rotationAxis = Vector2.zero;
    private Transform lookAtTarget = null;
    private Quaternion defaultRotation = Quaternion.identity;
    private Coroutine focusCoroutine;
    private float bobTimer = 0.0f;

    private bool canInteract = false;

    private RaycastHit hitInfo;

    private CursorUI cursorUI;

    private void Start()
    {
        InputManager.Instance.Interact += OnInteract;
        cursorUI = UIManager.Instance.GetUIPanel(UIManager.EUIPanels.CursorPanel) as CursorUI;
    }

    private void OnDestroy()
    {
        InputManager.Instance.Interact -= OnInteract;
    }

    private void Update()
    {
        canInteract = Physics.Raycast(transform.position, transform.forward, out hitInfo, reachDistance, interactableMask);

        if (canInteract && hitInfo.collider.TryGetComponent(out IInteractable interactable))
        {
            cursorUI?.SetInteractCursor(interactable.InteractionText, true);
        }
        else
        {
            cursorUI?.SetInteractCursor(string.Empty, false);
        }
    }

    private void OnInteract()
    {
        if (GameManager.Instance.CurrentState == GameManager.GameState.Paused) return;

        if (canInteract && hitInfo.collider.TryGetComponent(out IInteractable interactable))
        {
            interactable.Interact();
        }
    }

    public void MoveCamera(float hLimit = 0, float vLimit = 90)
    {
        rotationAxis.y += InputManager.Instance.Look.x * GameSettings.Instance.MouseSensitivity;
        rotationAxis.x -= InputManager.Instance.Look.y * GameSettings.Instance.MouseSensitivity;

        rotationAxis.x = Mathf.Clamp(rotationAxis.x, -vLimit, vLimit);
        if (hLimit > 0) rotationAxis.y = Mathf.Clamp(rotationAxis.y, -hLimit, hLimit);

        transform.localRotation = Quaternion.Euler(rotationAxis.x, rotationAxis.y, 0);
        orientation.rotation = Quaternion.Euler(0, rotationAxis.y, 0);
    }

    public void ZoomCam(float amount, float lerp = 32f)
    {
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, amount, lerp * Time.deltaTime);
    }

    public void TiltCamera(float tiltAmount, float tiltSpeed)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, tiltAmount);
        bobbingHead.localRotation = Quaternion.Slerp(bobbingHead.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

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
        bobbingHead.localPosition = new Vector3(-bobOffset * 0.5f, bobOffset, localPos.z); // 1.7f is base eye height; adjust as needed
    }


    public void FocusAt(Transform target)
    {
        if (focusCoroutine != null) StopCoroutine(focusCoroutine); // Stop any previous coroutine
        focusCoroutine = StartCoroutine(FocusCoroutine(target));
    }

    public void StopFocus()
    {
        if (focusCoroutine != null) StopCoroutine(focusCoroutine);
        focusCoroutine = StartCoroutine(ResetFocusCoroutine());
    }

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

    private IEnumerator ResetFocusCoroutine()
    {
        Quaternion startRotation = transform.rotation;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f; // Adjust speed as needed
            transform.rotation = Quaternion.Slerp(startRotation, defaultRotation, t);
            yield return null;
        }

        lookAtTarget = null;
    }

    #region Debugging
    private void OnDrawGizmosSelected()
    {
        DebugDrawer drawer = new DebugDrawer();

        drawer.DrawRaySphereCheck(transform.position, transform.forward, reachDistance, 0.03f, canInteract ? Color.green : Color.red); ;


    }
    #endregion
}
