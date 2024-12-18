using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtons : MonoBehaviour
{
    [Header("Scripts")]
    AudioManager am;
    PauseMenu pm;
    public Tutorials tutorial;

    [Header("Settings")]
    public GameObject settingsMenu;
    public GameObject title;
    public bool settings;

    void Start()
    {
        settingsMenu = GameObject.FindGameObjectWithTag("Settings").transform.GetChild(0).gameObject;
        settingsMenu.SetActive(false);
        tutorial = FindAnyObjectByType<Tutorials>();
    }

    private void Update()
    {
        settingsMenu = GameObject.FindGameObjectWithTag("Settings").transform.GetChild(0).gameObject;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        tutorial = FindAnyObjectByType<Tutorials>();
        tutorial.RestartSeenTutorials();

        // Reload the scene and resume time
        Time.timeScale = 1;

        
    }
    public void ChangeScene()
    {
        am = GetComponent<AudioManager>();
        am.PlaySFX(am.inputUI);
        if (SceneManager.GetActiveScene().name == "1-1")
        {
            Debug.Log("Main Menu");
            SceneManager.LoadScene("TitleScreen");
        }
        else
        {
            SceneManager.LoadScene("1-1");
        }
    }
    public void ToggleSettings()
    {
        am = GetComponent<AudioManager>();
        am.PlaySFX(am.inputUI);
        settingsMenu = GameObject.FindGameObjectWithTag("Settings").transform.GetChild(0).gameObject;

        if (SceneManager.GetActiveScene().name == "TitleScreen")
        {
            title = GameObject.FindGameObjectWithTag("TitleScreen").transform.GetChild(0).gameObject;
        }
        else
        {
            pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PauseMenu>();
        }

        if (settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            if(SceneManager.GetActiveScene().name == "TitleScreen")
            {
                title.SetActive(true);
            }
            else
            {
                pm.pauseMenu.SetActive(true);
            }
            settings = false;
        }
        else
        {
            settingsMenu.SetActive(true);
            if (SceneManager.GetActiveScene().name == "TitleScreen")
            {
                title.SetActive(false);
            }
            else
            {
                pm.pauseMenu.SetActive(false);
            }
            settings = true;
        }
    }

    public void TogglePause()
    {
        am = GetComponent<AudioManager>();
        Debug.Log(am);
        am.PlaySFX(am.inputUI);
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PauseMenu>();
        pm.TogglePause();
    }
}
