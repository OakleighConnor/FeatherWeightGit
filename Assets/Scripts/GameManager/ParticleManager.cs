using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParticleManager : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem explosion;
    public ParticleSystem enemyRemains;
    public ParticleSystem impactSystem;
    public ParticleSystem ShootingSystem;
    public ParticleSystem smoke;

    [Header("Trails")]
    public TrailRenderer BulletTrail;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Explosion(Transform trans)
    {
        Instantiate(explosion, trans.position, transform.rotation);
        Instantiate(enemyRemains, trans.position, transform.rotation);
    }
}
