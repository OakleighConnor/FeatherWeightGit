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
    public float dashSpeedChangeFactor;

    public bool gp;
    public float gpSpeed;

    public Vector3 moveDirection;
    Vector3 slideDirection;

    public MovementState state;
    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    public float airTime;

    [Header("Sliding")]
    public float slideSpeed;
    public float slideYScale;
    private float startYScale;
    public bool sliding;

    [Header("Inputs")]
    KeyCode jumpKey = KeyCode.Space;
    KeyCode dashKey = KeyCode.LeftShift;
    KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask Ground;
    public bool grounded;

    [Header("SlopeHandling")]
    float maxSlopeAngle;
    public float originalMaxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("References")]
    public Transform orientation;
    public Transform playerCam;
    Rigidbody rb;
    ParticleManager pm;
    Grappling grapple;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    [Header("Settings")]
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    float dashCdTimer;

    [Header("Weight")]
    public float weight;

    public enum MovementState
    {
        sprinting,
        air,
        dashing,
        gp,
        sliding
    }

    public bool dashing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        pm = FindAnyObjectByType<ParticleManager>();
        grapple = GetComponent<Grappling>();
        rb.freezeRotation = true;

        readyToJump = true;
        sliding = false;

        startYScale = transform.localScale.y;
    }

    void Update()
    {

        WeightImpactCalculations();

        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        
        MyInput();
        SpeedControl();
        StateHandler();

        if (grounded)
        {
            airTime = 0;
        }
        else
        {
            airTime += Time.deltaTime;
        }

        /*if (airTime >= .5f)
        {
            print("Ground pound");
        }*/

        if (dashCdTimer > 0)
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

        if(transform.rotation != Quaternion.Euler(0, 0, 0))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void WeightImpactCalculations()
    {
        // A variable called "weight" just feels a lot simplistic to work with rather than rb.mass
        weight = rb.mass;

        // The effect of weight on the movement
        // Slope
        maxSlopeAngle = originalMaxSlopeAngle / weight;

        // Dash
        dashCd *= weight;

        // The effect of weight on the grapple
        grapple.grapplingCd *= weight;
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
            rb.AddForce(Vector3.down * gpSpeed * weight * 2, ForceMode.Impulse);

            if (grounded || airTime <= 0.5f)
            {
                sliding = true;
            }
            else
            {
                gp = true;
            }
        }

        if (Input.GetKeyUp(slideKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            sliding = false;
        }
    }
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private MovementState lastState;
    private bool keepMomentum;
    void StateHandler()
    {

        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        // Mode - Sliding
        else if (Input.GetKey(slideKey))
        {
            if (sliding)
            {
                state = MovementState.sliding;
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                if (grounded && gp)
                {
                    if (rb.velocity.y == 0 || OnSlope())
                    {
                        GroundPound();
                        gp = false;
                    }
                }
                state = MovementState.gp;
            }
        }

        // Mode - Sprinting
        else if (grounded)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            desiredMoveSpeed = sprintSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing)
        {
            keepMomentum = true;
        }

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private float speedChangeFactor;
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        //smoothly lerp movement speed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MovePlayer()
    {
        if (state == MovementState.dashing) return;

        //calculate movement direction
        if (!sliding)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        }

        //on slope
        if (OnSlope() && !exitingSlope && state != MovementState.gp)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //Applies the force in the direction input
        if (grounded && state != MovementState.gp)
        {
            if (sliding)
            {
                moveSpeed = slideSpeed / weight;
            }
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
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
                rb.velocity = rb.velocity.normalized * moveSpeed / weight;
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

        Transform forwardT;

        forwardT = orientation;

        Vector3 direction = GetDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = true;
        }

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;
    private void DelayedDashForce()
    {
        if (resetVel)
        {
            rb.velocity = Vector3.zero;
        }

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        dashing = false;
    }

    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }

        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }

    private void GroundPound()
    {
        pm.GroundPound();
    }

    public void Knockback()
    {
        grapple.StopGrapple();

        rb.velocity = new Vector3(0, 0, 0);

        Debug.Log("player knocked back");

        Vector3 direction = -playerCam.forward;
        direction.Normalize();

        rb.AddForce(direction * 40, ForceMode.Impulse);
    }
}