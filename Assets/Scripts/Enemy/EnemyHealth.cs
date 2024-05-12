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

    [Header("References")]
    public GameObject hitbox;
    ParticleSystem smoke;
    
    // The total max health of the common enemies will be the same as the player (250)
    // The weight of the enemies will be directly proportional to the health.
    // 1 health = 0.5 mass, 250 health = 3 mass
    [Header("Health")]
    public float weight;
    public float health;
    public bool kb;
    public bool lighter;

    [Header("Scrap")]
    public GameObject scrap;
    public GameObject scrapPrefab;
    public Rigidbody scrapRB;
    public float scrapSpeed;
    public Vector3 scrapDirection;
    Quaternion rotation;
    public float scrapAmount;

    // Start is called before the first frame update
    void Start()
    {
        // Enemy referencess
        healthScript = GetComponent<EnemyHealth>();
        enemyScript = GetComponent<EnemyScript>();

        // Player references
        playerRef = FindAnyObjectByType<PlayerReferences>();

        // Exterior references
        pm = FindAnyObjectByType<ParticleManager>();
        helper = FindAnyObjectByType<HelperScript>();

        // Variable assignment
        health = Random.Range(50, 250);
        kb = false;
    }
    void Update()
    {
        // The calculation used to get the enemy's weight value. Sets the enemy weight to the health and divides it by 100 as .1 mass = 10 health.
        // We add 0.5 on as the player's mass can never be below 0.5 as otherwise the player would move far too quickly
        weight = health / 100 + 0.5f;
        weight /= 1.75f;

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
                smoke = Instantiate(pm.smoke, hitbox.transform.position, transform.rotation);
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

    public void Hit(float damageTaken, bool knockback, bool playerKnockback)
    {
        kb = knockback;

        //Debug.Log("Enemy has taken damage!");
        health -= damageTaken;
        //Debug.Log("Enemy took " + damageTaken + " damage");
        //Debug.Log("Enemy has " + health + " health remaining");

        if (lighter)
        {
            enemyScript.state = EnemyScript.MovementState.knockback;
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
        if (!kb)
        {
            Explode();
        }
    }

    public void Explode()
    {
        //StartCoroutine(Drops());
        Vector3 scrapPos = transform.position;
        scrapPos.y += 2;
        scrap = Instantiate(scrapPrefab, scrapPos, Quaternion.identity);
        
        Destroy(gameObject);
        pm.Explosion(hitbox.transform);

        if (smoke != null)
        {
            Destroy(smoke);
        }
    }

    public IEnumerator Drops()
    {
        while (scrapAmount >= 0)
        {
            Debug.Log(scrapAmount);
            scrap = Instantiate(scrapPrefab, transform.position, Quaternion.identity);
            scrapRB = scrap.GetComponentInParent<Rigidbody>();

            scrapDirection = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));

            //scrapSpeed = Random.Range(1, 3);

            rotation = Quaternion.LookRotation(scrapDirection);
            scrap.transform.rotation = Quaternion.Slerp(scrap.transform.rotation, rotation, 0.2f);
            scrapRB.AddForce(scrap.transform.forward.normalized * scrapSpeed * 1000f * Time.deltaTime, ForceMode.Force);

            scrapAmount = scrapAmount - 1;

            yield return null;
        }
    }
}
