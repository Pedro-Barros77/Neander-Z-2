using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raimundo : BaseEnemy
{
    protected override void Start()
    {
        Type = EnemyTypes.Z_Raimundo;
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 40f;
        Damage = 19f;
        HeadshotDamageMultiplier = 2f;
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
