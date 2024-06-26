using System.Collections;
using System.Linq;
using UnityEngine;

public class Ronald : BaseEnemy, IKnockBackable, IBurnable
{
    public float RonaldoSpawnChance { get; set; } = 0.1f;
    private bool SpawnRonaldo;
    private float MaxRonaldoSpawnDelayMs = 5000f;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Ronald;
        MovementSpeed = 0.57f;
        AccelerationSpeed = 1f;
        Health = 24f;
        Damage = 18f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        base.Start();

        HealthBar.AnimationSpeed = 5f;
    }

    public override void Die(string lastDamagedBodyPartName, IEnemyTarget attacker)
    {
        if (isDying || !IsAlive)
            return;

        float randomValue = Random.Range(0f, 1f);
        SpawnRonaldo = randomValue <= RonaldoSpawnChance;

        IsAlive = false;
        isDying = true;
        DeathTime = Time.time;
        isRunning = false;
        isAttacking = false;

        bool isHeadshot = lastDamagedBodyPartName == "Head";

        WavesManager.Instance.CurrentWave.HandleScore(this, attacker, isHeadshot);

        DeathSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);

        if (HealthBar != null)
            Destroy(HealthBar.gameObject);

        if (!SpawnRonaldo && DeathFadeOutDelayMs > 0)
        {
            StartCoroutine(StartDeathFadeOutCountDown());
        }
    }

    protected override void OnDeathEnd()
    {
        if (SpawnRonaldo)
        {
            float ronaldoSpawnDelayMs = Random.Range(0, MaxRonaldoSpawnDelayMs);
            StartCoroutine(SpawnRonaldoDelayed(ronaldoSpawnDelayMs));
        }

        base.OnDeathEnd();

    }
    /// <summary>
    /// Gera um Z_Ronaldo ap�s um delay.
    /// </summary>
    /// <param name="delayMs">O tempo em milessegundos para aparecer o Ronaldo</param>
    private IEnumerator SpawnRonaldoDelayed(float delayMs)
    {
        yield return new WaitForSeconds(delayMs / 1000f);
        float ronaldoSpawnOffsetX = transform.localScale.x * (-1.7f);
        float ronaldoSpawnOffsetY = transform.localScale.y * (-1);
        Vector3 RonaldPosition = transform.position + new Vector3(ronaldoSpawnOffsetX * EnemyDirection, ronaldoSpawnOffsetY * EnemyDirection, 0);
        FlipEnemy(EnemyDirection);
        BaseEnemy ronaldo = WavesManager.Instance.CurrentWave.SpawnEnemy(EnemyTypes.Z_Ronaldo, RonaldPosition, WavesManager.Instance.CurrentWave.EnemiesContainer);
        WavesManager.Instance.CurrentWave.TotalEnemiesCount++;
        Destroy(gameObject);
    }
    public void ActiveBurningParticles(BurningEffect burnFx)
    {
        BodyFlames.StartFire(burnFx, true);
    }

    public void DeactivateFireParticles()
    {
        BodyFlames.StopFire();
    }
}
