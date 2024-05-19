using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Scripts")]
    HelperScript helper;
    PauseMenu pause;
    PlatformManager pm;
    UIButtons ui;

    public float sensitivity;
    float sensX;
    float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    // Start is called before the first frame update
    void Start()
    {
        helper = FindAnyObjectByType<HelperScript>();
        pause = FindAnyObjectByType<PauseMenu>();
        pm = FindAnyObjectByType<PlatformManager>();

        ui = FindAnyObjectByType<UIButtons>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sensX = sensitivity;
        sensY = sensitivity;   
    }

    // Update is called once per frame
    void Update()
    {
        pause = FindAnyObjectByType<PauseMenu>();
        ui = FindAnyObjectByType<UIButtons>();
        pm = FindAnyObjectByType<PlatformManager>();

        if (pause.pauseMenu.activeSelf || ui.settingsMenu.activeSelf || pm.winScreen.activeSelf)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            CameraMovement();
        }


    }

    void CameraMovement()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
