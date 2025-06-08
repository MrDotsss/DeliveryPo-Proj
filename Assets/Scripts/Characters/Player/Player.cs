using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Player : Character
{
    [Header("References")]
    public Transform orientation;
    public FPSCamera cam;
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerHand Hand { get; private set; }

    [Header("Mobility")]
    public float walkSpeed = 10.0f;
    public float runSpeed = 15.0f;
    public float crouchSpeed = 5.0f;
    public float crouchHeightFactor = 0.5f;
    public float jumpStrength = 10.0f;

    public float acceleration = 3.0f;
    public float friction = 25.0f;

    private Coroutine crouchCoroutine;

    public List<Sprite> imageSprites = new List<Sprite>();

    private void Start()
    {
        originalHeight = controller.height;
        cam = GetComponentInChildren<FPSCamera>();
        Hand = GetComponentInChildren<PlayerHand>();
        StateMachine = gameObject.GetComponent<PlayerStateMachine>();

        InputManager.Instance.Inventory += ToggleInventory;
        InputManager.Instance.Phone += TogglePhone;

        UIManager.Instance.SwitchPanel(UIManager.EUIPanels.CursorPanel);

        transform.position = SaveNLoadManager.Instance.LoadPlayer();
    }

    private void OnDestroy()
    {
        InputManager.Instance.Inventory -= ToggleInventory;
    }

    private void ToggleInventory()
    {
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Inventory);
    }

    private void TogglePhone()
    {
        UIManager.Instance.ToggleCurrentPanel(UIManager.EUIPanels.Phone);
    }

    private void Update()
    {
        PerformCheckers();

        cam.MoveCamera();

        float speed = new Vector2(velocity.x, velocity.z).magnitude;
        float tiltInput = InputManager.Instance.Move.x;
        if (speed > 0.5f) cam.BobCamera(true, speed / 2, 0.08f);
        else cam.BobCamera(true, 1.5f, 0.05f);

        cam.TiltCamera(-tiltInput * 3f, 10f);
    }

    public Vector3 GetMoveDirection()
    {
        Vector2 inputDir = InputManager.Instance.Move;

        Vector3 dir = orientation.forward * inputDir.y + orientation.right * inputDir.x;

        return dir.normalized;
    }

    public void DoJump(float strength)
    {
        velocity.y = strength;
    }

    public void DoCrouch(bool isCrouch)
    {
        if (crouchCoroutine != null)
        {
            StopCoroutine(crouchCoroutine);
        }

        crouchCoroutine = StartCoroutine(Crouching(isCrouch));
    }

    IEnumerator Crouching(bool crouch)
    {
        float crouchHeight = originalHeight * crouchHeightFactor; // Set this to something like 0.5f in your class

        float startHeight = controller.height;
        float targetHeight = crouch ? crouchHeight : originalHeight;

        float startCenterY = controller.center.y;
        float targetCenterY = startCenterY + (targetHeight - startHeight) / 2f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * crouchSpeed;

            controller.height = Mathf.Lerp(startHeight, targetHeight, t);
            //controller.center = new Vector3(0, Mathf.Lerp(startCenterY, targetCenterY, t), 0);

            yield return null;
        }

        controller.height = targetHeight;
        //controller.center = new Vector3(0, targetCenterY, 0);
    }
}
