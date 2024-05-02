using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFlames : MonoBehaviour
{
    ParticleSystem FireParticles;
    void Start()
    {
        FireParticles = GetComponent<ParticleSystem>();
        var main = FireParticles.main;
        main.startLifetime = 1f / FireParticles.startSpeed;
    }

    void Update()
    {
        
    }
}
