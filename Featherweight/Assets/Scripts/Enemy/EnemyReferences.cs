using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class EnemyReferences : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent navMesh;
    [HideInInspector] public Animator anim;

    [Header("Stats")]
    public float pathUpdateDelay = 0.2f;
    // Start is called before the first frame update
    void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
}
