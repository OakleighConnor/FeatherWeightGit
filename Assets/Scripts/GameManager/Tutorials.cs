using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    [Header("Tutorial UI")]
    public GameObject health;
    public bool healthSeen;
    public GameObject grapple;
    public bool grappleSeen;
    public GameObject smoking;
    public bool smokingSeen;
    public GameObject currentTutorial;


    // Start is called before the first frame update
    void Start()
    {
        //health.SetActive(false);
        //grapple.SetActive(false);
        smoking.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTutorial != null && Input.GetKeyDown(KeyCode.E))
        {
            CloseTutorial();
        }
    }

    public void PlayTutorial(GameObject tutorial)
    {
        if (tutorial == health && healthSeen == true) return;
        else if (tutorial == grapple && grappleSeen == true) return;
        else if (tutorial == smoking && smokingSeen == true) return;
        else if (tutorial.activeSelf) return;

        Time.timeScale = 0;
        tutorial.SetActive(true);
        currentTutorial = tutorial;
    }

    public void CloseTutorial()
    {
        Time.timeScale = 1;

        currentTutorial.SetActive(false);

        if (currentTutorial == health)
        {
            healthSeen = true;
        }
        else if (currentTutorial == grapple)
        {
            grappleSeen = true;
        }
        else if (currentTutorial == smoking)
        {
            smokingSeen = true;
        }

        currentTutorial = null;
    }
}
