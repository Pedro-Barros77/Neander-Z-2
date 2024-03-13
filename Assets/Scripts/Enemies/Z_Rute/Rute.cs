using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rute : BaseEnemy
{
    protected override void Start()
    {
        Type = EnemyTypes.Z_Rute;
        MovementSpeed = 2.2f;
        AccelerationSpeed = 1f;
        Health = 30f;
        Damage = 6f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        base.Start();

        HealthBar.AnimationSpeed = 5f;
    }
}
