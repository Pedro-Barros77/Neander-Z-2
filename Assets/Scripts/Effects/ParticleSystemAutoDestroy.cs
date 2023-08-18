using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    ParticleSystem Particles;

    void Start()
    {
        Particles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!Particles.isPlaying)
            Destroy(gameObject);
    }
}
