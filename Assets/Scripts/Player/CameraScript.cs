using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [Header("Scripts")]
    HelperScript helper;

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        sensX = sensitivity;
        sensY = sensitivity;   
    }

    // Update is called once per frame
    void Update()
    {
        if (helper.playerAlive)
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
}
