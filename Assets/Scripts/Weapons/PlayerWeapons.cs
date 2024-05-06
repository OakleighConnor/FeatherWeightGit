using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    public float shootDelay = 0.5f;


    public float maxBulletDistance = 30;




    [Header("References")]
    PlayerReferences playerRef;
    Gun gunScript;
    Fist fistScript;
    Scrap scrapScript;
    public ParticleSystem ShootingSystem;
    public Transform BulletShootPoint;
    public ParticleSystem ImpactSystem;
    public GameObject weapons;
    public GameObject gun;
    public GameObject scrap;

    [Header("Fist")]
    public GameObject fist;
    public Transform fistDamagePoint;
    public float fistDistance;
    public float damageOffset;
    public bool fistHitbox;
    public bool playerKnockback;

    public bool player;
    public float xOffset;

    [Header("Timer")]
    public float timer;


    [Header("Inptus")]

    public KeyCode primaryKey = KeyCode.Mouse0;
    public KeyCode secondaryKey = KeyCode.Mouse1;
    public float scrollWheel;
    public float weaponValue;
    public bool attacking;

    [Header("Weapons")]

    public WeaponOut weapon;
    public enum WeaponOut
    {
        gun,
        fist,
        scrap,
        dead
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRef = GetComponent<PlayerReferences>();
        gunScript = GetComponent<Gun>();
        fistScript = GetComponent<Fist>();
        scrapScript = GetComponent<Scrap>();


        weapon = WeaponOut.gun;
        attacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon != WeaponOut.dead)
        {
            if (!attacking)
            {
                Inputs();
            }
            Weapons();


            playerRef.anim.speed = 1 / playerRef.weight / 1.5f * 2;

            if (attacking)
            {
                timer += Time.deltaTime;

                if(timer >= 6)
                {
                    attacking = false;
                    timer = 0;
                }
            }
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
            scrap.SetActive(false);
        }

        // Fist
        if (weaponValue == 2)
        {
            weapon = WeaponOut.fist;
            fist.SetActive(true);

            // Disabling other weapons
            gun.SetActive(false);
            scrap.SetActive(false);
        }

        // Scrap
        if (weaponValue == 3)
        {
            weapon = WeaponOut.scrap;
            scrap.SetActive(true);

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

        if (weaponValue >= 4) // One higher than the highest possible value
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
            if (weapon == WeaponOut.fist)
            {
                playerRef.anim.SetTrigger("punch");
                attacking = true;
            }
            else if (weapon == WeaponOut.gun)
            {
                playerRef.anim.SetTrigger("shoot");
                attacking = true;
            }
            else
            {
                
            }
        }

        if (Input.GetKey(primaryKey))
        {
            if (weapon == WeaponOut.scrap)
            {
                scrapScript.ChargeScrap();
            }
        }

        if (Input.GetKeyUp(primaryKey))
        {
            if (weapon == WeaponOut.scrap)
            {
                scrapScript.ThrowScrap();
            }
        }

        // Secondary weapons :

    }

}

