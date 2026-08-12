
using UnityEngine;

public class OtherPlayerMovement : MonoBehaviour
{
    public Transform orientation;

    PlayerControls controls;
    Vector2 move;
    Rigidbody rb;
    public bool grounded;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;
    public float groundDrag;

    public bool freeze;
    public bool activeGrapple;
    public bool glideHeld;  

    IMovementStrategy walkStrategy;
    IMovementStrategy glideStrategy;
    IMovementStrategy currentStrategy;

    Vector3 velocityToSet;

    [Header("Movement Data")]
    [SerializeField] GlideMovementDataSO glideData;


    void Awake(){
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        groundMask = LayerMask.GetMask("Ground", "Environment");

        walkStrategy = new WalkMovement{groundDrag = groundDrag};
        glideStrategy = new GlideMovement(glideData);
        currentStrategy = walkStrategy;

        controls.Player.Move.performed += ctx => {
            if (!freeze && !activeGrapple) move = ctx.ReadValue<Vector2>();
        };

        controls.Player.Move.canceled += ctx => move = Vector2.zero;
        controls.Player.Glide.performed += ctx => glideHeld = true;
        controls.Player.Glide.canceled += ctx => glideHeld = false;
    }

        
    public void SetMovementStrategy(IMovementStrategy newstrategy) =>currentStrategy = newstrategy;
    void OnEnable()=> controls.Player.Enable();
    void OnDisable()=> controls.Player.Disable();

Vector3 cachedInputDir;

    void Update()
    {
        //ToDO: Decide if it will be glide input to determine whether the player is gliding
        //ToDO: Might do state machine logic for the movement strategy basically transition with its condition
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;
        cachedInputDir = forward * move.y + right * move.x;

        if (grounded) currentStrategy = walkStrategy;
        else if (glideHeld) currentStrategy = glideStrategy;

        if (freeze || activeGrapple) return;

        var ctx = new MovementContext
        {
            Rb = rb,
            InputDirection = cachedInputDir,
            DeltaTime = Time.fixedDeltaTime,
            Grounded = grounded
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

}
