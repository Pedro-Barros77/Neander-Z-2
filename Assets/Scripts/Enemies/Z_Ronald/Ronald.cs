using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ronald : BaseEnemy
{
    private float spawnChance = 1f;
    private bool SpawnRonaldo;
    private float MaxRonaldoSpawnDelayMs = 5000f;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Ronald;
        MovementSpeed = 0.57f;
        AccelerationSpeed = 1f;
        Health = 24f;
        Damage = 18f;
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

    protected override void Die(string lastDamagedBodyPartName)
    {
        float randomValue = Random.Range(0f, 1f);
        SpawnRonaldo = randomValue <= spawnChance;

        IsAlive = false;
        isDying = true;
        DeathTime = Time.time;
        isRunning = false;
        isAttacking = false;

        if (DeathSounds.Any())
        {
            var randomDeathSound = DeathSounds[Random.Range(0, DeathSounds.Count)];
            AudioSource.PlayOneShot(randomDeathSound, DeathSoundVolume);
        }

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
    /// Gera um Z_Ronaldo após um delay.
    /// </summary>
    /// <param name="delayMs">O tempo em milessegundos para aparecer o Ronaldo</param>
    private IEnumerator SpawnRonaldoDelayed(float delayMs)
    {
        yield return new WaitForSeconds(delayMs / 1000f);
        float ronaldoSpawnOffsetX = transform.localScale.x * (-1.7f);
        float ronaldoSpawnOffsetY = transform.localScale.y * (-1);
        Vector3 RonaldPosition = transform.position + new Vector3(ronaldoSpawnOffsetX * EnemyDirection, ronaldoSpawnOffsetY * EnemyDirection, 0);
        FlipEnemy(EnemyDirection);
        Destroy(gameObject);
        GameObject ronaldo = Instantiate(Resources.Load<GameObject>($"Prefabs/Enemies/{EnemyTypes.Z_Ronaldo}"), RonaldPosition, Quaternion.identity);
    }

}
