using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyScript : MonoBehaviour
{
    public bool inRange;

    [Header("Scripts")]
    EnemyReferences enemyRef;
    EnemyHealth enemyHealth;
    Grappling grapple;
    Fist fist;
    HelperScript helper;
    PlayerReferences playerRef;

    [Header("References")]
    public Transform player;
    public Transform gunRotation;
    [HideInInspector] public Rigidbody rb;
    public Transform cam;
    public Transform playerCam;
    public LayerMask ground;

    [Header("Inverse Kinetics")]
    public Transform rightHandObj;
    public Transform leftHandObj;

    [Header("Animator")]
    float pathUpdateDeadline;
    float animationSpeed = 1;
    public float punchDis;

    float speed;
    float acceleration;

    [Header("Grappling")]
    public float savedGrappleSpeed;
    float grappleSpeed;
    public bool grappled;
    public float grappleDis;

    [Header("GroundCheck")]
    public float enemyHeight;
    public LayerMask Ground;
    public bool grounded;
    Vector3 enemyFeet;

    public Vector3 desiredDestination;

    float timeSinceShot;

    public MovementState state;
    public enum MovementState
    {
        shooting,
        punching,
        grappled,
        falling,
        knockback,
        knockbackEnd
    }
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerCam = GameObject.FindWithTag("MainCamera").transform;

        rb = GetComponent<Rigidbody>();
        enemyRef = GetComponent<EnemyReferences>();
        enemyHealth = GetComponent<EnemyHealth>();
        fist = GetComponent<Fist>();
        grapple = FindAnyObjectByType<Grappling>();
        helper = FindAnyObjectByType<HelperScript>();
        playerRef = FindAnyObjectByType<PlayerReferences>();

        speed = 7;
        acceleration = 20;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Determines the distance that the enemy is able to shoot
        // TODO: Make the enemy shoot
        punchDis = enemyRef.navMesh.stoppingDistance;
        grappled = false;
        timeSinceShot = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check
        enemyFeet = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        grounded = Physics.Raycast(enemyFeet, Vector3.down, enemyHeight * 0.5f + 0.2f, Ground);
        Debug.DrawRay(enemyFeet, Vector3.down * enemyHeight, Color.green);

        if (helper.playerAlive)
        {
            if(state == MovementState.grappled)
            {
                inRange = Vector3.Distance(transform.position, player.position) <= grappleDis;
            }
            else
            {
                inRange = Vector3.Distance(transform.position, player.position) <= punchDis;
            }
            

            Timer();
            if (enemyHealth.health > 0)
            {
                GroundCheck();

                if (state != MovementState.knockback)
                {
                    StateManager();
                    Behaviour();
                }
                else if (grounded)
                {
                    state = MovementState.shooting;
                }
            }
        }
        

        if(state != MovementState.punching && enemyRef.navMesh.enabled == true)
        {
            enemyRef.navMesh.isStopped = false;
        }
    }

    private void LateUpdate()
    {
        if (state != MovementState.knockback)
        {
            WeightCalculations();
        }
    }

    void Timer()
    {
        timeSinceShot += Time.deltaTime;
    }
    void GroundCheck()
    {
        enemyFeet = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        grounded = Physics.Raycast(enemyFeet, Vector3.down, enemyHeight * 0.5f + 0.2f, Ground);
        Debug.DrawRay(enemyFeet, Vector3.down * enemyHeight, Color.green);
    }
    void Behaviour()
    {
        if (state == MovementState.shooting)
        {
            UpdatePath();
            Shooting();
        }
        else if (state == MovementState.punching)
        {
            FacePlayer();
            Punch();
        }

        if (state == MovementState.grappled)
        {
            GrappleTowardsPlayer();
        }

        if (state == MovementState.grappled || state == MovementState.falling || state == MovementState.knockback)
        {
            enemyRef.navMesh.enabled = false;
        }
        else if (grounded)
        {
            enemyRef.navMesh.enabled = true;
        }
    }

    public void Punch()
    {
        if (enemyRef.navMesh.enabled == true)
        {
            rb.velocity = new Vector3(0, 0, 0);
            enemyRef.navMesh.velocity = Vector3.zero;
            enemyRef.navMesh.isStopped = true;
        }
        if (!enemyRef.punching)
        {
            enemyRef.punching = true;
            enemyRef.anim.SetTrigger("punch");
        }
    }
    public void TakeKnockback(bool forward)
    {
        Debug.Log("take knockback");
        enemyRef.navMesh.enabled = false;
        enemyHealth.kb = false;

        helper.Knockback(rb, playerCam, forward, enemyHealth);

        helper.RotateTowards(playerCam);
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        state = MovementState.falling;
    }
    void Shooting()
    {
        // Start shooting
        if (!enemyRef.shooting && !enemyRef.punching)
        {
            enemyRef.anim.SetTrigger("shoot");
            enemyRef.shooting = true;
            timeSinceShot = 0;
        }
        if(timeSinceShot >= 7)
        {
            enemyRef.anim.SetTrigger("shoot");
            enemyRef.shooting = true;
            timeSinceShot = 0;
        }
    }
    public void EndPunch()
    {
        enemyRef.punching = false;
    }

    public void EndShot()
    {
        enemyRef.shooting = false;
    }

    void WeightCalculations()
    {
        enemyRef.navMesh.speed = speed / enemyHealth.weight;
        enemyRef.navMesh.acceleration = acceleration / enemyHealth.weight;
        enemyRef.anim.speed = animationSpeed / enemyHealth.weight;
        grappleSpeed = savedGrappleSpeed;
        
        rb.mass = enemyHealth.weight;
    }
    void StateManager()
    {
        if (player != null)
        {
            

            if(rb.velocity.magnitude == 0)
            {
                grappled = false;
            }

            if (grappled)
            {
                state = MovementState.grappled;
            }
            else if (!grounded)
            {
                state = MovementState.falling;
            }
            else if (inRange)
            {
                state = MovementState.punching;
            }
            else
            {
                // Makes the enemy move towards the player
                state = MovementState.shooting;
            }
        }
        enemyRef.anim.SetFloat("speed", enemyRef.navMesh.desiredVelocity.sqrMagnitude);
    }

    void FacePlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }
    void OnAnimatorIK(int layerIndex)
    {
        enemyRef.anim.SetLookAtPosition(playerCam.position);
        enemyRef.anim.SetLookAtWeight(1, 0, 1); // Second value is the body weight, third value is the head weight (if = 1 then it moves)

        enemyRef.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        enemyRef.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        enemyRef.anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
        enemyRef.anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);

        enemyRef.anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        enemyRef.anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        enemyRef.anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
        enemyRef.anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
    }
    void UpdatePath()
    {
        rb.velocity = new Vector3(0,0,0);

        if (enemyRef.navMesh.enabled == false) return;


        if ( Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + enemyRef.pathUpdateDelay;
            enemyRef.navMesh.SetDestination(player.position);
        }
    }
    public void GrappleTowardsPlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.Normalize();

        if (rb.velocity.magnitude > grappleSpeed)
        {
            rb.velocity = rb.velocity.normalized * grappleSpeed;
        }
        rb.AddForce(direction * grappleSpeed * 1000f * Time.deltaTime, ForceMode.Force);
        rb.freezeRotation = false;

        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
        rb.freezeRotation = true;

        if (inRange)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            enemyRef.navMesh.velocity = Vector3.zero;
            grapple.StopAllCoroutines();
            grapple.StartReturn();
        }
    }
    public void StopGrappling()
    {
        grappled = false;
    }
}
