using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformInteraction : MonoBehaviour
{
    [Header("Script")]
    BattleActivate battle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("collectable"))
        {
            battle = other.GetComponent<BattleActivate>();
            battle.StartBattle();
        }
    }
}
