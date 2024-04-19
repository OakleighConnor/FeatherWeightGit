using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform rightHandObj = null;
    public Transform gunRotation;
    EnemyReferences er;
    EnemyHealth eh;
    Weapon weapon;
    public Transform cam;

    [Header("Cooldown")]
    float shootCd;
    float activeShootCd;

    [Header("Animator")]

    float pathUpdateDeadline;

    float shootDis;

    float speed;
    float acceleration;
    float animSpeed;
    void Awake()
    {
        er = GetComponent<EnemyReferences>();
        eh = GetComponent<EnemyHealth>();
        weapon = GetComponent<Weapon>();

        speed = 7;
        acceleration = 20;
        animSpeed = 1;
        shootCd = 2f;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Determines the distance that the enemy is able to shoot
        // TODO: Make the enemy shoot
        shootDis = er.navMesh.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        WeightCalculations();
        Behaviour();
        Timers();
    }
    void Timers()
    {
        if (activeShootCd > 0)
        {
            activeShootCd -= Time.deltaTime;
        }
    }
    void WeightCalculations()
    {
        er.navMesh.speed = speed / eh.weight;
        er.navMesh.acceleration = acceleration / eh.weight;
        er.anim.speed = animSpeed / eh.weight/ 1.5f * 2;
    }
    void Behaviour()
    {
        if (player != null)
        {
            bool inRange = Vector3.Distance(transform.position, player.position) <= shootDis;

            if (inRange)
            {
                // TODO: Make the enemy punch the player
            }
            else
            {
                // Makes the enemy move towards the player
                
                UpdatePath();
            }

            if (er.anim.GetFloat("speed") == 0)
            {
                FacePlayer();
            }

            if (!weapon.shooting && activeShootCd == 0)
            {
                activeShootCd = shootCd;
                weapon.shooting = true;
                er.anim.SetTrigger("shoot");
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

        if (!weapon.shooting)
        {
            Vector3 lookPos = player.position - transform.position;
            lookPos.y -= 1.6f;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            gunRotation.rotation = Quaternion.Slerp(gunRotation.rotation, rotation, 0.2f);
        }
    }

    void UpdatePath()
    {
        if ( Time.time >= pathUpdateDeadline)
        {
            Debug.Log("Updating Path");
            pathUpdateDeadline = Time.time + er.pathUpdateDelay;
            er.navMesh.SetDestination(player.position);
        }
    }
}
