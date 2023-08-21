public class Roger : BaseEnemy
{
    protected override void Start()
    {
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 28f;
        Damage = 15f;
        HeadshotDamageMultiplier = 2f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        base.Start();

        HealthBar.AnimationSpeed = 5f;
    }
}
