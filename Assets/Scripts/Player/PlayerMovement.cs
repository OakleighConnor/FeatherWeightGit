using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    float defaultMoveSpeed;

    public float groundDrag;

    public float dashSpeed;
    public float dashSpeedChangeFactor;

    public Vector3 moveDirection;
    Vector3 slideDirection;

    public MovementState state;
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

    [Header("Inputs")]
    KeyCode jumpKey = KeyCode.Space;
    KeyCode dashKey = KeyCode.LeftShift;
    KeyCode slideKey = KeyCode.LeftControl;
    KeyCode pauseKey = KeyCode.Escape;
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
    PlayerHealth playerHealth;
    Rigidbody rb;
    ParticleManager pm;
    Grappling grapple;
    PlayerReferences references;
    HelperScript helper;
    EnemyHealth enemyHealth;
    PauseMenu pauseMenuScript;
    GameObject pauseMenu;
    AudioManager am;
    UIButtons ui;
    PlatformManager platform;
    Win win;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    float dashDuration;
    public float savedDashDuration;

    [Header("Settings")]
    public bool allowAllDirections = true;
    public bool disableGravity = false;
    public bool resetVel = true;

    [Header("Cooldown")]
    public float dashCd;
    float dashCdTimer;


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
        references = GetComponent<PlayerReferences>();
        pm = FindAnyObjectByType<ParticleManager>();
        grapple = GetComponent<Grappling>();
        rb.freezeRotation = true;
        helper = FindAnyObjectByType<HelperScript>();
        pauseMenuScript = GetComponent<PauseMenu>();
        pauseMenu = pauseMenuScript.pauseMenu;
        am = FindAnyObjectByType<AudioManager>();
        ui = FindAnyObjectByType<UIButtons>();
        Time.timeScale = 1;

        readyToJump = true;
        sliding = false;

        startYScale = transform.localScale.y;
        defaultMoveSpeed = moveSpeed;

        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        WeightImpactCalculations();

        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight, Ground);

        if (grounded) dashDuration /= 2;

        if (!pauseMenuScript.paused)
        {
            GameInput();
        }
        UIInput();
        SpeedControl();
        StateHandler();

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

    void UIInput()
    {
        win = GameObject.FindGameObjectWithTag("WinPortal").GetComponentInChildren<Win>();
        if(win != null)
        {
            if (Input.GetKeyDown(pauseKey) && !win.winScreen.activeSelf)
            {
                ui = FindAnyObjectByType<UIButtons>();
                if (ui.settingsMenu.activeSelf)
                {
                    ui.ToggleSettings();
                }
                else
                {
                    pauseMenuScript.TogglePause();
                }
            }
        }
        else if (Input.GetKeyDown(pauseKey))
        {
            ui = FindAnyObjectByType<UIButtons>();
            if (ui.settingsMenu.activeSelf)
            {
                ui.ToggleSettings();
            }
            else
            {
                pauseMenuScript.TogglePause();
            }
        }
    }

    void WeightImpactCalculations()
    {
        // A variable called "weight" just feels a lot simplistic to work with rather than rb.mass
        rb.mass = references.weight;

        // The effect of weight on the movement

        
        // Slope
        maxSlopeAngle = originalMaxSlopeAngle / references.weight;

        // Dash
        dashCd = 0.5f * references.weight;

        // The effect of weight on the grapple
        grapple.grapplingCd *= references.weight;
    }

    void GameInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        //dash
        if (sliding)
        {
            state = MovementState.sliding;
        }
        else if (Input.GetKeyDown(dashKey) && state != MovementState.sliding)
        {
            Dash();
        }

        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded && state != MovementState.sliding)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //slide
        if (Input.GetKeyDown(slideKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);

            rb.AddForce(Vector3.down * 10, ForceMode.Impulse);

            sliding = true;
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
            state = MovementState.sliding;
            desiredMoveSpeed = slideSpeed;
        }

        // Mode - Sprinting
        else if (grounded)
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = defaultMoveSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
            desiredMoveSpeed = defaultMoveSpeed;
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
        //if (state == MovementState.dashing) return;

        //calculate movement direction
        if (!sliding)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        }

        //on slope
        if (OnSlope() && !exitingSlope && state != MovementState.gp)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 2000f * Time.deltaTime, ForceMode.Force);

            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 8000f * Time.deltaTime, ForceMode.Force);
            }
        }

        //Applies the force in the direction input
        if (grounded && state != MovementState.gp)
        {
            if (sliding)
            {
                moveSpeed = slideSpeed / references.weight;
            }

            rb.AddForce(moveDirection.normalized * moveSpeed * 1000f * Time.deltaTime, ForceMode.Force);

        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 1000f * airMultiplier * Time.deltaTime, ForceMode.Force);
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
                rb.velocity = rb.velocity.normalized * moveSpeed / references.weight;
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
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce * 10f, ForceMode.Impulse);
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

        rb.AddForce(delayedForceToApply * 100 * Time.deltaTime, ForceMode.Impulse);
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("scrap"))
        {
            am = FindAnyObjectByType<AudioManager>();
            am.PlaySFX(am.heal);
            Destroy(other.gameObject.transform.parent.gameObject);
            playerHealth = GetComponent<PlayerHealth>();
            playerHealth.health += 100;
        }
    }
}