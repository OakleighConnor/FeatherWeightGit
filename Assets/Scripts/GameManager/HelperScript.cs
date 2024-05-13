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

    GameObject player;
    public bool playerAlive;

    public LayerMask shootable;

    [Header("Knockback")]
    public float knockback;
    float originalKnockback;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerRef = FindAnyObjectByType<PlayerReferences>();
        weapon = FindAnyObjectByType<PlayerWeapons>();
        player = GameObject.FindWithTag("Player");

        bulletSpreadVariance = new Vector3(bulletSpread, bulletSpread, bulletSpread);
        playerAlive = true;
        originalKnockback = knockback;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerAlive)
        {
            player.SetActive(false);
            weapon.weapon = PlayerWeapons.WeaponOut.dead;
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

    public float DamageDealt(GameObject hit, float damage, float weapon, float playerWeight, float enemyWeight, EnemyReferences enemy)
    {

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
            direction.y += 2f;
            Debug.Log("Player taking damage");
        }
        else if (rb.CompareTag("enemy"))
        {
            knockback *= 2;
            Debug.Log("EnemyTakeDamage");
        }

        rb.AddForce(direction * knockback * 10000 * Time.deltaTime, ForceMode.Impulse);
    }

    public void RotateTowards(Transform cam)
    {
        Quaternion rotation = Quaternion.LookRotation(GetDirection(cam, false));
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }
}