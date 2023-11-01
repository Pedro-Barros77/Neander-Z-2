public class Roger : BaseEnemy, IKnockBackable
{
    protected override void Start()
    {
        Type = EnemyTypes.Z_Roger;
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 28f;
        Damage = 15f;
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
