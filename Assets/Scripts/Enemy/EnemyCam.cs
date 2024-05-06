using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCam : MonoBehaviour
{
    [Header("References")]
    public Transform playerCam;
    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 lookPos = playerCam.position - transform.position;
        lookPos.y -= 1;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }

}
