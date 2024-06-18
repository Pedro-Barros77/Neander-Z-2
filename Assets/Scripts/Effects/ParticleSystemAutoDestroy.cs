using UnityEngine;

public class ParticleSystemAutoDestroy : MonoBehaviour
{
    public float StopDelayMs, DestroyDelayMs;
    public bool DestroyIfStopped = true;
    ParticleSystem Particles;

    float stopTimeoutMs, destroyTimeoutMs;

    void Start()
    {
        Particles = GetComponent<ParticleSystem>();
        stopTimeoutMs = StopDelayMs;
        destroyTimeoutMs = DestroyDelayMs;
    }

    void Update()
    {
        if (DestroyIfStopped && !Particles.isPlaying)
            Destroy(gameObject);

        if (StopDelayMs > 0)
            stopTimeoutMs -= Time.deltaTime * 1000;

        if (stopTimeoutMs <= 0 && StopDelayMs > 0)
            Particles.Stop();

        if (DestroyDelayMs > 0)
            destroyTimeoutMs -= Time.deltaTime * 1000;

        if (destroyTimeoutMs <= 0 && DestroyDelayMs > 0)
            Destroy(gameObject);
    }
}
