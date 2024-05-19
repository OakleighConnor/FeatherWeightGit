using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperScript : MonoBehaviour
{
    public static HelperScript instance;

    [Header("Scripts")]
    PlayerReferences playerRef;
    EnemyReferences enemyRef;
    Grappling grapple;
    PlayerWeapons weapon;

    [Header("Bullet")]
    public float bulletSpread;
    Vector3 bulletSpreadVariance;
    int shotsFired;

    GameObject player;

    public LayerMask shootable;

    [Header("Knockback")]
    public float knockback;
    float originalKnockback;

    void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        weapon = FindAnyObjectByType<PlayerWeapons>();
        player = GameObject.FindWithTag("Player");

        bulletSpreadVariance = new Vector3(bulletSpread, bulletSpread, bulletSpread);
        originalKnockback = knockback;
        shotsFired = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 GetDirection(Transform cam, bool BulletSpread)
    {
        Vector3 direction = cam.forward;

        if (BulletSpread)
        {
            if(shotsFired > 3)
            {
                shotsFired = 0;
            }
            else
            {
                direction += new Vector3(Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x), Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y), Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z));
                shotsFired++;
            }
        }

        direction.Normalize();

        return direction;
    }

    public float DamageDealt(GameObject hit, float damage, float weapon, float playerWeight, float enemyWeight, EnemyReferences enemy)
    {
        playerRef = FindAnyObjectByType<PlayerReferences>();
        // gun = 1 fist = 2 scrap = 3

        // If the enemy has been hit (Player Attack)
        if (hit.CompareTag("enemy"))
        {
            // If the player is heavier than the enemy

            if (playerWeight > enemyWeight)
            {
                // If gun
                if (weapon == 1)
                {
                    // Decrease gun damage
                    damage /= 2;
                    enemy.KB = false;
                }
                // If fist
                else if (weapon == 2)
                {
                    // Increase fist damage and make it apply the knockback
                    damage *= 2;
                    enemy.KB = true;
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
            Debug.Log("Calculating player damage fine so far ");
            // If the player is heavier than the enemy
            if (playerWeight > enemyWeight)
            {
                // If gun
                if (weapon == 1)
                {
                    // Decrease gun damage
                    enemy.KB = false;
                }
                // If fist
                else if (weapon == 2)
                {
                    // Increase fist damage and make it apply the knockback
                    enemy.KB = true;
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
        Debug.Log("Player took " + " damage");
        return damage;
    }

    public void Knockback(Rigidbody rb, Transform cam, bool forward, EnemyHealth enemy)
    {
        grapple = FindAnyObjectByType<Grappling>();

        knockback = originalKnockback;

        grapple.StopAllCoroutines();
        grapple.StartReturn();

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

        if (rb.CompareTag("Player"))
        {
            direction.y += .5f;
            Debug.Log("Player taking damage");
        }
        else if (rb.CompareTag("enemy"))
        {
            knockback *= 2f;
            rb.gameObject.transform.position = new Vector3(rb.gameObject.transform.position.x, rb.gameObject.transform.position.y + 0.5f, rb.gameObject.transform.position.z);
            rb.mass = 0.5f;
            Debug.Log("EnemyTakeDamage");
        }
        rb.AddForce(direction * knockback * 1000 * Time.deltaTime, ForceMode.Impulse);
    }

    public void RotateTowards(Transform cam)
    {
        Quaternion rotation = Quaternion.LookRotation(GetDirection(cam, false));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }
}