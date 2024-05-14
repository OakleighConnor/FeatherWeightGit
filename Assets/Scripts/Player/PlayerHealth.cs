using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Scripts")]
    PlayerReferences playerRef;
    HelperScript helper;
    EnemyHealth enemyHealth;
    EnemyReferences enemyRef;

    [Header("Health")]
    public float health;
    public float maxHealth;
    public float displayedHealth;
    public float healthBarSpeed;

    [Header("UI")]
    // Health
    public Slider healthSlider;
    public Image healthBar;
    public TextMeshProUGUI healthPercentage;
    public TextMeshProUGUI weaponSlot1;
    public TextMeshProUGUI weaponSlot2;
    public TextMeshProUGUI weaponSlot3;

    // Colors
    public Color[] colors;
    public float healthColorValue;
    public int lowHealthColor = 0;
    public int midHealthColor = 1;
    public int highHealthColor = 2;


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
        playerRef.weight = health / 100;
        playerRef.weight += 0.5f;

        if(health > maxHealth)
        {
            health = maxHealth;
        }

        UpdateUI();
        UpdateUIColor();
    }
    void UpdateUI()
    {
        float healthUI;
        healthUI = Mathf.Round(health / 250 * 100);

        if (healthUI == 0)
        {
            healthUI = 1;
        }

        // Health Percentage UI
        if (healthUI > 0)
        {
            healthPercentage.text = healthUI.ToString() + "%";
        }
        else
        {
            healthPercentage.text = 0.ToString() + "%";
        }

        // Healthbar
        if (health != displayedHealth)
        {
            if (health - 1f > displayedHealth)
            {
                displayedHealth += Time.deltaTime * healthBarSpeed * 10f;
            }
            else if (health + 1f < displayedHealth)
            {
                displayedHealth -= Time.deltaTime * healthBarSpeed * 10f;
            }
        }

        healthSlider.value = displayedHealth;
    }

    // Healthbar color
    void UpdateUIColor()
    {
        healthColorValue = health / 150;
        if (healthColorValue <= .5f)
        {
            healthPercentage.color = Color.Lerp(colors[lowHealthColor], colors[midHealthColor], healthColorValue);
            weaponSlot1.color = Color.Lerp(colors[lowHealthColor], colors[midHealthColor], healthColorValue);
            weaponSlot2.color = Color.Lerp(colors[lowHealthColor], colors[midHealthColor], healthColorValue);
            weaponSlot3.color = Color.Lerp(colors[lowHealthColor], colors[midHealthColor], healthColorValue);
            healthBar.color = Color.Lerp(colors[lowHealthColor], colors[midHealthColor], healthColorValue);
        }
        else if (healthColorValue <= 1.5f)
        {
            healthPercentage.color = Color.Lerp(colors[midHealthColor], colors[highHealthColor], healthColorValue);
            weaponSlot1.color = Color.Lerp(colors[midHealthColor], colors[highHealthColor], healthColorValue);
            weaponSlot2.color = Color.Lerp(colors[midHealthColor], colors[highHealthColor], healthColorValue);
            weaponSlot3.color = Color.Lerp(colors[midHealthColor], colors[highHealthColor], healthColorValue);
            healthBar.color = Color.Lerp(colors[midHealthColor], colors[highHealthColor], healthColorValue);
        }
        else
        {
            healthPercentage.color = colors[highHealthColor];
            weaponSlot1.color = colors[highHealthColor];
            weaponSlot2.color = colors[highHealthColor];
            weaponSlot3.color = colors[highHealthColor];
            healthBar.color = colors[highHealthColor];
        }
    }

    public void TakeDamage(float damageTaken, bool knockback, GameObject enemy)
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
