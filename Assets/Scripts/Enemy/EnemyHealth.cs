using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    EnemyScript enemyScript;
    PlayerMovement player;
    ParticleManager pm;
    public GameObject hitbox;
    
    // The total max health of the common enemies will be the same as the player (250)
    // The weight of the enemies will be directly proportional to the health.
    // 1 health = 0.5 mass, 250 health = 3 mass
    [Header("Health")]
    public float weight;
    public float health;
    public bool kb;
    
    // Start is called before the first frame update
    void Start()
    {
        pm = FindAnyObjectByType<ParticleManager>();
        player = FindAnyObjectByType<PlayerMovement>();
        enemyScript = GetComponent<EnemyScript>();
        health = Random.Range(50, 250);
        kb = false;
    }

    // Update is called once per frame
    void Update()
    {
        // The calculation used to get the enemy's weight value. Sets the enemy weight to the health and divides it by 100 as .1 mass = 10 health.
        // We add 0.5 on as the player's mass can never be below 0.5 as otherwise the player would move far too quickly

        weight = health / 100 + 0.5f;

    }

    public void Hit(float damageTaken, bool knockback, bool playerKnockback)
    {

        kb = knockback;

        //Debug.Log("Enemy has taken damage!");
        health -= damageTaken;
        //Debug.Log("Enemy took " + damageTaken + " damage");
        //Debug.Log("Enemy has " + health + " health remaining");

        if (kb)
        {
            enemyScript.state = EnemyScript.MovementState.knockback;
            enemyScript.Knockback();
        }
        else if (health <= 0)
        {
            Death();
        }
        if (playerKnockback)
        {
            player.Knockback();
        }
    }

    public void Death()
    {
        if (!kb)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Destroy(gameObject);
        pm.Explosion(hitbox.transform);
    }

}
