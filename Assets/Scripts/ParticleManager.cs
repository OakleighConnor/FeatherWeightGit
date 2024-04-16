using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParticleManager : MonoBehaviour
{
    [Header("References")]
    PlayerMovement player;
    public ParticleSystem gp;
    public Transform gpPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GroundPound()
    {
        Instantiate(gp, gpPos.position, transform.rotation = Quaternion.Euler(90, 0, 0));
        //Make the particle effect destory itself after having played the animation
    }
}
