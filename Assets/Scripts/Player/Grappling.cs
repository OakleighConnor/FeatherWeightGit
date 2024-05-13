using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    PlayerReferences playerRef;
    EnemyScript enemyScript;
    EnemyHealth enemyHealth;
    Rigidbody rb;
    GameObject hitObject;

    [Header("Grapple Components")]
    public GameObject grapple;
    public GameObject grappleHand;
    public Transform shootPoint;
    public Transform grappleOrigin;
    public Transform grappleConnection;

    public Vector3 grapplePoint;
    float distance;

    public Vector3 grappledFrom;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    float currentMaxGrappleDistance;
    public float grappleDelayTime;
    RaycastHit hit;
    bool enemyGrappled;
    public float savedGrappleSpeed;
    float grappleSpeed;
    public Vector3 direction;

    public bool inRange;
    public bool returnRange;

    [Header("Hand")]
    public float grappleHandSpeed = 6;
    Quaternion grappleRotation;
    

    [Header("Timers")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;


    public GrapplingState state;
    public enum GrapplingState
    {
        idle,
        launching,
        grappling,
        returning
    }
    // Start is called before the first frame update
    void Start()
    {
        playerRef = GetComponent<PlayerReferences>();
        rb = GetComponent<Rigidbody>();
        enemyScript = null;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();

        CooldownTimer();

        GrappleExecution();
    }

    void LateUpdate()
    {
        // Update the position of the hand and line renderer
        HandManager();
    }

    void CooldownTimer()
    {
        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    void GrappleExecution()
    {
        if (state == GrapplingState.grappling)
        {
            if (enemyScript != null)
            {
                if (enemyGrappled)
                {
                    grapplePoint = hitObject.transform.position;
                    if (enemyHealth.weight < playerRef.weight)
                    {
                        enemyScript.grappled = true;
                    }
                    else
                    {
                        enemyScript.grappled = false;
                        ExecuteGrapple();
                        grappleSpeed = savedGrappleSpeed / 1.5f;
                    }
                }
                else
                {
                    enemyScript.grappled = false;
                    ExecuteGrapple();
                    grappleSpeed = savedGrappleSpeed;
                }
            }
            else
            {
                ExecuteGrapple();
                grappleSpeed = savedGrappleSpeed;
            }
        }
    }
    void Inputs()
    {

        if (Input.GetKeyDown(grappleKey) && state == GrapplingState.idle)
        {
            //state = GrapplingState.launching;
            //StartGrappling();
            playerRef.anim.SetBool("grappling", true);
        }

        if (!Input.GetKey(grappleKey))
        {
            if (state == GrapplingState.grappling)
            {
                StartReturn();
            }
            else if(state == GrapplingState.idle)
            {
                playerRef.anim.SetBool("grappling", false);
            }
            else if (state == GrapplingState.launching)
            {
                StartReturn();
            }
        }
    }

    void HandManager()
    {
        // The grapple moving towards the hit point of the raycast

        if (state == GrapplingState.launching)
        {
            inRange = Vector3.Distance(grappleHand.transform.position, grapplePoint) <= 1f;
        }
        else
        {
            inRange = false;
        }

        // Grappling (Moving the player towards the grappled position)

        if (state == GrapplingState.grappling)
        {
            grappleHand.transform.position = grapplePoint;
            grappleHand.transform.rotation = grappleRotation;
        }

        // Grapple's head returning to the player

        if (state == GrapplingState.returning)
        {
            returnRange = Vector3.Distance(grappleHand.transform.position, grappleOrigin.position) <= 3f;
        }
        else
        {
            returnRange = false;
        }

        lr.SetPosition(0, gunTip.position);


        if (state == GrapplingState.idle)
        {
            grappleHand.transform.position = grappleOrigin.transform.position;
            grappleHand.transform.rotation = grappleOrigin.transform.rotation;
            lr.enabled = false;
        }
        else
        {
            lr.enabled = true;
            lr.SetPosition(1, grappleConnection.position);
        }
    }


    public void StartGrappling()
    {
        state = GrapplingState.launching;

        if (grapplingCdTimer > 0) return;

        // Divides the max distance that the player can grapple from by half of the weight.
        currentMaxGrappleDistance = maxGrappleDistance / playerRef.weight;

        // Fires a raycast from the position of the camera forwards. If it is within the max grapple distance and it is something that can be grappled then the code activates
        if(Physics.Raycast(cam.position, cam.forward, out hit, currentMaxGrappleDistance, whatIsGrappleable))
        {
            // Activates what should happen when the point connects correctly
            // Saves the location that was hit
            grapplePoint = hit.point;
            grappleRotation = Quaternion.FromToRotation(Vector3.left, hit.normal);

            distance = Vector3.Distance(grapplePoint, transform.position);


            // Checks if the enemy is what the grapple has hit
            if(hit.collider.gameObject != null)
            {
                hitObject = hit.collider.gameObject;

                if (hitObject.CompareTag("enemy"))
                {
                    enemyGrappled = true;

                    hitObject = hit.collider.gameObject;
                    enemyHealth = hitObject.GetComponentInParent<EnemyHealth>();
                    enemyScript = hitObject.GetComponentInParent<EnemyScript>();
                    // Compares the two weight values of the enemy and the player
                    if (enemyHealth.weight < playerRef.weight)
                    {
                        // Grapples the enemy towards the player
                        grapplePoint = hitObject.transform.position;
                        enemyScript.rb.velocity = new Vector3(0f, 0f, 0f);
                    }
                    else
                    {
                        // Grapples player towards the enemy
                        StopAllCoroutines();
                        StartCoroutine(ShootHand(hitObject, grapplePoint));
                    }
                }
            }
            else
            {
                hitObject = null;
                enemyGrappled = false;
                // Plays the grapple
                Debug.Log("this stange bit");
                StopAllCoroutines();
                StartCoroutine(ShootHand(hitObject, grapplePoint));
            }
        }
        else
        {
            // If the grapple hits nothing

            grapplePoint = cam.position + cam.forward * currentMaxGrappleDistance;
            hitObject = null;

            enemyGrappled = false;
            // Plays the grapple
            Debug.Log("nobody hit");
            StopAllCoroutines();
            StartCoroutine(ShootHand(hitObject, grapplePoint));
        }

        StopAllCoroutines();
        StartCoroutine(ShootHand(hitObject, grapplePoint));
    }

    private IEnumerator ShootHand(GameObject hit, Vector3 hitPos)
    {

        // Move the grapple towards the grappled position
        while (!inRange)
        {
            grappleHand.transform.position = Vector3.MoveTowards(grappleHand.transform.position, hitPos, grappleHandSpeed * 100 * Time.deltaTime);


            yield return null;
        }

        // Grapple reached desired location

        // When it hits
        if (hit != null)
        {
            // If the grapple hit something

            if (hit.CompareTag("enemy"))
            {
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
            }
            else if (hit.CompareTag("ground"))
            {

            }

            state = GrapplingState.grappling;
        }
        else
        {
            // If the grapple didn't hit anything
            Debug.Log("nothing hit");
            StartReturn();
        }
    }



    private void ExecuteGrapple()
    {
        // Make the grapple give momentum towards the location that the grapple lands at if the player isn't there already

        // DIRECTION = DESTINATION - SOURCE
        
        Vector3 direction = grapplePoint - transform.position;
        direction.Normalize();

        // Rotate the grapple to point towards the location that it landed
        grappleSpeed = savedGrappleSpeed / rb.mass;

        rb.AddForce(direction.normalized * grappleSpeed * 1000f * Time.deltaTime, ForceMode.Force);
        
    }

    public void StartReturn()
    {
        state = GrapplingState.returning;

        grappleHand.transform.rotation = grappleOrigin.transform.rotation;

        StopAllCoroutines();
        StartCoroutine(ReturnGrapple());
    }
    private IEnumerator ReturnGrapple()
    {
        while (!returnRange)
        {
            grappleHand.transform.position = Vector3.MoveTowards(grappleHand.transform.position, grappleOrigin.transform.position, grappleHandSpeed * 100 * Time.deltaTime);

            yield return null;
        }


        StopGrapple();
    }

    public void StopGrapple()
    {
        playerRef.anim.SetBool("grappling", false);

        state = GrapplingState.idle;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;

        enemyGrappled = false;

        if(enemyScript != null)
        {
            enemyScript.StopGrappling();
        }

        hitObject = null;
        enemyHealth = null;
        enemyScript = null;
    }
}