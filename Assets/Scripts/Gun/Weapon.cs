using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool AddBulletSpread = true; 
    public Vector3 bulletSpreadVariance = new Vector3(1f, 1f, 1f);
    public TrailRenderer BulletTrail;
    public float shootDelay = 0.5f;
    public LayerMask shootable;
    Vector3 bulletDestination;

    float lastShootTime;

    public float maxBulletDistance = 30;



    bool knockback;

    [Header("References")]
    PlayerMovement pm;
    Animator anim;
    public ParticleSystem ShootingSystem;
    public Transform BulletShootPoint;
    public ParticleSystem ImpactSystem;
    public Transform cam;
    EnemyHealth enemyHealth;
    GameObject objectHit;
    public GameObject weapons;
    public GameObject gun;

    [Header("Fist")]
    public GameObject fist;
    public Transform fistDamagePoint;
    public float fistDistance = 1;
    public float damageOffset;
    public bool fistHitbox;

    public bool player;
    public float xOffset;


    [Header("Damage")]
    float pistolDamage = 25;
    float fistDamage = 25;


    [Header("Inptus")]

    public KeyCode primaryKey = KeyCode.Mouse0;
    public KeyCode secondaryKey = KeyCode.Mouse1;
    public float scrollWheel;
    public float weaponValue;

    [Header("Weapons")]

    public WeaponOut weapon;
    public enum WeaponOut
    {
        gun,
        fist,
        scrap
    }

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();

        if(weapons != null)
        {
            anim = weapons.GetComponent<Animator>();
        }

        weapon = WeaponOut.gun;
        AddBulletSpread = false;
        knockback = false;
    }

    // Update is called once per frame
    void Update()
    {

        
        if (player)
        {
            Inputs();
            Weapons();
            Attacks();
        }
    }
    void Attacks()
    {
        if (fistHitbox)
        {
            FistHitbox();
        }
    }
    void Weapons()
    {
        // Gun
        if (weaponValue == 1)
        {
            weapon = WeaponOut.gun;
            gun.SetActive(true);
            
            // Disabling other weapons
            fist.SetActive(false);
            // scrap.SetActive(false);
        }

        // Fist
        if (weaponValue == 2)
        {
            weapon = WeaponOut.fist;
            fist.SetActive(true);

            // Disabling other weapons
            gun.SetActive(false);
            // scrap.SetActive(false);
        }

        // Scrap
        if (weaponValue == 3)
        {
            weapon = WeaponOut.scrap;
            // scrap.SetActive(true);

            // Disabling the other weapons
            gun.SetActive(false);
            fist.SetActive(false);
        }
    }

    void Inputs()
    {
        // Scroll wheel input :

        weaponValue += Input.mouseScrollDelta.y;

        // Checks if weaponValue has ascended out of the range that it is given
        // If so then it loops the values

        if(weaponValue >= 4) // One higher than the highest possible value
        {
            weaponValue = 1;
        }
        else if (weaponValue <= 0) // One lower than the lowest possible value
        {
            weaponValue = 3;
        }

        // Primary weapon input :

        if (Input.GetKeyDown(primaryKey))
        {
            if(weapon == WeaponOut.gun)
            {
                Shoot();
            }
            else if (weapon == WeaponOut.fist)
            {
                Punch();
            }
        }

        // Secondary weapons :

    }

    public void Punch()
    {
        anim.SetTrigger("punch");
    }

    public void FistHitbox()
    {
        Vector3 direction = GetDirection(cam);

        if (Physics.Raycast(fistDamagePoint.position, direction + new Vector3(0.3f, 0, 0), out RaycastHit hit1, fistDistance, shootable))
        {
            objectHit = hit1.collider.gameObject;
        }
        else if (Physics.Raycast(fistDamagePoint.position, direction, out RaycastHit hit2, fistDistance, shootable))
        {
            objectHit = hit2.collider.gameObject;
        }
        else if (Physics.Raycast(fistDamagePoint.position, direction + new Vector3(-0.3f, 0, 0), out RaycastHit hit3, fistDistance, shootable))
        {
            objectHit = hit3.collider.gameObject;
        }
        else
        {
            objectHit = null;
        }

        Debug.DrawRay(fistDamagePoint.position, direction * fistDistance, Color.green, 3);
        Debug.DrawRay(fistDamagePoint.position, direction + new Vector3(0.3f, 0, 0) * fistDistance, Color.red, 3);
        Debug.DrawRay(fistDamagePoint.position, direction + new Vector3(-0.3f, 0, 0) * fistDistance, Color.blue, 3);

        if (objectHit != null)
        {
            if (objectHit.CompareTag("enemy"))
            {
                fistHitbox = false;
                enemyHealth = objectHit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(DamagePlayerDealt(fistDamage, 2), knockback);
            }
        }
    }

    public void Shoot()
    {
        ShootingSystem.Play();
        Vector3 direction = GetDirection(cam);


        if (Physics.Raycast(cam.position, direction, out RaycastHit hit, float.MaxValue, shootable))
        {
            bulletDestination = hit.point;

            lastShootTime = Time.time;

            objectHit = hit.collider.gameObject;
        }
        else
        {
            bulletDestination = cam.position + cam.forward * maxBulletDistance;
            objectHit = null;
        }

        TrailRenderer trail = Instantiate(BulletTrail, BulletShootPoint.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(trail, objectHit,bulletDestination));
    }
    Vector3 GetDirection(Transform cam)
    {
        Vector3 direction = cam.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x), Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y), Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z));
        }

        direction.Normalize();

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, GameObject hit ,Vector3 hitPos)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, bulletDestination, time);
            time += Time.deltaTime / trail.time;

            yield return null; 
        }
        trail.transform.position = hitPos;
        Instantiate(ImpactSystem, hitPos, Quaternion.LookRotation(hitPos));

        if (hit != null)
        {
            if (hit.CompareTag("enemy"))
            {
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(DamagePlayerDealt(pistolDamage, 1), knockback);
            }
            else if (hit.CompareTag("Player"))
            {
                Debug.Log("player hit!");
            }
            else
            {
                Debug.Log("no enemy hit");
            }
        }
        

        Destroy(trail.gameObject, trail.time);
    }

    float DamagePlayerDealt(float damage, float weapon)
    {
        // gun = 1 fist = 2 scrap = 3

        // If the player is heavier than the enemy
        if(weapon == 2)
        {
            Debug.Log("fist");
        }
        if (pm.weight > enemyHealth.weight)
        {
            if(weapon == 1)
            {
                // Decrease gun damage
                damage /= 4;
            }
            else if (weapon == 2)
            {
                Debug.Log("knockback!");
                // Increase fist damage and make it apply the knockback
                damage *= 2;
                knockback = true;
            }
        }
        return damage;
    }

}
