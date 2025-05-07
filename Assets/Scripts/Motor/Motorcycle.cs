using UnityEngine;

public class Motorcycle : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform playerPos;
    [SerializeField] private Transform phonePos;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform model;

    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 30f;
    public float reverseSpeed = 5f;
    public float brakeForce = 30f;
    public float steerSpeed = 3f;

    [Header("Orientation")]
    public float uprightSpeed = 5f;
    public float maxTiltAngle = 45f;

    [Header("Leaning")]
    public float leanAngle = 30f;
    public float leanSpeed = 5f;

    private bool isDriving = false;
    private float inputForward;
    private float inputTurn;
    private bool isBraking;
    private bool isOnFloor = false;

    private Player player;
    private Transform phoneParent;
    private float currentSpeed = 0;

    public void Interact()
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (!isDriving)
        {
            player.transform.position = playerPos.position;
            player.transform.rotation = playerPos.rotation;
            player.phone.canInput = false;
            player.phone.OpenPhone();
            player.phone.ScreenButtonPressed("home");
            player.SetController(false);
            player.transform.SetParent(playerPos);

            phoneParent = player.phone.transform.parent;
            player.phone.transform.SetParent(phonePos);
            player.phone.transform.position = phonePos.position;
            player.phone.transform.rotation = phonePos.rotation;
            player.phone.transform.localScale = Vector3.one;

            isDriving = true;
        }
    }

    public void ExitVehicle()
    {
        player.transform.position = playerPos.position + (transform.right * 2);
        player.transform.rotation = Quaternion.identity;
        player.SetController(true);
        player.phone.canInput = true;
        player.transform.SetParent(null);

        player.phone.transform.SetParent(phoneParent);
        player.phone.transform.position = phoneParent.position;
        player.phone.transform.rotation = phoneParent.rotation;
        player.phone.transform.localScale = Vector3.one;

        isDriving = false;
        player.canInput = true;
    }

    public string GetInteractionText()
    {
        if (!isDriving)
        {
            return "Press 'F' to Mount";
        }
        else return "";
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    void Update()
    {
        if (!isDriving) return;

        if (InputManager.Instance.input.actions["interact"].WasPressedThisFrame())
        {
            ExitVehicle();
            return;
        }



        player.canInput = false;

        Vector2 inputDir = InputManager.Instance.input.actions["move"].ReadValue<Vector2>();
        inputForward = inputDir.y;
        inputTurn = inputDir.x;
        isBraking = InputManager.Instance.input.actions["jump"].IsPressed();

        if (InputManager.Instance.input.actions["mouse-lock"].IsPressed() || PlayerInventory.Instance.inventoryMode || InspectionManager.Instance.isInspecting)
        {
            player.cam.canInput = false;
            InputManager.Instance.SetCursor(false, false);
        }
        else
        {
            player.cam.canInput = true;
            InputManager.Instance.SetCursor(true, true);
        }


        player.cam.DoLook(player.cam.GetMouseDelta(), 120);

        currentSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
    }

    void FixedUpdate()
    {
        if (DialogueManager.Instance.IsDialogueActive) return;

        if (isOnFloor)
        {
            ApplyMovement();
        }

        ApplyUprightAndSteering();
        ApplyLeaning();

    }

    void ApplyMovement()
    {

        if (isBraking || DialogueManager.Instance.IsDialogueActive || !isDriving)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(0, rb.velocity.y, 0), brakeForce * Time.fixedDeltaTime);
            return;
        }

        float desiredSpeed = inputForward > 0 ? maxSpeed : -reverseSpeed;

        if (inputForward != 0)
        {

            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * desiredSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(0, rb.velocity.y, 0), acceleration * Time.fixedDeltaTime);
        }

    }
    void ApplyUprightAndSteering()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f))
        {
            isOnFloor = true;

            Vector3 groundNormal = hit.normal;

            // Apply steering (yaw rotation)
            if (Mathf.Abs(inputTurn) > 0.1f)
            {
                Quaternion steerRotation = Quaternion.Euler(0f, inputTurn * steerSpeed, 0f);
                rb.MoveRotation(rb.rotation * steerRotation);
            }

            // Check tilt
            float tiltAngle = Vector3.Angle(transform.up, groundNormal);

            if (tiltAngle > maxTiltAngle)
            {
                // Apply upright correction if tilted too much
                Quaternion uprightRotation = Quaternion.LookRotation(
                    Vector3.ProjectOnPlane(transform.forward, groundNormal),
                    groundNormal
                );

                rb.MoveRotation(Quaternion.Slerp(rb.rotation, uprightRotation, uprightSpeed * Time.fixedDeltaTime));
            }


        }
        else isOnFloor = false;
    }

    void ApplyLeaning()
    {
        if (!isDriving) return;

        if (model != null)
        {
            float targetLeanAngle = -inputTurn * leanAngle;
            Quaternion targetLeanRotation = Quaternion.Euler(model.localEulerAngles.x, model.localEulerAngles.y, targetLeanAngle);
            model.localRotation = Quaternion.Slerp(model.localRotation, targetLeanRotation, leanSpeed * Time.fixedDeltaTime);

            Vector3 currentEuler = playerPos.transform.localEulerAngles;
            currentEuler.z = Mathf.LerpAngle(NormalizeAngle(currentEuler.z), targetLeanAngle / 2, leanSpeed * Time.fixedDeltaTime);
            playerPos.transform.localEulerAngles = currentEuler;
        }
    }

    float NormalizeAngle(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }
}
