using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Scripts")]
    EnemyScript enemyScript;
    PlayerReferences playerRef;
    HelperScript helper;
    ParticleManager pm;
    EnemyHealth healthScript;
    PlatformManager manager;
    AudioManager am;
    Tutorials tutorial;

    [Header("References")]
    public GameObject hitbox;
    public ParticleSystem smoke;
    
    // The total max health of the common enemies will be the same as the player (250)
    // The weight of the enemies will be directly proportional to the health.
    // 1 health = 0.5 mass, 250 health = 3 mass
    [Header("Health")]
    public float weight;
    public float health;
    public bool kb;
    public bool lighter;
    public bool dead;

    [Header("Scrap")]
    public GameObject scrapPrefab;
    public float scrapSpeed;
    public Vector3 scrapDirection;
    public float scrapAmount;

    // Start is called before the first frame update
    void Start()
    {
        // Enemy referencess
        healthScript = GetComponent<EnemyHealth>();
        enemyScript = GetComponent<EnemyScript>();

        // Player references
        playerRef = FindAnyObjectByType<PlayerReferences>();

        // Level references
        manager = FindAnyObjectByType<PlatformManager>();

        // Exterior references
        pm = FindAnyObjectByType<ParticleManager>();
        helper = FindAnyObjectByType<HelperScript>();
        am = FindAnyObjectByType<AudioManager>();

        tutorial = FindAnyObjectByType<Tutorials>();

        // Variable assignment
        health = Random.Range(150, 250);
        kb = false;
    }
    void Update()
    {
        // The calculation used to get the enemy's weight value. Sets the enemy weight to the health and divides it by 100 as .1 mass = 10 health.
        // We add 0.5 on as the player's mass can never be below 0.5 as otherwise the player would move far too quickly
        weight = health / 100;
        weight += 0.5f;

        // Health check

        if (health <= 0 && enemyScript.state != EnemyScript.MovementState.knockback)
        {
            Death();
        }

        // Smoke 

        if (weight < playerRef.weight)
        {
            lighter = true;
        }
        else
        {
            lighter = false;
        }

        if (smoke == null)
        {
            if (lighter)
            {
                tutorial.PlayTutorial(tutorial.smoking);
                smoke = Instantiate(pm.smoke);
                smoke.transform.rotation = Quaternion.Euler(-90, 0, 0);
            }
        }
        else
        {
            if (lighter)
            {
                smoke.transform.position = hitbox.transform.position;
            }
            else
            {
                Destroy(smoke);
            }
            smoke.transform.position = hitbox.transform.position;
        }
    }

    void LateUpdate()
    {
        if (health <= 0 && enemyScript.state != EnemyScript.MovementState.knockback)
        {
            Destroy(gameObject);
        }
    }

    public void Hit(float damageTaken, bool knockback, bool playerKnockback)
    {
        am.PlaySFX(am.enemyDamage);

        kb = knockback;

        //Debug.Log("Enemy has taken damage!");
        health -= damageTaken;
        //Debug.Log("Enemy took " + damageTaken + " damage");
        //Debug.Log("Enemy has " + health + " health remaining");

        if (lighter && kb)
        {
            enemyScript.state = EnemyScript.MovementState.knockback;
            Vector3 knockbackPrepPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            transform.position = knockbackPrepPos;
            enemyScript.TakeKnockback(true);
        }
        else if (playerKnockback && !lighter)
        {
            helper.Knockback(playerRef.rb, playerRef.cam, false, healthScript);
        }
        else if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        if (dead) return;

        dead = true;

        am.PlaySFX(am.explosion);

        Debug.Log("scrap spawned");
        Vector3 scrapPos = transform.position;
        scrapPos.y += 2;

        pm.Explosion(hitbox.transform);

        if (smoke != null)
        {
            Destroy(smoke);
        }

        manager.activeScrap++;
        Instantiate(scrapPrefab, scrapPos, Quaternion.identity);

        if (manager.enemiesRemaining == 0) return;

        manager.enemiesRemaining--;

        manager.NextLevel();
    }


}
