using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleActivate : MonoBehaviour
{
    [Header("Scripts")]
    PlatformManager manager;

    [Header("Enemies")]
    public GameObject enemyPrefab;
    public GameObject enemy;
    public float enemies;

    [Header("Spawn Positions")]
    public Transform spawn1;
    bool spawnedAt1 = false;
    public Transform spawn2;
    bool spawnedAt2 = false;
    public Transform spawn3;
    bool spawnedAt3 = false;
    public Transform spawn4;
    bool spawnedAt4 = false;

    [Header("Battling")]
    public bool battleComplete = false;
    bool battling = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindAnyObjectByType<PlatformManager>();

        spawn1 = transform.Find("SpawnA");
        spawn2 = transform.Find("SpawnB");
        spawn3 = transform.Find("SpawnC");
        spawn4 = transform.Find("SpawnD");
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void StartBattle()
    {
        if (battleComplete || manager.battling) return;

        if(gameObject.CompareTag("BattleA"))
        {
            Debug.Log("First battle");
            enemies = 1;
        }
        else
        {
            enemies = Random.Range(1, 4);
        }
        Debug.Log("There are " + enemies + " enemies spawning");

        manager.StartBattle(enemies, GetComponent<BattleActivate>());
        StartCoroutine(SpawnEnemies(enemies));
    }

    public void RestartBattle()
    {
        battling = false;
    }

    IEnumerator SpawnEnemies(float enemies)
    {
        while (enemies > 0)
        {
            Debug.Log(enemies);
            Transform enemySpawnPoint = GetEnemySpawnPoint();

            if(enemySpawnPoint = null)
            {
                enemies = 0;
            }
            else
            {
                Instantiate(enemyPrefab, GetEnemySpawnPoint().position, Quaternion.identity);
                enemies--;
            }
            yield return enemies;
        }
    }

    public Transform GetEnemySpawnPoint()
    {
        float enemySpawnPos = Random.Range(1, 4);

        if (enemySpawnPos == 1)
        {
            return spawn1;
        }
        else if (enemySpawnPos == 2)
        {
            return spawn2;
        }
        else if (enemySpawnPos == 3)
        {
            return spawn3;
        }
        else
        {
            return spawn4;
        }
    }
}
