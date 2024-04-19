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
    public ParticleSystem ShootingSystem;
    public Transform BulletShootPoint;
    public ParticleSystem ImpactSystem;
    public Transform cam;
    EnemyHealth enemyHealth;
    GameObject objectHit;
    public bool player;


    [Header("Damage")]
    float pistolDamage = 25;
    bool headshot;

    public bool shooting;
   
    [Header("Inptus")]
    public KeyCode primaryKey = KeyCode.Mouse0;
    public KeyCode secondaryKey = KeyCode.Mouse1;

    [Header("Weapons")]

    public WeaponOut weapon;
    public enum WeaponOut
    {
        gun,
        fist,
        scrapo
    }

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovement>();

        weapon = WeaponOut.gun;
        AddBulletSpread = false;
        shooting = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (player)
        {
            Inputs();
            Weapons();
        }
    }

   
    void Weapons()
    {
        if (weapon == WeaponOut.gun)
        {
            knockback = false;
        }
    }

    void Inputs()
    {
        
        // Primary weapons :

        if (Input.GetKeyDown(primaryKey))
        {
            if(weapon == WeaponOut.gun)
            {
                Shoot();
            }
        }

        // Secondary weapons :

    }
    public void Shoot()
    {

        Debug.Log("Shoot");

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
        Debug.Log("Direction");
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
            if (hit.CompareTag("head"))
            {
                headshot = true;
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(DamagePlayerDealt(headshot, pistolDamage), knockback);
            }
            else if (hit.CompareTag("body"))
            {
                headshot = false;
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(DamagePlayerDealt(headshot, pistolDamage), knockback);
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

    float DamagePlayerDealt(bool headshot, float damage)
    {
        if (pm.weight > enemyHealth.weight)
        {
            damage /= 4;
        }
        if (headshot)
        {
            damage *= 2;
        }
        return damage;
    }

    public void ResetGun()
    {
        shooting = false;
    }
}
