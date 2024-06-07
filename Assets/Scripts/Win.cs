using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    [Header("UI")]
    public GameObject winScreen;

    // Start is called before the first frame update
    void Start()
    {
        winScreen = GameObject.FindGameObjectWithTag("Win").transform.GetChild(0).gameObject;
        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(winScreen == null)
        {
            winScreen = GameObject.FindGameObjectWithTag("Win").transform.GetChild(0).gameObject;
        }
    }

    public void WinScreen()
    {
        Time.timeScale = 0;
        winScreen.SetActive(true);
    }
}
