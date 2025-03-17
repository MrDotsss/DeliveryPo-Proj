using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//the owner class or ito yung humahawak ng lahat ng actions, variables, properties ng entities

//as much as possible keep special components private
//iwas modding, hacking, cheats or colliding with other script entities with same namespace

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    public PlayerCam cam;
    [Space]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform neck;
    [Space]
    [Header("Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [Space]
    //setting all properties to be public para madali makuha at maset sa states
    [Header("Body")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private float wallCheckDistance = 0.7f;
    [SerializeField] private float maxSlopeAngle = 45f;

    [Header("Stats")]
    public float maxStamina = 50f;
    public float staminaRate = 8f;
    public float currentStamina { private set; get; }

    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 8f;
    public float crouchSpeed = 1.5f;
    public float acceleration = 5f;
    public float friction = 8f;
    public float airFriction = 1.5f;
    [Space]
    [Header("Mobility")]
    public float jumpStrength = 15f;


    // RaycastHits
    //storing all raycast hits
    private RaycastHit ceilingHit;
    private RaycastHit groundHit;
    //player velocity
    private Vector3 velocity;


    //this checkers are private set and public getters
    //dito malalaman if nakadikit ulo sa ceiling or nasa ground etc.
    //refer to update method para sa checking
    public bool IsOnFrontWall { private set; get; }
    public bool IsOnFloor { private set; get; }
    public bool IsOnCeiling { private set; get; }
    public bool IsOnLedge { private set; get; }
    public bool IsOnSlope { private set; get; }

    public bool canInput = true;


    private void InitializeFields()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        InitializeFields();

        currentStamina = maxStamina;
    }

    private void Update()
    {
        IsOnFloor = Physics.Raycast(transform.position, Vector3.down, out groundHit, playerHeight / 2 + groundCheckDistance, groundMask) || controller.isGrounded;
        IsOnCeiling = Physics.SphereCast(transform.position, 0.6f, Vector3.up, out ceilingHit, playerHeight / 2 + groundCheckDistance, groundMask)
          || Physics.Raycast(transform.position, Vector3.up, playerHeight / 2 + groundCheckDistance, groundMask);

        IsOnFrontWall = Physics.Raycast(transform.position, orientation.forward, wallCheckDistance, wallMask);

        IsOnLedge = !Physics.Raycast(transform.position + (Vector3.up * (playerHeight / 2 + groundCheckDistance)), orientation.forward, wallCheckDistance, wallMask) && IsOnFrontWall;

        if (IsOnFloor)
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);
            IsOnSlope = angle < maxSlopeAngle && angle != 0;
        }

        if (canInput)
            controller.Move(velocity * Time.deltaTime);

    }

    #region Setters and Getters

    #region Setters
    //ito naman for components if may seset specific sa component while staying private
    //limiting sa control ng components (iwas confusion and such)
    public void UseGravity(bool gravity = true)
    {
        if (gravity)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
    }

    public void DefineVelocity(float x, float y, float z)
    {
        velocity = new Vector3(x, y, z);
    }

    public void RateStamina(float rate)
    {
        currentStamina += rate;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }
    #endregion

    #region Getters
    public Vector2 GetInputDir()
    {
        //yan yung syntax ng bagong input system
        //i'll discuss it nalang since may hiwalay na ui kasi jan
        Vector2 dir = InputManager.Instance.input.actions["move"].ReadValue<Vector2>();
        return dir.normalized;
    }

    public Vector3 GetMoveDirection()
    {
        //so para mahanap yung forward, left, right, and backward direction ni player na relative sa camera
        //need natin ng move direction na nakadepends sa orientation ng player at sa input 
        Vector3 dir = orientation.forward * GetInputDir().y + orientation.right * GetInputDir().x;
        //then we normalize it para maging 0-1 yung value
        //i'll explain it further
        return dir.normalized;
    }

    public Vector3 GetSlopeDirection()
    {
        //if nasa slope naman si player at para di magcontradict sa gravity at di dumulas si player
        Vector3 dir = orientation.forward * GetInputDir().y + orientation.right * GetInputDir().x;

        //we add force (yes not define new velocity kasi kelangan consistent padin si velocity)
        //we force it down para dumikit si player sa slope at hindi sya floaty
        //velocity = velocity + (Vector3.down * 5f);

        //then we project the angle of direction sa plane (face ng mesh) na inaapakan ni player
        return Vector3.ProjectOnPlane(dir, groundHit.normal).normalized;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
    #endregion

    #endregion


    #region Actions
    //naming convention should be: Do+(actionName)

    //move has these parameters
    //direction = DIREKSYON ng player kung saan papunta
    //speed = for maximum speed ng movement
    //amount = how much interpolation relative to frametime (di ko masyado gets pero ito yung how smooth it is)
    public void DoMove(Vector3 direction, float speed, float amount)
    {
        velocity = Vector3.Lerp(velocity, new Vector3(direction.x * speed, velocity.y, direction.z * speed), amount * Time.deltaTime);
    }

    public void DoJump(float force)
    {
        velocity = new Vector3(velocity.x, force, velocity.z);
    }


    public void DoVault(float force)
    {
        //vaulting naman is if nasa legde si player at abot nya yung talon magjajump ulit by force para makaakyat si player
        //refer to air state for vaulting
        velocity = Vector3.up * force;
    }

    public void DoCrouch(bool crouch = true, float lerpTime = 8f)
    {
        //we use 2 colliders instead of scaling the player
        //since may mga child kasi na pwedeng maapektuhan sa scale 2 colliders is the solution
        //if true ang crouch we turn off the standing collider at ang crouch collider naman ang active
        controller.height = crouch ? crouchHeight : playerHeight;
        //then para di parin mukhang nakatayo si player
        if (crouch)
        {
            neck.localPosition = Vector3.zero;
        }
        else
        {
            neck.localPosition = Vector3.up * 0.5f;
        }
    }
    #endregion
}
