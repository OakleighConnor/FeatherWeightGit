using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{

    [Header("References")]
    public GameObject player;
    Fist fist;
    PlayerWeapons weapon;

    void Start()
    {
        fist = player.GetComponent<Fist>();
        weapon = FindAnyObjectByType<PlayerWeapons>();
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
