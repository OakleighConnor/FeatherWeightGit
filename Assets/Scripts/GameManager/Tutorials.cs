using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorials : MonoBehaviour
{
    [Header("Tutorial UI")]
    public GameObject health;
    public bool healthSeen;
    public GameObject controls;
    public bool controlsSeen;
    public GameObject smoking;
    public bool smokingSeen;
    public GameObject scrap;
    public bool scrapSeen;
    public GameObject currentTutorial;

    // Start is called before the first frame update
    void Start()
    {
        RestartSeenTutorials();
    }

    // Update is called once per frame
    void Update()
    {
        if(!controlsSeen && SceneManager.GetActiveScene().name != "TitleScreen")
        {
            controls.SetActive(true);
            currentTutorial = controls;
        }

        if(currentTutorial != null)
        {
            Time.timeScale = 0;

            if(Input.GetKeyDown(KeyCode.E))
            {
                CloseTutorial();
            }
        }
    }

    public void PlayTutorial(GameObject tutorial)
    {
        if (tutorial == health && healthSeen == true) return;
        else if (tutorial == controls && controlsSeen == true) return;
        else if (tutorial == smoking && smokingSeen == true) return;
        else if (tutorial == scrap && scrapSeen == true) return;
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
            currentTutorial = null;
        }
        else if (currentTutorial == controls)
        {
            controlsSeen = true;
            PlayTutorial(health);
        }
        else if (currentTutorial == smoking)
        {
            smokingSeen = true;
            currentTutorial = null;
        }
        else if (currentTutorial == scrap)
        {
            scrapSeen = true;
            currentTutorial = null;
        }

    }

    public void RestartSeenTutorials()
    {
        healthSeen = false;
        controlsSeen = false;
        smokingSeen = false;
        scrapSeen = false;
    }
}
