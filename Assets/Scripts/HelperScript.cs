using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperScript : MonoBehaviour
{
    [Header("Scripts")]
    PlayerReferences playerRef;
    EnemyReferences enemyRef;
    Grappling grapple;
    EnemyHealth enemyHealth;

    [Header("Bullet")]
    float bulletSpread;
    public Vector3 bulletSpreadVariance;

    GameObject player;
    public bool playerAlive;

    public LayerMask shootable;
    // Start is called before the first frame update
    void Start()
    {
        playerRef = FindAnyObjectByType<PlayerReferences>();
        grapple = FindAnyObjectByType<Grappling>();
        player = GameObject.FindWithTag("Player");

        bulletSpread = 0.1f;
        bulletSpreadVariance = new Vector3(bulletSpread, bulletSpread, bulletSpread);
        playerAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAlive)
        {
            player.SetActive(true);
        }
        else
        {
            player.SetActive(false);
        }
    }

    public Vector3 GetDirection(Transform cam, bool BulletSpread)
    {
        Vector3 direction = cam.forward;

        if (BulletSpread)
        {
            direction += new Vector3(Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x), Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y), Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z));
        }

        direction.Normalize();

        return direction;
    }

    public float DamageDealt(GameObject hit, float damage, float weapon, float playerWeight, float enemyWeight)
    {
        Debug.Log("calc damage " + hit);
        // gun = 1 fist = 2 scrap = 3

        // If the enemy has been hit (Player Attack)
        if (hit.CompareTag("enemy"))
        {
            Debug.Log("enemy hit");
            enemyRef = hit.GetComponentInParent<EnemyReferences>();

            // If the player is heavier than the enemy

            if (playerWeight > enemyWeight)
            {
                // If gun
                if (weapon == 1)
                {
                    // Decrease gun damage
                    damage /= 2;
                    enemyRef.KB = false;
                }
                // If fist
                else if (weapon == 2)
                {
                    // Increase fist damage and make it apply the knockback
                    damage *= 2;
                    enemyRef.KB = true;
                }
            }

            // If the player is lighter than the enemy
            else
            {
                // If heavy fist weapon, apply knockback
                if (weapon == 2)
                {
                    playerRef.KB = true;
                }
                else
                {
                    playerRef.KB = false;
                }
            }
        }
        // If the player has been hit (Enemy Attack)
        else if (hit.CompareTag("Player"))
        {
            enemyRef = GetComponent<EnemyReferences>();

            // If the player is heavier than the enemy
            if (playerWeight > enemyWeight)
            {
                // If gun
                if (weapon == 1)
                {
                    // Decrease gun damage
                    enemyRef.KB = false;
                }
                // If fist
                else if (weapon == 2)
                {
                    // Increase fist damage and make it apply the knockback
                    enemyRef.KB = true;
                }
            }

            // If the player is lighter than the enemy
            else
            {
                // If heavy fist weapon, apply knockback
                if (weapon == 2)
                {
                    playerRef.KB = true;
                }
                else
                {
                    playerRef.KB = false;
                }
            }
        }
        return damage;
    }

    public void Knockback(Rigidbody rb, Transform cam, bool forward, EnemyHealth enemy)
    {

        grapple.StopGrapple();

        rb.velocity = new Vector3(0, 0, 0);
        Vector3 direction;
        if (forward)
        {
            direction = GetDirection(cam, false);
        }
        else
        {
            direction = -GetDirection(cam, false);
        }
        direction.y += 0.2f;

        rb.AddForce(direction * 40, ForceMode.Impulse);
    }

    public void RotateTowards(Transform cam)
    {
        Quaternion rotation = Quaternion.LookRotation(GetDirection(cam, false));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }
}