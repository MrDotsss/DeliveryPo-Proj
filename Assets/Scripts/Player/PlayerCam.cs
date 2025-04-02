using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [Space]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform head;

    private Vector2 rotationAxis;
    private float bobTime = 0.0f;
    private float currentTilt = 0.0f;
    private Vector3 originalHeadPosition;

    private Transform focusTarget = null; // Target to focus on
    private Quaternion originalRotation;
    private Coroutine focusCoroutine;

    public bool canInput = true;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        originalHeadPosition = head.localPosition;
    }

    public void DoLook(Vector2 mouseDelta)
    {
        if (focusTarget != null || DialogueManager.Instance.IsDialogueActive || !canInput) return;

        rotationAxis.y += mouseDelta.x;
        rotationAxis.x -= mouseDelta.y;
        rotationAxis.x = Mathf.Clamp(rotationAxis.x, -90, 90);

        transform.rotation = Quaternion.Euler(rotationAxis.x, rotationAxis.y, 0);
        orientation.rotation = Quaternion.Euler(0, rotationAxis.y, 0);
    }

    #region Methods
    public Vector2 GetMouseDelta()
    {
        Vector2 sensitivity = new Vector2(GameSetting.Instance.mouseXSensitivity, GameSetting.Instance.mouseYSensitivity);
        Vector2 dir = InputManager.Instance.input.actions["look"].ReadValue<Vector2>();
        return dir * sensitivity;
    }

    public void ZoomCam(float amount, float lerp = 32f)
    {
        cam.fieldOfView = Mathf.MoveTowards(cam.fieldOfView, amount, lerp * Time.deltaTime);
    }

    public void TiltCam(float angle, float lerp = 4f)
    {
        currentTilt = Mathf.MoveTowards(currentTilt, angle, lerp * Time.deltaTime);
        head.localRotation = Quaternion.Euler(0, 0, currentTilt);
    }

    public void BobCam(Vector2 intensity, float speed)
    {
        if (intensity.magnitude != 0 && speed != 0)
        {
            bobTime += Time.deltaTime * speed;
            head.localPosition = originalHeadPosition + new Vector3(Mathf.Sin(bobTime / 2) * intensity.x, Mathf.Sin(bobTime) * intensity.y, 0);
        }
        else
        {
            bobTime = 0.0f;
            head.localPosition = Vector3.MoveTowards(head.localPosition, originalHeadPosition, Time.deltaTime * speed);
        }
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
        focusTarget = target;
        originalRotation = transform.rotation;

        while (focusTarget != null)
        {
            Vector3 direction = (focusTarget.position - transform.position).normalized;
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
            transform.rotation = Quaternion.Slerp(startRotation, originalRotation, t);
            yield return null;
        }

        focusTarget = null;
    }
    #endregion
}
