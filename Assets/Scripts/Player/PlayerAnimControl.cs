using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{

    [Header("References")]
    public GameObject player;
    PlayerReferences playerRef;
    Fist fist;
    Gun gun;
    PlayerWeapons weapon;
    Grappling grapple;

    void Start()
    {
        playerRef = player.GetComponent<PlayerReferences>();
        fist = player.GetComponent<Fist>();
        gun = player.GetComponent<Gun>();
        weapon = FindAnyObjectByType<PlayerWeapons>();
        grapple = FindAnyObjectByType<Grappling>();
    }

    public void Grapple()
    {
        grapple.StartGrappling();
    }

    public void Shoot()
    {
        gun.Shoot(playerRef.cam, false, playerRef.interactableLayers);
    }
    public void PunchHitboxBegin()
    {
        fist.fistHitbox = true;
    }

    public void PunchHitboxStop()
    {
        fist.fistHitbox = false;
    }

    public void ResetAttack()
    {
        weapon.attacking = false;
    }
}
