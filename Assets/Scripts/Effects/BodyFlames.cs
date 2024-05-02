using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BodyFlames : MonoBehaviour
{
    ParticleSystem FireParticles;
    BurningEffect BurnFX;
    List<ParticleSystem> Flames = new();
    bool handleFlamesDeactivation;

    private void Start()
    {
        FireParticles = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!handleFlamesDeactivation)
            return;

        if (BurnFX != null)
        {
            float totalDuration = BurnFX.DurationMs / 1000;
            float timeLeft = BurnFX.TimeLeft;
            int flamesCount = Flames.Count;

            float flamesToActivate = Mathf.Ceil((flamesCount * timeLeft) / totalDuration);
            for (int i = 0; i < flamesCount; i++)
            {
                if (i < flamesToActivate)
                    Flames[i].Play(true);
                else
                    Flames[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    public void StartFire(BurningEffect burnFX, bool deactivateFlamesOverTime)
    {
        BurnFX = burnFX;
        Flames.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            var particles = transform.GetChild(i).GetComponent<ParticleSystem>();
            if (particles != null)
                Flames.Add(particles);
        }
        Flames = Flames.OrderBy(_ => Random.value).ToList();
        FireParticles.Play(true);
        handleFlamesDeactivation = deactivateFlamesOverTime;
    }

    public void StopFire()
    {
        FireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        handleFlamesDeactivation = false;
    }
}
