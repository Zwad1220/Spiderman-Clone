using UnityEngine;

public class SwingMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip; // point the rope visually comes from
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public Rigidbody rb;
    PlayerControls controls;
    private PlayerMovement pm;

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

    //private bool activeGrapple = false;

    private void Awake()
    {
        controls = new PlayerControls();
        pm = GetComponent<PlayerMovement>();
        //controls.Player.Shoot.performed += ctx => {
        //    StartSwing();
        //};
        //controls.Player.Shoot.canceled += ctx =>
        //{
        //    StopSwing();
        //};
        controls.Player.Jump.performed += ctx =>
        {
            
        };
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
            return;
        }

        // find the closest valid point among candidates
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

            predictionHit = new RaycastHit(); // kept for compatibility with StartSwing() below
            swingPoint = closestPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
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
    }

    private void StopSwing()
    {
        Debug.Log("Stopping");
        pm.activeGrapple = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentSwingPosition;

    private void OdmGearMovement()
    {
        // manual air control while swinging — pulls you toward look direction / input
        if (pm.move.y > 0)
            rb.AddForce(cam.forward * forwardThrustForce * Time.deltaTime);

        if (pm.move.x > 0)
            rb.AddForce(cam.right * horizontalThrustForce * Time.deltaTime);
        if (pm.move.x < 0)
            rb.AddForce(-cam.right * horizontalThrustForce * Time.deltaTime);

        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 directionToPoint = swingPoint - transform.position;
            rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;
        }
        // extend cable
        if (Input.GetKey(KeyCode.S))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

            joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            joint.minDistance = extendedDistanceFromPoint * 0.25f;
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
