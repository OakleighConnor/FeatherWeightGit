using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyScript : MonoBehaviour
{
    bool inRange;

    [Header("References")]
    public Transform player;
    public Transform rightHandObj = null;
    public Transform gunRotation;
    EnemyReferences er;
    EnemyHealth eh;
    Grappling grapple;
    [HideInInspector] public Rigidbody rb;
    public Transform cam;
    public Transform playerCam;
    public LayerMask ground;

    [Header("Cooldown")]
    float shootCd;
    public float activeShootCd;

    [Header("Animator")]

    float pathUpdateDeadline;
    float animationSpeed = 1;
    float shootDis;

    float speed;
    float acceleration;

    [Header("Grappling")]
    public float savedGrappleSpeed;
    float grappleSpeed;
    public bool grappled;

    [Header("GroundCheck")]
    public float enemyHeight;
    public LayerMask Ground;
    public bool grounded;
    Vector3 enemyFeet;

    public MovementState state;
    public enum MovementState
    {
        shooting,
        punching,
        grappled,
        falling,
        knockback
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        er = GetComponent<EnemyReferences>();
        eh = GetComponent<EnemyHealth>();
        grapple = player.GetComponent<Grappling>();

        speed = 7;
        acceleration = 20;
        shootCd = 1.2f;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Determines the distance that the enemy is able to shoot
        // TODO: Make the enemy shoot
        shootDis = er.navMesh.stoppingDistance;
        grappled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check
        enemyFeet = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        grounded = Physics.Raycast(enemyFeet, Vector3.down, enemyHeight * 0.5f + 0.2f, Ground);
        Debug.DrawRay(enemyFeet, Vector3.down * enemyHeight, Color.green);

        if (eh.health > 0)
        {
            if (state != MovementState.knockback)
            {
                WeightCalculations();
                StateManager();
                Behaviour();
            }
            else if (grounded)
            {
                state = MovementState.shooting;
            }
        }
        else if (state == MovementState.falling)
        {
            Debug.Log("death");
            eh.Death();
        }
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
        }
        else if (state == MovementState.grappled)
        {
            GrappleTowardsPlayer();
        }

        if (state == MovementState.grappled || state == MovementState.falling || state == MovementState.knockback)
        {
            er.navMesh.enabled = false;
        }
        else if (grounded)
        {
            er.navMesh.enabled = true;
        }
    }

    public void Knockback()
    {
        rb.velocity = new Vector3(0, 0, 0);
        er.navMesh.enabled = false;

        Debug.Log("knocking backwards the enemy");

        Vector3 direction = playerCam.forward;
        direction.Normalize();

        rb.AddForce(direction * 30, ForceMode.Impulse);

        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
        rb.freezeRotation = true;

        eh.kb = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        state = MovementState.falling;
    }
    void Shooting()
    {
        // Start shooting
        if (activeShootCd <= 0)
        {
            er.anim.SetTrigger("shoot");
            activeShootCd = shootCd * eh.weight;
        }
        else
        {
            activeShootCd -= Time.deltaTime;
        }
    }

    void WeightCalculations()
    {
        er.navMesh.speed = speed / eh.weight;
        er.navMesh.acceleration = acceleration / eh.weight;
        er.anim.speed = animationSpeed / eh.weight/ 1.5f * 2;
        rb.mass = eh.weight;
    }
    void StateManager()
    {
        if (player != null)
        {
            inRange = Vector3.Distance(transform.position, player.position) <= shootDis;

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
        er.anim.SetFloat("speed", er.navMesh.desiredVelocity.sqrMagnitude);
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
        er.anim.SetLookAtPosition(player.position);
        er.anim.SetLookAtWeight(1, 0, 1); // Second value is the body weight, third value is the head weight (if = 1 then it moves)

        er.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        er.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        er.anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
        er.anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
    }

    void UpdatePath()
    {
        rb.velocity = new Vector3(0,0,0);

        if (er.navMesh.enabled == false) return;

        if ( Time.time >= pathUpdateDeadline)
        {
            Debug.Log("Updating Path");
            pathUpdateDeadline = Time.time + er.pathUpdateDelay;
            er.navMesh.SetDestination(player.position);
        }
    }
    public void GrappleTowardsPlayer()
    {
        Debug.Log("enemy is lighter than the player");

        Vector3 direction = player.position - transform.position;
        direction.Normalize();

        grappleSpeed = savedGrappleSpeed;

        if (rb.velocity.magnitude > grappleSpeed)
        {
            rb.velocity = rb.velocity.normalized * grappleSpeed;
        }
        rb.AddForce(direction * grappleSpeed * 10f, ForceMode.Force);
        rb.freezeRotation = false;

        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
        rb.freezeRotation = true;

        if (inRange)
        {
            grapple.StopGrapple();
        }
    }
    public void StopGrappling()
    {
        grappled = false;
    }
}
