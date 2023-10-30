using UnityEngine;

public class Robert : BaseEnemy, IKnockBackable
{
    protected override void Start()
    {
        Type = EnemyTypes.Z_Robert;
        MovementSpeed = 5f;
        AccelerationSpeed = 1.5f;
        Health = 18f;
        Damage = 24f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        base.Start();

        HealthBar.AnimationSpeed = 5f;
    }

    protected override void StartAttack(IEnemyTarget target)
    {
        if (isAttacking)
            return;

        if (target == null)
            return;

        RigidBody.velocity = new Vector2(RigidBody.velocity.x / 2, RigidBody.velocity.y);

        base.StartAttack(target);
    }
}
