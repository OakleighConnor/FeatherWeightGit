using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlatformManager : MonoBehaviour
{
    [Header("Scripts")]
    BattleActivate battle;
    HelperScript helper;
    Grappling grapple;
    PlayerHealth playerHealth;

    [Header("References")]
    public GameObject player;
    public GameObject[] scrap;
    public float activeScrap = 0;

    [Header("Battle")]
    public bool battling = false;
    public float enemiesRemaining;
    BattleActivate currentBattle;

    [Header("Level")]
    public int levelPart;
    public GameObject spawnPlatform;
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

    [Header("Checkpoints")]
    public GameObject currentCheckpoint;
    public GameObject checkpoint1;
    public GameObject checkpoint2;
    public GameObject checkpoint3;
    public GameObject checkpoint4;
    public GameObject checkpoint5;


    // Start is called before the first frame update
    void Start()
    {
        helper = GetComponent<HelperScript>();

        currentBattle = null;
        levelPart = 1;

        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        grapple = player.GetComponent<Grappling>();

        // Assigning all platforms that are used in the levels

        // Passive platforms
        spawnPlatform = GameObject.FindGameObjectWithTag("Spawn");

        transition1 = GameObject.FindGameObjectWithTag("T1").transform.root.gameObject;
        transition2 = GameObject.FindGameObjectWithTag("T2").transform.root.gameObject;
        transition3 = GameObject.FindGameObjectWithTag("T3").transform.root.gameObject;
        transition4 = GameObject.FindGameObjectWithTag("T4").transform.root.gameObject;
        transition5 = GameObject.FindGameObjectWithTag("T5").transform.root.gameObject;

        // Combat platforms
        arena1 = GameObject.FindGameObjectWithTag("A1");
        arena2 = GameObject.FindGameObjectWithTag("A2");
        arena3 = GameObject.FindGameObjectWithTag("A3");
        arena4 = GameObject.FindWithTag("A4");
        arena5 = GameObject.FindWithTag("A5");

        // Checkpoints
        checkpoint1 = GameObject.FindGameObjectWithTag("C1");
        checkpoint2 = GameObject.FindGameObjectWithTag("C2");
        checkpoint3 = GameObject.FindGameObjectWithTag("C3");
        checkpoint4 = GameObject.FindGameObjectWithTag("C4");
        checkpoint5 = GameObject.FindGameObjectWithTag("C5");

        // Sets all of the platforms to false except the platform the player first spawns on to prevent the player skipping through the level
        spawnPlatform.SetActive(true);
        transition2.SetActive(false);
        transition3.SetActive(false);
        transition4.SetActive(false);
        transition5.SetActive(false);
        arena2.SetActive(false);
        arena3.SetActive(false);
        arena4.SetActive(false);
        arena5.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If enemies are alive then battling is set to true

        LevelInfo();

        
    }

    public void LevelInfo()
    {

        // Transition platforms
        if (battling)
        {
            spawnPlatform.SetActive(false);
            transition1.SetActive(false);
            transition2.SetActive(false);
            transition3.SetActive(false);
            transition4.SetActive(false);
            transition5.SetActive(false);

            if(levelPart == 2)
            {
                arena1.SetActive(false);
            }
            else if (levelPart == 3)
            {
                arena2.SetActive(false);
            }
            else if (levelPart == 4)
            {
                arena3.SetActive(false);
            }
            else if (levelPart == 5)
            {
                arena4.SetActive(false);
            }
        }
        else
        {
            if (levelPart == 1)
            {
                spawnPlatform.SetActive(true);
                transition1.SetActive(true);
            }
            else if (levelPart == 2)
            {
                transition2.SetActive(true);
            }
            else if (levelPart == 3)
            {
                transition3.SetActive(true);
            }
            else if (levelPart == 4)
            {
                transition4.SetActive(true);
            }
            else if (levelPart == 5)
            {
                transition5.SetActive(true);
            }
        }

        // Combat platforms
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

        if (levelPart == 3 || levelPart == 4)
        {
            arena3.SetActive(true);
        }
        else
        {
            arena3.SetActive(false);
        }

        if (levelPart == 4 || levelPart == 5)
        {
            arena4.SetActive(true);
        }
        else
        {
            arena4.SetActive(false);
        }

        if (levelPart == 5 || levelPart == 6)
        {
            arena5.SetActive(true);
        }
        else
        {
            arena5.SetActive(false);
        }
        // Checkpoint management
        if (levelPart == 1)
        {
            currentCheckpoint = checkpoint1;
        }
        else if (levelPart == 2)
        {
            currentCheckpoint = checkpoint2;
        }
        else if (levelPart == 3)
        {
            currentCheckpoint = checkpoint3;
        }
        else if (levelPart == 4)
        {
            currentCheckpoint = checkpoint4;
        }
        else if (levelPart == 5)
        {
            currentCheckpoint = checkpoint5;
        }
    }

    public void StartBattle(float enemies, BattleActivate battle)
    {
        enemiesRemaining = enemies;
        currentBattle = battle;
    }

    public void NextLevel()
    {
        if (enemiesRemaining == 0)
        {
            battling = false;
            currentBattle.battleComplete = true;
            currentBattle = null;
            Debug.Log("Battle Complete!");
            levelPart++;
        }
    }
    public void RespawnPlayer()
    {
        scrap = GameObject.FindGameObjectsWithTag("scrap");
        foreach (GameObject target in scrap)
        Destroy(target);
        playerHealth.health = 125;
        
        player.transform.position = currentCheckpoint.transform.position;
        battling = false;
        enemiesRemaining = 0;

        if(currentBattle != null)
        {
            currentBattle = null;
        }

        Debug.Log("Player respawned at " + currentCheckpoint);
    }
}
