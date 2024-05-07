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
    public float healthBarSpeed;

    [Header("UI")]
    public Slider healthSlider;
    public Image healthBar;
    public Color[] colors;
    float targetPoint;
    int currentColorIndex = 0;
    int targetColorIndex = 1;
    public float time;

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
        playerRef.weight /= 1.75f;

        UpdateUIValue();
        UpdateUIColor();
    }

    void UpdateUIValue()
    {
        if (health != displayedHealth)
        {
            if (health - 1f > displayedHealth)
            {
                displayedHealth += Time.deltaTime * healthBarSpeed * 100f;
            }
            else if (health + 1f < displayedHealth)
            {
                displayedHealth -= Time.deltaTime * healthBarSpeed * 100f;
            }
        }

        healthSlider.value = displayedHealth;
    }

    // Make color change depending on health
    void UpdateUIColor()
    {
        targetPoint += Time.deltaTime / time;
        healthBar.color = Color.Lerp(colors[currentColorIndex], colors[targetColorIndex], targetPoint);
        if(targetPoint >= 1f)
        {
            targetPoint = 0f;
            currentColorIndex = targetColorIndex;
            targetColorIndex++;
            if(targetColorIndex == colors.Length)
            {
                targetColorIndex = 0;
            }
        }
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
