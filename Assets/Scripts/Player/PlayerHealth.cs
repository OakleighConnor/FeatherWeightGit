using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Scripts")]
    PlayerReferences playerRef;
    HelperScript helper;
    EnemyHealth enemyHealth;
    EnemyReferences enemyRef;

    [Header("Health")]
    public float health;
    public float displayedHealth;

    [Header("UI")]
    public Slider healthSlider;
    // Start is called before the first frame update
    void Start()
    {
        playerRef = GetComponent<PlayerReferences>();
        helper = FindAnyObjectByType<HelperScript>();

        displayedHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        playerRef.weight = health / 100 + 0.5f;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (health != displayedHealth)
        {
            if (health - 1f > displayedHealth)
            {
                displayedHealth += Time.deltaTime * 100f;
            }
            else if (health + 1f < displayedHealth)
            {
                displayedHealth -= Time.deltaTime * 100f;
            }
        }

        healthSlider.value = displayedHealth;

        

    }

    public void TakeDamage(float damageTaken, bool knockback, bool forward, GameObject enemy)
    {
        Debug.Log("damaged player");

        enemyRef = enemy.GetComponent<EnemyReferences>();
        enemyHealth = enemy.GetComponent<EnemyHealth>();

        health -= damageTaken;

        if(health <= 0)
        {
            Death();
        }
        else
        {
            if (knockback)
            {
                helper.Knockback(playerRef.rb, enemyRef.cam, true, enemyHealth);
            }
        }
    }

    void Death()
    {
        Debug.Log("Player Dead");
        helper.playerAlive = false;
    }
}
