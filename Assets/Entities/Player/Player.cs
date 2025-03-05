using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//the owner class or ito yung humahawak ng lahat ng actions, variables, properties ng entities

//as much as possible keep special components private
//iwas modding, hacking, cheats or colliding with other script entities with same namespace

public class Player : MonoBehaviour
{
    //practice using regions iwas clogged code when finding something.
    #region Inspector Properties
    //references region ay yung mga kukunin na ibang components
    #region References
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider standCollider;
    [SerializeField] private SphereCollider crouchCollider;
    [Space]
    public PlayerInput input;
    public PlayerCam cam;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform neck;
    [Space]
    [Header("Masks")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [Space]
    #endregion
    #region Properties
    //setting all properties to be public para madali makuha at maset sa states
    [Header("Body")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float standPosition = 0.5f;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private float wallCheckDistance = 0.7f;
    [SerializeField] private float maxSlopeAngle = 45f;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 3f;
    public float acceleration = 8f;
    public float friction = 12f;
    public float airFriction = 5f;
    [Space]
    [Header("Mobility")]
    public float jumpStrength = 15f;
    #endregion
    #endregion

    #region Private Properties
    // RaycastHits
    //storing all raycast hits
    private RaycastHit ceilingHit;
    private RaycastHit groundHit;
    #endregion

    #region Public Properties
    //this checkers are private set and public getters
    //dito malalaman if nakadikit ulo sa ceiling or nasa ground etc.
    //refer to update method para sa checking
    public bool IsOnFrontWall { private set; get; }
    public bool IsOnFloor { private set; get; }
    public bool IsOnCeiling { private set; get; }
    public bool IsOnLedge { private set; get; }
    public bool IsOnSlope { private set; get; }

    #endregion

    private void InitializeFields()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; //para di matumba si player XD

        input = GetComponent<PlayerInput>();

        standCollider = GetComponent<CapsuleCollider>();
        crouchCollider = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        InitializeFields();
    }

    private void Update()
    {
        #region Checkers
        //using Physics.Raycast is what mostly uses sa FPS shooters at sa anyting na gustong makadetect in specific direction at point
        //transform.position = position ng raycast
        //Vector3.down = direction ng raycast (pababa yan from transform.position)
        //out floorHit = 'out' keyword is storing the data set from this method sa floorHit while returning boolean value
        //(playerHeight * 0.5f) + 0.3f = kalahati ni playerheight (which is yung nasa pinakababa nya or taas depends sa direction plus onti pa
        //para macompensate yung input delay ni player (para sa mga slow pumindot)
        //groundMask = anong layer lang ang idedetect ni raycast
        IsOnFloor = Physics.Raycast(transform.position, Vector3.down, out groundHit, playerHeight / 2 + groundCheckDistance, groundMask);
        IsOnCeiling = Physics.SphereCast(transform.position, 0.6f, Vector3.up, out ceilingHit, playerHeight / 2 + groundCheckDistance, groundMask)
          || Physics.Raycast(transform.position, Vector3.up, playerHeight / 2 + groundCheckDistance, groundMask);

        IsOnFrontWall = Physics.Raycast(transform.position, orientation.forward, wallCheckDistance, wallMask);

        IsOnLedge = !Physics.Raycast(transform.position + (Vector3.up * (playerHeight / 2 + groundCheckDistance)), orientation.forward, wallCheckDistance, wallMask) && IsOnFrontWall;

        if (IsOnFloor)
        {
            float angle = Vector3.Angle(Vector3.up, groundHit.normal);
            IsOnSlope = angle < maxSlopeAngle && angle != 0;
        }

        #endregion
    }

    #region Setters and Getters

    #region Setters
    //ito naman for components if may seset specific sa component while staying private
    //limiting sa control ng components (iwas confusion and such)
    public void UseGravity(bool gravity = true)
    {
        rb.useGravity = gravity;
    }

    public void DefineVelocity(Vector3 newVel)
    {
        rb.velocity = newVel;
    }
    #endregion

    #region Getters
    public Vector2 GetInputDir()
    {
        //yan yung syntax ng bagong input system
        //i'll discuss it nalang since may hiwalay na ui kasi jan
        Vector2 dir = input.actions["movement"].ReadValue<Vector2>();
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
        rb.AddForce(Vector3.down * 50f, ForceMode.Force);

        //then we project the angle of direction sa plane (face ng mesh) na inaapakan ni player
        return Vector3.ProjectOnPlane(dir, groundHit.normal).normalized;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
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
        rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(direction.x * speed, rb.velocity.y, direction.z * speed), amount * Time.fixedDeltaTime);
    }

    public void DoJump(float force)
    {
        rb.velocity = new Vector3(rb.velocity.x, force, rb.velocity.z);
    }

   
    public void DoVault(float force)
    {
        //vaulting naman is if nasa legde si player at abot nya yung talon magjajump ulit by force para makaakyat si player
        //refer to air state for vaulting
        rb.velocity = Vector3.up * force;
    }

    public void DoCrouch(bool crouch = true, float lerpTime = 8f)
    {
        //we use 2 colliders instead of scaling the player
        //since may mga child kasi na pwedeng maapektuhan sa scale 2 colliders is the solution
        //if true ang crouch we turn off the standing collider at ang crouch collider naman ang active
        standCollider.enabled = !crouch;
        crouchCollider.enabled = crouch;
        //then para di parin mukhang nakatayo si player
        //we move toward the neck position (which is kung nasaan si camera) to stand or up position
        if (crouch)
        {
            //move toward is to smoothly interpolate to the exact values
            //lerp make it precise for smoother easing but move toward is linear
            neck.localPosition = Vector3.MoveTowards(neck.localPosition, Vector3.zero, lerpTime * Time.deltaTime);
        } else
        {
            neck.localPosition = Vector3.up * standPosition;
        }
    }
    #endregion
}
