using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    PlayerMovement pm;
    EnemyScript enemyScript;
    EnemyHealth enemyHealth;
    Rigidbody rb;
    GameObject hitEnemy;
    public Weapon weapon;
    public Vector3 grappledFrom;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public Transform grapple;

    [Header("Grappling")]
    public float maxGrappleDistance;
    float currentMaxGrappleDistance;
    public float grappleDelayTime;
    RaycastHit hit;
    bool enemyGrappled;
    Vector3 grapplePoint;
    public float savedGrappleSpeed;
    public float grappleSpeed;
    public Vector3 direction;
    bool grappling;

    [Header("Timers")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;
    Vector3 moveDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        enemyScript = null;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(grappleKey))
        {
            StartGrappling();
        }
        if (Input.GetKeyUp(grappleKey))
        {
            StopGrapple();
        }

        if(grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }

        if (grappling)
        {
            lr.SetPosition(1, grapplePoint);
            if(enemyScript != null)
            {
                if (enemyGrappled)
                {
                    grapplePoint = hitEnemy.transform.position;
                    if (enemyHealth.weight < pm.weight)
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

    void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrappling()
    {
        if (grapplingCdTimer > 0) return;

        // Divides the max distance that the player can grapple from by half of the weight.
        currentMaxGrappleDistance = maxGrappleDistance / (pm.weight / 2);

        // Fires a raycast from the position of the camera forwards. If it is within the max grapple distance and it is something that can be grappled then the code activates
        if(Physics.Raycast(cam.position, cam.forward, out hit, currentMaxGrappleDistance, whatIsGrappleable))
        {
            // Activates what should happen when the point connects correctly
            
            // Saves the location that was hit
            grapplePoint = hit.point;

            //Enables the rope
            grappling = true;
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);

            // Checks if the enemy is what the grapple has hit
            if(hit.collider.gameObject.CompareTag("enemy"))
            {
                enemyGrappled = true;

                hitEnemy = hit.collider.gameObject;
                enemyHealth = hitEnemy.GetComponentInParent<EnemyHealth>();
                enemyScript = hitEnemy.GetComponentInParent<EnemyScript>();
                // Compares the two weight values of the enemy and the player
                if (enemyHealth.weight < pm.weight)
                {
                    Debug.Log("Enemy is lighter than the player");
                    // Grapples the enemy towards the player
                    grapplePoint = hitEnemy.transform.position;
                    enemyScript.rb.velocity = new Vector3(0f, 0f, 0f);
                }
                else
                {
                    // Grapples player towards the enemy
                    Invoke(nameof(ExecuteGrapple), grappleDelayTime);
                }
            }
            else
            {
                enemyGrappled = false;
                // Plays the grapple
                Invoke(nameof(ExecuteGrapple), grappleDelayTime);
            }
        }
        else
        {
            // Cancels the grapple
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
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

        rb.AddForce(direction.normalized * grappleSpeed * 10f, ForceMode.Force);

    }

    public void StopGrapple()
    {
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;

        enemyGrappled = false;

        if(enemyScript != null)
        {
            enemyScript.StopGrappling();
        }

        hitEnemy = null;
        enemyHealth = null;
        enemyScript = null;
    }
}