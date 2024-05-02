using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class EnemyReferences : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent navMesh;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    public Transform cam;


    [Header("Stats")]
    public float pathUpdateDelay = 0.2f;

    [Header("Damage")]
    public bool KB;
    public LayerMask interactableLayers;
    public bool shooting;
    public bool punching;

    // Start is called before the first frame update
    void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
}
