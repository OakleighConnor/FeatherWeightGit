using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleActivate : MonoBehaviour
{
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
    bool battleComplete = false;
    bool battling = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBattle()
    {
        if (battleComplete || battling) return;

        battling = true;

        if(gameObject.name != "TriggerBattleA")
        {
            Debug.Log("First battle");
            enemies = Random.Range(1, 3);
        }
        else
        {
            enemies = 1;
        }
        StartCoroutine(SpawnEnemies(enemies));
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
                enemy = Instantiate(enemyPrefab, spawn1.position, Quaternion.identity);
                enemies--;
            }
            yield return enemies;
        }
    }

    public Transform GetEnemySpawnPoint()
    {
        float enemySpawnPos = Random.Range(1, 4);

        Transform enemySpawn;

        if (enemySpawnPos == 1)
        {
            if (spawnedAt1)
            {
                spawnedAt1 = false;
                spawnedAt2 = false;
                spawnedAt3 = false;
                spawnedAt4 = false;
                return null;
            }
            else
            {
                enemySpawn = spawn1;
                spawnedAt1 = true;
            }
        }
        else if (enemySpawnPos == 2)
        {
            if (spawnedAt2)
            {
                spawnedAt1 = false;
                spawnedAt2 = false;
                spawnedAt3 = false;
                spawnedAt4 = false;
                return null;
            }
            else
            {
                enemySpawn = spawn2;
                spawnedAt2 = true;
            }
        }
        else if (enemySpawnPos == 3)
        {
            if (spawnedAt3)
            {
                spawnedAt1 = false;
                spawnedAt2 = false;
                spawnedAt3 = false;
                spawnedAt4 = false;
                return null;
            }
            else
            {
                enemySpawn = spawn3;
                spawnedAt3 = true;
            }
        }
        else
        {
            if (spawnedAt4)
            {
                spawnedAt1 = false;
                spawnedAt2 = false;
                spawnedAt3 = false;
                spawnedAt4 = false;
                return null;
            }
            else
            {
                enemySpawn = spawn4;
                spawnedAt4 = true;
            }
        }

        return enemySpawn;
    }
}
