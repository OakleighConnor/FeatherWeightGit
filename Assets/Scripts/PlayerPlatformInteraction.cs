using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformInteraction : MonoBehaviour
{
    [Header("Script")]
    public BattleActivate battle;
    PlatformManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindAnyObjectByType<PlatformManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            Debug.Log("Kill the player");

            manager.RespawnPlayer();
        }
        else if (!other.CompareTag("scrap"))
        {
            battle = other.GetComponent<BattleActivate>();
            battle.StartBattle();
        }
    }
}
