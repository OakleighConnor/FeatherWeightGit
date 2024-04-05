using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm;
    private Rigidbody rb;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;
    public Transform grapple;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;

    public Vector3 grapplePoint;

    public float grappleSpeed;

    public Vector3 direction;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    Vector3 moveDirection;
    private bool grappling;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
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
            ExecuteGrapple();
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


        RaycastHit hit;
        // Fires a raycast from the position of the camera forwards. If it is within the max grapple distance and it is something that can be grappled then the code activates
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            // Activates what should happen when the point connects correctly
            
            // Saves the location that was hit
            grapplePoint = hit.point;

            //Enables the rope
            grappling = true;
            lr.enabled = true;
            lr.SetPosition(1, grapplePoint);

            // Plays the grapple
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
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

        rb.AddForce(direction.normalized * grappleSpeed * 10f, ForceMode.Force);
    }

    private void StopGrapple()
    {
        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }
}
