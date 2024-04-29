using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{

    [Header("References")]
    public Weapon weapon;

    public void PunchHitboxBegin()
    {
        weapon.fistHitbox = true;
    }

    public void PunchHitboxStop()
    {
        weapon.fistHitbox = false;
    }

    public void ResetAttack()
    {
        weapon.attacking = false;
    }
}
