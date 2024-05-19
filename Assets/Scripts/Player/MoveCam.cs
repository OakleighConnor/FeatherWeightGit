using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    [Header("Scripts")]
    HelperScript helper;

    public Transform camPos;
    void Start()
    {
        helper = FindAnyObjectByType<HelperScript>();
    }
        // Update is called once per frame
    void Update()
    {
        transform.position = camPos.position;
    }
}
