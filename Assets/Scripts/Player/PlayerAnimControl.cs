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
    Scrap scrap;
    PlayerWeapons weapon;
    Grappling grapple;

    void Start()
    {
        playerRef = player.GetComponent<PlayerReferences>();
        fist = player.GetComponent<Fist>();
        gun = player.GetComponent<Gun>();
        weapon = FindAnyObjectByType<PlayerWeapons>();
        grapple = FindAnyObjectByType<Grappling>();
        scrap = FindAnyObjectByType<Scrap>();
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

    public void ScrapCharge()
    {
        scrap.state = Scrap.State.charging;
    }


    public void ThrowScrap()
    {
        scrap.state = Scrap.State.throwing;
        scrap.ThrowScrap();
    }

    public void ScrapThrowEnd()
    {
        scrap.state = Scrap.State.idle;
        ResetAttack();
    }

    public void ResetAttack()
    {
        weapon.attacking = false;
    }
}
