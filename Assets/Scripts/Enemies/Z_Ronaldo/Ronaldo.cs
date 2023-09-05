public class Ronaldo : BaseEnemy
{
    protected override void Start()
    {
        Type = EnemyTypes.Z_Ronaldo;
        MovementSpeed = 0.4f;
        AccelerationSpeed = 1f;
        Health = 24f;
        Damage = 20f;
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
