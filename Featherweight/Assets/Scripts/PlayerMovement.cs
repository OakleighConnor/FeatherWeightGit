using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    

    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float dashSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Sliding")]
    public float slideSpeed;
    public float slideYScale;
    private float startYScale;
    public bool sliding;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode slideKey = KeyCode.LeftControl;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    [Header("SlopeHandling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    Rigidbody rb;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 slideDirection;


    public MovementState state;


    public enum MovementState
    {
        sprinting,
        air,
        dashing,
        sliding
    }

    public bool dashing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        sliding = false;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        
        MyInput();
        SpeedControl();
        StateHandler();

        if(dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }

        //handle drag
        if (grounded && state != MovementState.sliding || state != MovementState.dashing)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //dash
        if (Input.GetKeyDown(dashKey) && state != MovementState.sliding)
        {
            Dash();
        }

        if (Input.GetKey(jumpKey) && readyToJump && grounded && state != MovementState.sliding)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //slide
        if (Input.GetKeyDown(slideKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 20f, ForceMode.Impulse);

            sliding = true;
        }

        if (Input.GetKeyUp(slideKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            sliding = false;
        }
    }

    void StateHandler()
    {
        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
        }

        // Mode - Sliding
        else if (Input.GetKey(slideKey))
        {
            state = MovementState.sliding;
        }

        // Mode - Sprinting
        else if (grounded)
        {
            state = MovementState.sprinting;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        if (!sliding)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        }

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //Applies the force in the direction input
        if (grounded)
        {
            if (sliding)
            {
                rb.AddForce(moveDirection.normalized * slideSpeed * 10f, ForceMode.Force);
            }
            else
            {

                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //turn gravity off while on slope
        rb.useGravity = !OnSlope() || !dashing;
    }
    void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if(rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        //limiting speed on ground or in air
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        //limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void Dash()
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        dashing = true;

        Vector3 forceToApply = orientation.forward * dashForce + orientation.up * dashUpwardForce;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        dashing = false;
    }
}
