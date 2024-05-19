using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Scripts")]
    public UIButtons ui;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public bool paused;

    [Header("HUD")]
    public GameObject HUD;

    void Awake()
    {
        ui = FindAnyObjectByType<UIButtons>();
        pauseMenu = GameObject.FindGameObjectWithTag("PauseMenu");
        HUD = GameObject.FindGameObjectWithTag("HUD");
    }
    void Start()
    {
        pauseMenu.SetActive(false);
        HUD.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        ui = FindAnyObjectByType<UIButtons>();
        Debug.Log(pauseMenu);
        if (pauseMenu != null && !ui.settings)
        {
            if (pauseMenu.activeSelf)
            {
                paused = true;
            }
            else
            {
                paused = false;
            }
        }
    }
    public void TogglePause()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            HUD.SetActive(true);
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            HUD.SetActive(false);
            Time.timeScale = 0;
        }
    }
}
