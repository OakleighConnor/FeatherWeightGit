using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyScript : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    private EnemyReferences er;
    public Transform rightHandObj = null;

    [Header("Animator")]
    public float headWeight;
    public float bodyWeight;

    private float pathUpdateDeadline;

    private float shootDis;
    void Awake()
    {
        er = GetComponent<EnemyReferences>();
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
        if(player != null)
        {
            bool inRange = Vector3.Distance(transform.position, player.position) <= shootDis;

            if (inRange)
            {
                // Makes the enemy move its entire body to face towards the player. This will also be where the enemy starts to punch the player.

                FacePlayer();
            }
            else
            {
                // Makes the enemy move towards the player

                UpdatePath();
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
        er.anim.SetLookAtWeight(1, bodyWeight, headWeight);

        er.anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        er.anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        er.anim.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
        er.anim.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
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
