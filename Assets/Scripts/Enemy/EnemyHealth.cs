using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // The total max health of the common enemies will be the same as the player (250)
    // The weight of the enemies will be directly proportional to the health.
    // 1 health = 0.5 mass, 250 health = 3 mass
    [Header("Health")]
    public float weight;
    public float health;
    // Start is called before the first frame update
    void Start()
    {
        health = Random.Range(50, 250);
    }

    // Update is called once per frame
    void Update()
    {
        // The calculation used to get the enemy's weight value. Sets the enemy weight to the health and divides it by 100 as .1 mass = 10 health.
        // We add 0.5 on as the player's mass can never be below 0.5 as otherwise the player would move far too quickly
        weight = health / 100 + 0.5f;
    }

    public void Hit(float damageTaken, bool knockback)
    {
        //Debug.Log("Enemy has taken damage!");
        health -= damageTaken;
        //Debug.Log("Enemy took " + damageTaken + " damage");
        //Debug.Log("Enemy has " + health + " health remaining");

        if (health <= 0)
        {
            Death(knockback);
        }
    }

    public void Death(bool knockback)
    {
        //Debug.Log("The enemy has died");
        if (knockback)
        {
            // Start a coroutine that knocks the enemy backwards until it hits something
            // This coroutine will be used for normal knockback + death knockback, they will work differently



        } 
        else
        {
            Explode();
        }
    }

    public void Explode()
    {
        Destroy(this.gameObject);
    }

}
