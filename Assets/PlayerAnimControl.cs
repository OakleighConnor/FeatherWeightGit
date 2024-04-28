using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{

    [Header("References")]
    public Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PunchHitboxBegin()
    {
        weapon.fistHitbox = true;
    }

    public void PunchHitboxStop()
    {
        weapon.fistHitbox = false;
    }
}
