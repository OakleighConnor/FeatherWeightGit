using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [Header("Camera")]
    public Camera playerCamera;
    public float originalCameraSpeed;
    float cameraSpeed;

    [Header("Scripts")]
    public HelperScript helper;
    public PauseMenu pause;
    public UIButtons ui;
    public PlatformManager pm;

    public float sensitivity;
    float sensX;
    float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    public GameObject winScreen;
    // Start is called before the first frame update
    void Start()
    {

        playerCamera = GetComponent<Camera>();

        helper = FindAnyObjectByType<HelperScript>();
        pause = FindAnyObjectByType<PauseMenu>();

        ui = FindAnyObjectByType<UIButtons>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sensX = sensitivity;
        sensY = sensitivity;

        winScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        helper = FindAnyObjectByType<HelperScript>();
        sensitivity = helper.sensitivity;
        sensX = sensitivity;
        sensY = sensitivity;

        pause = FindAnyObjectByType<PauseMenu>();
        ui = FindAnyObjectByType<UIButtons>();

        if(ui.settingsMenu != null)
        {
            if (pause.pauseMenu.activeSelf || ui.settingsMenu.activeSelf || winScreen.activeSelf)
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
    
    public void RespawnCamera()
    {
        playerCamera.fieldOfView = 60;
        cameraSpeed = originalCameraSpeed;
        StopAllCoroutines();
        StartCoroutine(RespawnFOV(playerCamera.fieldOfView));
    }

    IEnumerator RespawnFOV(float fov)
    {
        bool zoomOut = true;

        while (zoomOut)
        {
            playerCamera.fieldOfView += Time.deltaTime * cameraSpeed;
            cameraSpeed += cameraSpeed/10;
            zoomOut = playerCamera.fieldOfView < 170;
            yield return null;
        }

        bool respawned = false;

        if (!respawned)
        {
            pm = FindAnyObjectByType<PlatformManager>();
            pm.TeleportToSpawn();
            respawned = true;
        }

        bool zoomIn = true;

        while (zoomIn)
        {
            playerCamera.fieldOfView -= Time.deltaTime * cameraSpeed;
            cameraSpeed -= cameraSpeed/10;
            zoomIn = playerCamera.fieldOfView > 60;

            if(playerCamera.fieldOfView < fov)
            {
                playerCamera.fieldOfView = 60;
            }
            yield return null;
        }

        cameraSpeed = originalCameraSpeed;
    }
}
