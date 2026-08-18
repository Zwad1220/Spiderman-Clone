using System.Collections.Generic;
using UnityEngine;

public class SwingMovement : MonoBehaviour, ISwingTargetProvider
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

    [Header("Targeting")]
    [Range(0f, 1f)] public float facingCosThreshold = 0.6f; // ~53 degrees
    public LayerMask occlusionMask; // set to whatever counts as "blocking geometry" (usually excludes the anchors themselves)

    // ---- ISwingTargetProvider ----
    private readonly List<SwingAnchorCandidate> visibleAnchors = new();
    public IReadOnlyList<SwingAnchorCandidate> VisibleAnchors => visibleAnchors;
    public Transform CurrentAnchor => visibleAnchors.Count > 0 ? visibleAnchors[0].Anchor : null;

    public event System.Action OnSwingStarted;

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

        visibleAnchors.Clear();

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, whatIsGrappleable);

        foreach (Collider col in hits)
        {
            Vector3 point = col.ClosestPoint(transform.position);

            // GATE 1: facing cone
            Vector3 toPoint = (point - cam.position).normalized;
            float facing = Vector3.Dot(cam.forward, toPoint);
            if (facing < facingCosThreshold) continue;

            // GATE 2: occlusion — nothing blocking between camera and point
            float distToPoint = Vector3.Distance(cam.position, point);
            if (Physics.Raycast(cam.position, toPoint, out RaycastHit occl, distToPoint, occlusionMask))
            {
                continue; // something is in the way before we even reach the point
            }

            visibleAnchors.Add(new SwingAnchorCandidate
            {
                Anchor = col.transform,
                Distance = Vector3.Distance(transform.position, point)
            });
        }

        visibleAnchors.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        if (visibleAnchors.Count > 0)
        {
            Transform closest = visibleAnchors[0].Anchor;
            Vector3 closestPoint = closest.GetComponent<Collider>().ClosestPoint(transform.position);

            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = closestPoint;
            predictionHit = new RaycastHit();
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