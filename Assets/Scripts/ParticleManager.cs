using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParticleManager : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem groundPound;
    public ParticleSystem explosion;
    public ParticleSystem enemyRemains;
    public Transform gpPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GroundPound()
    {
        Instantiate(groundPound, gpPos.position, transform.rotation = Quaternion.Euler(90, 0, 0));
    }

    public void Explosion(Transform trans)
    {
        Instantiate(explosion, trans.position, transform.rotation = Quaternion.Euler(90, 0, 0));
        Instantiate(enemyRemains, trans.position, transform.rotation = Quaternion.Euler(90, 0, 0));
    }
}
