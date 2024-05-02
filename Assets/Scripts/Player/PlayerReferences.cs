using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{
    [Header("References")]
    public Animator anim;
    public Rigidbody rb;

    [Header("Scripts")]
    //PlayerWeapons weapons;

    [Header("Transforms")]
    public Transform cam;

    [Header("Damage")]
    public bool KB;
    public LayerMask interactableLayers;

    [Header("Health")]
    public float weight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        
    }
}
