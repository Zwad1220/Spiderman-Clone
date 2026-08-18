using UnityEngine;

public class SwingMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public Rigidbody rb;
    PlayerControls controls;
    private OtherPlayerMovement pm;

    [Header("Swinging")]
    public float maxSwingDistance = 25f, detectionRadius = 25;
    private Vector3 swingPoint;
    private SpringJoint joint;

    [Header("Prediction")]
    public RaycastHit predictionHit;
    public float predictionSphereCastRadius = 2f;
    public Transform predictionPoint;

    [Header("Swing Forces")]
    public float horizontalThrustForce = 10f;
    public float forwardThrustForce = 8f;
    public float extendCableSpeed = 20f;

    // Events for UI communication
    public System.Action OnSwingStarted;
    public System.Action OnSwingStopped;

    // Public properties for UI
    public Vector3 CurrentSwingPoint => swingPoint;
    public bool IsGrappling => pm.activeGrapple;
    public Vector3 PlayerPosition => transform.position;

    private void Awake()
    {
        controls = new PlayerControls();
        pm = GetComponent<OtherPlayerMovement>();
        controls.Player.Jump.performed += ctx => { };
    }

    void Update()
    {
        CheckForSwingPoint();
        if (Input.GetKeyDown(KeyCode.Mouse0)) StartSwing();
        if (Input.GetKeyUp(KeyCode.Mouse0)) StopSwing();
        if (pm.activeGrapple) OdmGearMovement();
    }

    void LateUpdate()
    {
        DrawRope();
    }

    private void CheckForSwingPoint()
    {
        if (pm.activeGrapple) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, whatIsGrappleable);

        if (hits.Length == 0)
        {
            predictionPoint.gameObject.SetActive(false);
            predictionHit = default;
            swingPoint = Vector3.zero;
            return;
        }

        Collider closest = null;
        float closestDist = Mathf.Infinity;
        Vector3 closestPoint = Vector3.zero;

        foreach (Collider col in hits)
        {
            Vector3 point = col.ClosestPoint(transform.position);
            float dist = Vector3.Distance(transform.position, point);

            if (dist < closestDist)
            {
                closestDist = dist;
                closest = col;
                closestPoint = point;
            }
        }

        if (closest != null)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = closestPoint;
            swingPoint = closestPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
            swingPoint = Vector3.zero;
        }
    }

    private void StartSwing()
    {
        if (swingPoint == Vector3.zero) return;

        pm.activeGrapple = true;
        Debug.Log("Swinging");

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentSwingPosition = gunTip.position;

        OnSwingStarted?.Invoke();
    }

    private void StopSwing()
    {
        Debug.Log("Stopping");
        pm.activeGrapple = false;
        lr.positionCount = 0;
        Destroy(joint);
        
        OnSwingStopped?.Invoke();
    }

    private Vector3 currentSwingPosition;

    private void OdmGearMovement()
    {
        if (Input.GetKey(KeyCode.Space) && joint.maxDistance > 0)
        {
            joint.maxDistance -= extendCableSpeed * Time.deltaTime;
            joint.minDistance -= extendCableSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) && joint.maxDistance < maxSwingDistance)
        {
            joint.maxDistance += extendCableSpeed * Time.deltaTime;
            joint.minDistance += extendCableSpeed * Time.deltaTime;
        }
    }

    private void DrawRope()
    {
        if (!joint) return;

        currentSwingPosition = Vector3.Lerp(currentSwingPosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }
}