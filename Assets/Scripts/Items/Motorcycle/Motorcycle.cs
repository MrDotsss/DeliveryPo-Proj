using Unity.VisualScripting;
using UnityEngine;

public class Motorcycle : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private Transform playerPos;
    [SerializeField] private FPSCamera cam;
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
    private float currentSpeed = 0;

    public string InteractionText => isDriving ? "Press 'F' to Mount" : "";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    public void Interact()
    {
        if (player == null) player = GameManager.Instance.GetPlayer();

        if (!isDriving)
        {
            player.transform.position = playerPos.position;
            player.transform.rotation = playerPos.rotation;
            player.canInput = false;
            player.gameObject.SetActive(false);
            cam.gameObject.SetActive(true);

            isDriving = true;

            InputManager.Instance.Interact += ExitVehicle;
        }
    }

    public void ExitVehicle()
    {
        if (!isDriving) return;

        player.transform.position = playerPos.position + (transform.right * 2);
        player.transform.rotation = Quaternion.identity;
        player.transform.SetParent(null);

        isDriving = false;
        player.canInput = true;
        player.gameObject.SetActive(true);
        cam.gameObject.SetActive(false);

        InputManager.Instance.Interact -= ExitVehicle;
    }

    void Update()
    {
        if (!isDriving) return;

        Vector2 inputDir = InputManager.Instance.Move;
        inputForward = inputDir.y;
        inputTurn = inputDir.x;
        isBraking = InputManager.Instance.jumpAction.IsPressed();

        cam.MoveCamera(110);

        currentSpeed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
    }

    void FixedUpdate()
    {
        if (isOnFloor)
        {
            ApplyMovement();
        }

        ApplyUprightAndSteering();
        ApplyLeaning();

    }

    void ApplyMovement()
    {

        if (isBraking || !isDriving)
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
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 20f))
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