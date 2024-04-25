using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCam : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInParent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y -= 1;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.2f);
    }

}
