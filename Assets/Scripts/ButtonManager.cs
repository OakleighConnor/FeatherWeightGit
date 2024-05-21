using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    UIButtons ui;
    public void ChangeScene()
    {
        ui = FindAnyObjectByType<UIButtons>();
        ui.ChangeScene();
    }
    public void ToggleSettings()
    {
        ui = FindAnyObjectByType<UIButtons>();
        ui.ToggleSettings();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
