using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("References")]
    BattleActivate battle;

    [Header("Battle")]
    bool battling = false;
    float enemiesRemaining;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(enemiesRemaining == 0)
        {
            battling = false;
        }
        else
        {
            battling = true;
        }
    }

    public void StartBattle()
    {

    }
}
