using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scrap : MonoBehaviour
{
    [Header("Scripts")]
    HelperScript helper;
    EnemyHealth enemyHealth;
    PlayerHealth playerHealth;
    PlayerReferences playerRef;

    [Header("References")]
    public GameObject scrapProjectile;
    public Transform shootPoint;
    GameObject projectile;

    [Header("UI")]
    public Slider scrapSlider;
    public float scrapCharge;

    [Header("Damage")]
    public float chargeSpeed;
    public float damage;
    public float projectileSpeed;

    [Header("Raycast")]
    GameObject objectHit;
    Vector3 destination;
    float maxDistance = 1000;
    public bool inRange;

    [Header("States")]
    public State state;
    public bool charging;

    public enum State
    {
        idle,
        charging,
        throwing
    }

    // Start is called before the first frame update
    void Start()
    {
        helper = FindAnyObjectByType<HelperScript>();
        playerHealth = GetComponent<PlayerHealth>();
        playerRef = GetComponent<PlayerReferences>();

        charging = false;
    }

    // Update is called once per frame
    void Update()
    {
        scrapSlider.value = scrapCharge;

        StateManager();

        if (projectile != null)
        {
            inRange = Vector3.Distance(projectile.transform.position, destination) <= 1f;
        }
        else
        {
            inRange = false;
        }
    }

    void StateManager()
    {
        if (state != State.charging)
        {
            scrapCharge = 0;
        }
    }
    public void ChargeScrap()
    {
        state = State.charging;

        Debug.Log("ChargingScrap");

        if(scrapCharge <= playerHealth.health - 1)
        {
            scrapCharge += Time.deltaTime * chargeSpeed * 10;
        }
        damage = scrapCharge;
    }

    public void ThrowScrap()
    {
        state = State.throwing;
        playerHealth.health -= damage;

        Vector3 direction = helper.GetDirection(playerRef.cam, false);

        if (Physics.Raycast(playerRef.cam.position, direction, out RaycastHit hit, float.MaxValue, playerRef.interactableLayers))
        {
            destination = hit.point;

            objectHit = hit.collider.gameObject;
        }
        else
        {
            destination = playerRef.cam.position + playerRef.cam.forward * maxDistance;
            objectHit = null;
        }

        projectile = Instantiate(scrapProjectile, shootPoint.position, Quaternion.identity);
        StartCoroutine(LaunchScrap(projectile, objectHit, destination));
    }

    private IEnumerator LaunchScrap(GameObject projectile, GameObject hit, Vector3 hitPos)
    {
        // Move the grapple towards the grappled position
        while (!inRange)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, hitPos, projectileSpeed * 100 * Time.deltaTime);

            yield return null;
        }



        if(hit != null)
        {
            if (hit.CompareTag("enemy"))
            {
                enemyHealth = hit.GetComponentInParent<EnemyHealth>();
                enemyHealth.Hit(helper.DamageDealt(objectHit, damage, 1, playerRef.weight, enemyHealth.weight, objectHit.GetComponentInParent<EnemyReferences>()), false, false);
            }
        }

        Destroy(projectile);
    }
}
