using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [Header("References")]
    BattleActivate battle;

    [Header("Battle")]
    public bool battling = false;
    float enemiesRemaining;
    BattleActivate currentBattle;

    [Header("Level")]

    public int levelPart;

    public GameObject transition1;
    public GameObject arena1;
    public GameObject transition2;
    public GameObject arena2;
    public GameObject transition3;
    public GameObject arena3;
    public GameObject transition4;
    public GameObject arena4;
    public GameObject transition5;
    public GameObject arena5;


    // Start is called before the first frame update
    void Start()
    {
        currentBattle = null;
        levelPart = 1;

        transition1 = GameObject.FindGameObjectWithTag("T1").transform.parent.gameObject.transform.parent.gameObject;
        transition2 = GameObject.FindGameObjectWithTag("T2").transform.parent.gameObject.transform.parent.gameObject;
        //transition3 = GameObject.FindGameObjectWithTag("T3").transform.parent.gameObject.transform.parent.gameObject;
        //transition4 = GameObject.FindGameObjectWithTag("T4").transform.parent.gameObject.transform.parent.gameObject;
        //transition5 = GameObject.FindGameObjectWithTag("T5").transform.parent.gameObject.transform.parent.gameObject;

        arena1 = GameObject.FindWithTag("A1");
        arena2 = GameObject.FindWithTag("A2");
        //arena3 = GameObject.FindWithTag("A3");
        //arena4 = GameObject.FindWithTag("A4");
        //arena5 = GameObject.FindWithTag("A5");
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

        PlatformEnabler();

        
    }

    public void PlatformEnabler()
    {

        // Transition platforms
        if (battling)
        {
            transition1.SetActive(false);
            transition2.SetActive(false);
            //transition3.SetActive(false);
            //transition4.SetActive(false);
            //transition5.SetActive(false);
        }
        else
        {
            if (levelPart == 1)
            {
                transition1.SetActive(true);
            }
            else if (levelPart == 2)
            {
                transition2.SetActive(true);
            }
        }

        // Arena
        if (levelPart == 1 || levelPart == 2)
        {
            arena1.SetActive(true);
        }
        else
        {
            arena1.SetActive(false);
        }

        if (levelPart == 2 || levelPart == 3)
        {
            arena2.SetActive(true);
        }
        else
        {
            arena2.SetActive(false);
        }

    }

    public void StartBattle(float enemies, BattleActivate battle)
    {
        enemiesRemaining = enemies;
        currentBattle = battle;
    }

    public void EnemySlain()
    {
        if (enemiesRemaining == 0) return;

        enemiesRemaining--;

        if(enemiesRemaining == 0)
        {
            battling = false;
            currentBattle.battleComplete = true;
            currentBattle = null;
            Debug.Log("Battle Complete!");
            levelPart++;
        }
    }
}
