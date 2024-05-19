using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Scripts")]
    PlayerMovement player;
    PlayerWeapons weapon;
    PlayerReferences playerRef;
    EnemyReferences enemyRef;
    HelperScript helper;
    ParticleManager effects;
    EnemyHealth enemyHealth;
    PlayerHealth playerHealth;
    AudioManager am;

    [Header("Gun")]
    public Transform BulletShootPoint;
    public float pistolDamage;
    public float shootDistance = 250;
    bool bulletSpread;

    [Header("Bullet")]
    Vector3 bulletDestination;
    public GameObject objectHit;


    // Start is called before the first frame update
    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        player = FindAnyObjectByType<PlayerMovement>();
        playerRef = FindAnyObjectByType<PlayerReferences>();
        weapon = FindAnyObjectByType<PlayerWeapons>();
        am = FindAnyObjectByType<AudioManager>();

        if (CompareTag("enemy"))
        {
            enemyRef = GetComponent<EnemyReferences>();
            enemyHealth = GetComponent<EnemyHealth>();
        }

        // Components in other game objects
        helper = FindAnyObjectByType<HelperScript>();
        effects = FindAnyObjectByType<ParticleManager>();
    }


    public void EnemyShoot()
    {
        bulletSpread = true;
        enemyRef = GetComponent<EnemyReferences>();
        Shoot(enemyRef.cam, bulletSpread, enemyRef.interactableLayers);
    }

    public void Shoot(Transform cam, bool bulletSpread, LayerMask shootable)
    {
        helper = FindAnyObjectByType<HelperScript>();
        am = FindAnyObjectByType<AudioManager>();
        am.PlaySFX(am.shoot);
        effects.ShootingSystem.Play();
        Debug.Log(helper);
        Debug.Log(cam);
        Debug.Log(bulletSpread);
        Vector3 direction = helper.GetDirection(cam, bulletSpread);

        if (Physics.Raycast(cam.position, direction, out RaycastHit hit, shootDistance, shootable))
        {
            bulletDestination = hit.point;

            objectHit = hit.collider.gameObject;
        }
        else
        {
            bulletDestination = cam.position + cam.forward * shootDistance;
            objectHit = null;
        }

        TrailRenderer trail = Instantiate(effects.BulletTrail, BulletShootPoint.position, Quaternion.identity);

        StartCoroutine(SpawnTrail(trail, objectHit, bulletDestination));
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, GameObject hit, Vector3 hitPos)
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
        Instantiate(effects.impactSystem, hitPos, Quaternion.LookRotation(hitPos));

        if (hit != null)
        {
            if (hit.CompareTag("enemy"))
            {
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(helper.DamageDealt(objectHit, pistolDamage, 1, playerRef.weight, enemyHealth.weight, objectHit.GetComponentInParent<EnemyReferences>()), false, false);
            }
            else if (objectHit.name == "PlayerObj")
            {
                Debug.Log("Player hit");
                playerHealth.TakeDamage(helper.DamageDealt(objectHit, pistolDamage, 1, playerRef.weight, enemyHealth.weight, GetComponent<EnemyReferences>()), false, gameObject);
            }
            else
            {
                // Nothing interactable hit
            }
        }
        Destroy(trail.gameObject, trail.time);
    }
}
