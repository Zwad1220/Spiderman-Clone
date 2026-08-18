
using Unity.VisualScripting;
using UnityEngine;

public class OtherPlayerMovement : MonoBehaviour
{
    public Transform orientation;

    PlayerControls controls;
    public Vector2 move;
    Rigidbody rb;
    public bool grounded;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public LayerMask wallMask;
    public float groundDrag;

    public bool freeze;
    public bool activeGrapple;
    public bool glideHeld;  
    public bool viableWallCrawling => touchingWall && wallHeld ;
    bool touchingWall, wallHeld;


    IMovementStrategy walkStrategy;
    IMovementStrategy glideStrategy;
    IMovementStrategy currentStrategy;
    IMovementStrategy wallCrawlStrategy;

    Vector3 velocityToSet;
    Vector3 wallNormal;

    [Header("Movement Data")]
    [SerializeField] GlideMovementDataSO glideData;
    [SerializeField] WallCrawlMovementDataSO wallCrawlData;



    void Awake(){
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        groundMask = LayerMask.GetMask("Ground", "Environment");

        walkStrategy = new WalkMovement{groundDrag = groundDrag};
        glideStrategy = new GlideMovement(glideData);
        wallCrawlStrategy = new VerticalMovementStrategy(wallCrawlData);
        currentStrategy = walkStrategy;

        controls.Player.Move.performed += ctx => {
            //if (!freeze && !activeGrapple)
                move = ctx.ReadValue<Vector2>();
        };

        controls.Player.Move.canceled += ctx => move = Vector2.zero;
        controls.Player.Glide.performed += ctx => glideHeld = true;
        controls.Player.Glide.canceled += ctx => glideHeld = false;
        controls.Player.Jump.performed += ctx => wallHeld = true;
        controls.Player.Jump.canceled += ctx => wallHeld = false;

    }

        
    public void SetMovementStrategy(IMovementStrategy newstrategy) =>currentStrategy = newstrategy;
    void OnEnable()=> controls.Player.Enable();
    void OnDisable()=> controls.Player.Disable();

Vector3 cachedInputDir;

    void Update()
    {
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //TODO: Add case for when wall held

        Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;
        cachedInputDir = forward * move.y + right * move.x;

        Debug.Log($"grounded:{grounded} activeGrapple:{activeGrapple} touchingWall:{touchingWall} wallHeld:{wallHeld}");

        if (activeGrapple) currentStrategy = walkStrategy;
        else if (viableWallCrawling) currentStrategy = wallCrawlStrategy;
        else if (grounded) currentStrategy = walkStrategy;
        else if (glideHeld) currentStrategy = glideStrategy;

        var ctx = new MovementContext
        {
            Rb = rb,
            InputDirection = cachedInputDir,
            DeltaTime = Time.fixedDeltaTime,
            Grounded = grounded,
            TouchingWall = touchingWall,
            WallNormal = wallNormal
        };

        currentStrategy.Move(ctx);
    }

    void SetVelocity() => rb.linearVelocity = velocityToSet;

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }



    void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & wallMask) == 0) return;

        touchingWall = true;
        wallNormal = collision.GetContact(0).normal;
        Debug.Log($"Touching Wall: {touchingWall} ");
    }

    void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & wallMask) == 0) return;
        touchingWall = false;
        Debug.Log($"Touching Wall: {touchingWall} ");
    }

}
