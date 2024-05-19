using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerWeapons player;
    public PlayerReferences playerRef;
    public EnemyReferences enemyRef;
    public HelperScript helper;
    public EnemyHealth enemyHealth;
    public PlayerHealth playerHealth;

    [Header("References")]
    Animator anim;
    GameObject objectHit;

    [Header("Fist")]
    public Transform fistDamagePoint;
    public float fistDistance = 2.5f;
    public float fistDamage = 25;
    public bool fistHitbox;
    LayerMask interactable;

    // Start is called before the first frame update
    void Start()
    {
        helper = FindAnyObjectByType<HelperScript>();
        playerRef = FindAnyObjectByType<PlayerReferences>();
        playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (CompareTag("Player"))
        {
            player = GetComponent<PlayerWeapons>();
        }
        else
        {
            player = null;
        }

        // If the script is on a player
        if (player != null)
        {
            anim = player.weapons.GetComponent<Animator>();
        }

        // If the script is on an enemy
        else
        {
            anim = GetComponent<Animator>();
            enemyRef = GetComponent<EnemyReferences>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fistHitbox)
        {
            if (CompareTag("Player"))
            {
                FistHitbox(playerRef.cam);
            }
            else
            {
                FistHitbox(enemyRef.cam);
            }
        }
    }
    public void StartPunch()
    {
        fistHitbox = true;
    }
    public void EndPunch()
    {
        fistHitbox = false;
    }
    
    public void FistHitbox(Transform cam)
    {
        helper = FindAnyObjectByType<HelperScript>();

        Debug.Log("play animation");
        if (CompareTag("Player"))
        {
            interactable = playerRef.interactableLayers;
        }
        else
        {
            interactable = enemyRef.interactableLayers;
        }

        Vector3 direction = helper.GetDirection(cam, false);

        if (Physics.Raycast(fistDamagePoint.position, direction + new Vector3(0, 0.3f, 0), out RaycastHit hit1, fistDistance, interactable))
        {
            objectHit = hit1.collider.gameObject;
        }
        else if (Physics.Raycast(fistDamagePoint.position, direction, out RaycastHit hit2, fistDistance, interactable))
        {
            objectHit = hit2.collider.gameObject;
        }
        else if (Physics.Raycast(fistDamagePoint.position, direction + new Vector3(0, -0.3f, 0), out RaycastHit hit3, fistDistance, interactable))
        {
            objectHit = hit3.collider.gameObject;
        }
        else
        {
            objectHit = null;
        }

        Debug.DrawRay(fistDamagePoint.position, direction * fistDistance, Color.green, 3);
        Debug.DrawRay(fistDamagePoint.position, direction + new Vector3(0, 0.3f, 0) * fistDistance, Color.red, 3);
        Debug.DrawRay(fistDamagePoint.position, direction + new Vector3(0, -0.3f, 0) * fistDistance, Color.blue, 3);

        if (objectHit != null)
        {
            Debug.Log(objectHit);

            if (objectHit.CompareTag("enemy"))
            {
                Debug.Log("hit enemy");
                fistHitbox = false;
                enemyHealth = objectHit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(helper.DamageDealt(objectHit, fistDamage, 2, playerRef.weight, enemyHealth.weight, objectHit.GetComponentInParent<EnemyReferences>()), true, true); // Bools are Enemy KB and Player KB
            }
            else if (objectHit.name == "PlayerObj")
            {
                fistHitbox = false;
                Debug.Log("Player has been punched");

                enemyHealth = GetComponent<EnemyHealth>();

                playerHealth.TakeDamage(helper.DamageDealt(objectHit, fistDamage, 2, playerRef.weight, enemyHealth.weight, GetComponent<EnemyReferences>()), true, gameObject);
            }
        }
    }
}
// Make the player not take way too much knockback