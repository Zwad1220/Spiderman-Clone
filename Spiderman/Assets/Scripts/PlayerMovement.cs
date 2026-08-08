using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;


public class PlayerMovement : MonoBehaviour
{
    public Transform orientation;

    PlayerControls controls;
    Vector2 move;
    private Rigidbody rb;
    public float playerHeight;
    public float groundDrag;
    public bool grounded;
    public CharacterController controller;

    public float gravity;
    public float speed;

    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    Vector3 velocity;

    public bool freeze;
    public bool activeGrapple;
    public bool swinging;
    public bool usingShooter;
    public bool easy;

    private Vector3 velocityToSet;
    //private Grappling grappling;
    //private Shooting shooting;
    //private Swinging swing;


    public bool tutOver;


    private void Awake()
    {
        tutOver = false;
        usingShooter = false;

        //swing = GetComponent<Swinging>();
        //grappling = GetComponent<Grappling>();
        //shooting = GetComponent<Shooting>();
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => {
            if (!freeze && !activeGrapple)
            {
                move = ctx.ReadValue<Vector2>();
            }
        };

        controls.Player.Interact.performed += ctx =>
        {
        };

        controls.Player.Move.canceled += ctx => move = Vector2.zero;

        //controls.Player.Shoot.performed += ctx =>
        //{
        //    if (!grounded && !usingShooter)
        //    {
        //        shootingGun.SetActive(true);
        //        grappleGun.SetActive(false);
        //        grappleGun2.SetActive(false);
        //        usingShooter = true;
        //        if (swinging) swing.stopSwing();
        //    }
        //    else if (usingShooter)
        //    {
        //        grappleGun.SetActive(true);
        //        grappleGun2.SetActive(true);
        //        shootingGun.SetActive(false);
        //        usingShooter = false;
        //    }
        //};

        groundMask = LayerMask.GetMask("Ground", "Environment");
    }

    public enum MovementState
    {
        walking,
        freeze
    }

    private void Start()
    {
        Time.timeScale = 1f;
    }
    private void Update()
    {
        
        if (freeze)
        {
            speed = 0;
        }
        else { speed = 12; }
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // handle drag
        if (grounded && !activeGrapple)
        {
            rb.linearDamping = groundDrag;
            speed = 16;
        }
        else
        {
            rb.linearDamping = 0;
            speed = 8;
        }

        Vector3 forward = Vector3.ProjectOnPlane(orientation.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(orientation.right, Vector3.up).normalized;

        Vector3 moveDir = forward * move.y + right * move.x;
        rb.AddForce(moveDir * speed, ForceMode.Force);

        /*Vector3 moveDir = transform.right * move.x + transform.forward * move.y;
        rb.AddForce(moveDir.normalized * speed , ForceMode.Force);*/

        //rb.AddForce(moveDir * speed, ForceMode.Acceleration);

        /*controller.Move(moveDir * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);*/
    }

    private void SetVelocity()
    {
        rb.linearVelocity = velocityToSet;
    }

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

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
}
