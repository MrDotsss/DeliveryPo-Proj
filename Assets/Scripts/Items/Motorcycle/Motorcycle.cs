using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Motorcycle class controls a driveable motorcycle and allows interaction.
/// Implements the IInteractable interface for player interaction.
/// </summary>
public class Motorcycle : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform playerPos; // Position where the player sits on the motorcycle
    [SerializeField] private FPSCamera cam; // Reference to the FPS camera for driving view
    [SerializeField] private Rigidbody rb; // Rigidbody for physics-based movement
    [SerializeField] private Transform model; // Motorcycle model for visual adjustments

    [Header("Movement Settings")]
    public float acceleration = 10f; // Acceleration force applied to movement
    public float maxSpeed = 30f; // Maximum forward speed
    public float reverseSpeed = 5f; // Maximum reverse speed
    public float brakeForce = 30f; // Force applied when braking
    public float steerSpeed = 3f; // Steering responsiveness

    [Header("Orientation")]
    public float uprightSpeed = 5f; // Speed at which the motorcycle corrects its tilt
    public float maxTiltAngle = 45f; // Maximum allowable tilt before upright correction

    [Header("Leaning")]
    public float leanAngle = 30f; // Maximum angle for leaning during turns
    public float leanSpeed = 5f; // Speed at which leaning transitions occur

    private bool isDriving = false; // Determines whether the player is driving
    private float inputForward; // Forward movement input
    private float inputTurn; // Turning input
    private bool isBraking; // Whether the brake is pressed
    private bool isOnFloor = false; // Checks if the motorcycle is grounded

    private Player player; // Reference to the player object
    private float currentSpeed = 0; // Stores current movement speed

    /// <summary>
    /// Interaction text displayed when near the motorcycle.
    /// </summary>
    public string InteractionText => isDriving ? "" : "Press 'F' to Mount";

    /// <summary>
    /// Initializes the motorcycle physics.
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Adjust center of mass for stability
    }

    /// <summary>
    /// Handles interaction with the motorcycle.
    /// Allows the player to enter and drive.
    /// </summary>
    public void Interact()
    {
        if (player == null) player = GameManager.Instance.GetPlayer();

        if (!isDriving)
        {
            // Position the player on the motorcycle and disable player input
            player.transform.position = playerPos.position;
            player.transform.rotation = playerPos.rotation;
            player.canInput = false;
            player.gameObject.SetActive(false);
            cam.gameObject.SetActive(true);

            isDriving = true;

            // Subscribe to exit vehicle input
            InputManager.Instance.Interact += ExitVehicle;
        }
    }

    /// <summary>
    /// Allows the player to exit the motorcycle.
    /// </summary>
    public void ExitVehicle()
    {
        if (!isDriving) return;

        // Reset player position and re-enable input
        player.transform.position = playerPos.position + (transform.right * 2);
        player.transform.rotation = Quaternion.identity;
        player.transform.SetParent(null);

        isDriving = false;
        player.canInput = true;
        player.gameObject.SetActive(true);
        cam.gameObject.SetActive(false);

        // Unsubscribe from exit input
        InputManager.Instance.Interact -= ExitVehicle;
    }

    /// <summary>
    /// Handles input updates while driving.
    /// </summary>
    void Update()
    {
        if (!isDriving) return;

        // Retrieve input values
        Vector2 inputDir = InputManager.Instance.Move;
        inputForward = inputDir.y;
        inputTurn = inputDir.x;
        isBraking = InputManager.Instance.jumpAction.IsPressed();

        // Move the camera with vehicle motion
        cam.MoveCamera(110);

        // Calculate the current movement speed
        currentSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
    }

    /// <summary>
    /// Handles physics-based movement in FixedUpdate.
    /// </summary>
    void FixedUpdate()
    {
        if (isOnFloor)
        {
            ApplyMovement();
        }

        ApplyUprightAndSteering();
        ApplyLeaning();
    }

    /// <summary>
    /// Applies movement forces based on player input.
    /// </summary>
    void ApplyMovement()
    {
        if (isBraking || !isDriving)
        {
            // Apply braking force when stopping
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(0, rb.velocity.y, 0), brakeForce * Time.fixedDeltaTime);
            return;
        }

        // Determine target speed based on forward or reverse movement
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

    /// <summary>
    /// Ensures the motorcycle remains upright and responds to steering.
    /// </summary>
    void ApplyUprightAndSteering()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 20f))
        {
            isOnFloor = true;

            Vector3 groundNormal = hit.normal;

            // Apply steering adjustments
            if (Mathf.Abs(inputTurn) > 0.1f)
            {
                Quaternion steerRotation = Quaternion.Euler(0f, inputTurn * steerSpeed, 0f);
                rb.MoveRotation(rb.rotation * steerRotation);
            }

            // Check tilt angle to determine upright correction
            float tiltAngle = Vector3.Angle(transform.up, groundNormal);
            if (tiltAngle > maxTiltAngle)
            {
                Quaternion uprightRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, groundNormal), groundNormal);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, uprightRotation, uprightSpeed * Time.fixedDeltaTime));
            }
        }
        else isOnFloor = false;
    }

    /// <summary>
    /// Applies leaning effects based on turning input.
    /// </summary>
    void ApplyLeaning()
    {
        if (!isDriving) return;

        if (model != null)
        {
            float targetLeanAngle = -inputTurn * leanAngle;
            Quaternion targetLeanRotation = Quaternion.Euler(model.localEulerAngles.x, model.localEulerAngles.y, targetLeanAngle);
            model.localRotation = Quaternion.Slerp(model.localRotation, targetLeanRotation, leanSpeed * Time.fixedDeltaTime);

            // Adjust player positioning relative to the lean
            Vector3 currentEuler = playerPos.transform.localEulerAngles;
            currentEuler.z = Mathf.LerpAngle(NormalizeAngle(currentEuler.z), targetLeanAngle / 2, leanSpeed * Time.fixedDeltaTime);
            playerPos.transform.localEulerAngles = currentEuler;
        }
    }

    /// <summary>
    /// Normalizes an angle for consistent calculations.
    /// </summary>
    float NormalizeAngle(float angle)
    {
        return angle > 180 ? angle - 360 : angle;
    }
}
