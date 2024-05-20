using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public KeyCode weaponSlot1 = KeyCode.Alpha1;
    public KeyCode weaponSlot2 = KeyCode.Alpha2;
    public KeyCode weaponSlot3 = KeyCode.Alpha3;
    public float scrollWheel;
    public float weaponValue;
    public float lastWeaponValue;
    public bool attacking;

    [Header("UI")]
    public Image gunIcon;
    public Image fistIcon;
    public Image scrapIcon;

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
        attacking = true;
        timer = 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (weapon != WeaponOut.dead)
        {
            Weapons();
            Inputs();

            playerRef.anim.speed = 1 / playerRef.weight / 1.5f * 2;

            if (attacking)
            {
                timer += Time.deltaTime;

                if(timer >= 5)
                {
                    attacking = false;
                    timer = 0;
                }
            }
        }

    }
    void Weapons()
    {
        if(lastWeaponValue != weaponValue)
        {
            attacking = false;
        }

        lastWeaponValue = weaponValue;

        // Gun
        if (weaponValue == 1)
        {
            weapon = WeaponOut.gun;
            gun.SetActive(true);
            gunIcon.enabled = true;

            // Disabling other weapons
            fist.SetActive(false);
            fistIcon.enabled = false;

            scrap.SetActive(false);
            scrapIcon.enabled = false;
        }

        // Fist
        if (weaponValue == 2)
        {
            weapon = WeaponOut.fist;
            fist.SetActive(true);
            fistIcon.enabled = true;

            // Disabling other weapons
            gun.SetActive(false);
            gunIcon.enabled = false;

            scrap.SetActive(false);
            scrapIcon.enabled = false;
        }

        // Scrap
        if (weaponValue == 3)
        {
            if(scrapScript.state != Scrap.State.throwing)
            {
                scrap.SetActive(true);
            }
            else
            {
                scrap.SetActive(false);
            }

            weapon = WeaponOut.scrap;
            scrapIcon.enabled = true;

            // Disabling the other weapons
            gun.SetActive(false);
            gunIcon.enabled = false;

            fist.SetActive(false);
            fistIcon.enabled = false;

            playerRef.anim.SetBool("scrap", true);
        }
        else
        {
            scrapScript.state = Scrap.State.idle;
            playerRef.anim.SetBool("scrap", false);
            playerRef.anim.SetBool("scrapCharge", false);
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

        if (Input.GetKeyDown(weaponSlot1))
        {
            weaponValue = 1;
        }
        else if (Input.GetKeyDown(weaponSlot2))
        {
            weaponValue = 2;
        }
        else if (Input.GetKeyDown(weaponSlot3))
        {
            weaponValue = 3;
        }



        // Primary weapon input :

        if (!attacking)
        {
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
            }
        }

        if (Input.GetKeyDown(primaryKey) && scrapScript.state == Scrap.State.idle && weapon == WeaponOut.scrap)
        {
            playerRef.anim.SetBool("scrapCharge", true);
        }

        // Scrap
        if (Input.GetKey(primaryKey))
        {
            if (weapon == WeaponOut.scrap && playerRef.weight >= 1)
            {
                scrapScript.ChargeScrap();
                attacking = true;
            }
        }
        if (Input.GetKeyUp(primaryKey))
        {
            if (weapon == WeaponOut.scrap)
            {
                playerRef.anim.SetBool("scrapCharge", false);
            }
        }
    }

}

