using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
    [SerializeField] private Camera mainCam;
    [Space]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform head;
    [Space]
    public Vector2 mouseSensitivity = new Vector2(0.15f, 0.15f);

    private Vector2 rotationAxis;

    private float bobTime = 0.0f;
    private float currentTilt = 0.0f;
    private Vector3 originalHeadPosition;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input = GetComponentInParent<PlayerInput>();
        originalHeadPosition = head.localPosition;
    }

    private void Update()
    {
        rotationAxis.y += GetMouseDelta().x;
        rotationAxis.x -= GetMouseDelta().y;

        rotationAxis.x = Mathf.Clamp(rotationAxis.x, -90, 90);

        transform.rotation = Quaternion.Euler(rotationAxis.x, rotationAxis.y, 0);
        orientation.rotation = Quaternion.Euler(0, rotationAxis.y, 0);
    }

    #region Methods
    public Vector2 GetMouseDelta()
    {
        Vector2 dir = input.actions["look"].ReadValue<Vector2>();
        return dir * mouseSensitivity;
    }

    public void ZoomCam(float amount, float lerp = 32f)
    {
        mainCam.fieldOfView = Mathf.MoveTowards(mainCam.fieldOfView, amount, lerp * Time.deltaTime);
    }

    public void TiltCam(float angle, float lerp = 2f)
    {
        currentTilt = Mathf.MoveTowards(currentTilt, angle, lerp * Time.deltaTime);

        head.localRotation = Quaternion.Euler(0, 0, currentTilt);
    }

    public void BobCam(Vector2 intensity, float speed)
    {
        if (intensity.magnitude > 0 && speed > 0)
        {
            bobTime += Time.deltaTime * speed;
            head.localPosition = originalHeadPosition + new Vector3(Mathf.Sin(bobTime / 2) * intensity.x, Mathf.Sin(bobTime) * intensity.y, 0);
        }
        else
        {
            bobTime = 0.0f;
            head.localPosition = Vector3.MoveTowards(head.localPosition, originalHeadPosition, Time.deltaTime * 5);
        }
    }
    #endregion
}
