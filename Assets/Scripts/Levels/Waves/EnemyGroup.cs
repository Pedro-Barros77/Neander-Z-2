using System;

/// <summary>
/// Um grupo de inimigos do mesmo tipo, para evitar de setar valores individualmente.
/// </summary>
[Serializable]
public class EnemyGroup
{
    /// <summary>
    /// O tipo de inimigo.
    /// </summary>
    public EnemyTypes EnemyType;
    /// <summary>
    /// O número de inimigos a serem spawnados com essa configuração.
    /// </summary>
    public int Count;
    /// <summary>
    /// A vida mínima dos inimigos no Random.
    /// </summary>
    public float MinHealth;
    /// <summary>
    /// A vida máxima dos inimigos no Random.
    /// </summary>
    public float MaxHealth;
    /// <summary>
    /// A velocidade mínima dos inimigos no Random.
    /// </summary>
    public float MinSpeed;
    /// <summary>
    /// A velocidade máxima dos inimigos no Random.
    /// </summary>
    public float MaxSpeed;
    /// <summary>
    /// O dano mínimo dos inimigos no Random.
    /// </summary>
    public float MinDamage;
    /// <summary>
    /// O dano máximo dos inimigos no Random.
    /// </summary>
    public float MaxDamage;
    /// <summary>
    /// A quantidade mínima de score que o inimigo vale.
    /// </summary>
    public int MinKillScore;
    /// <summary>
    /// A quantidade máxima de score que o inimigo vale.
    /// </summary>
    public int MaxKillScore;
    /// <summary>
    /// A chance de spawnar um inimigo com essas configurações.
    /// </summary>
    public float SpawnChanceMultiplier = 1;
    /// <summary>
    /// Se a quantidade de inimigos dessa configuração é infinita. Utilizado na wave de boss para spawnar inimigos infinitamente até que ele seja derrotado.
    /// </summary>
    public bool IsInfinite;
    /// <summary>
    /// Desativa o groupo para fins de teste.
    /// </summary>
    public bool IsDisabled;

    #region Enemy-Specific Properties
    //Ronald
    public float RonaldSpawnChance;

    //Raimundo
    public float RaimundoHelmetHealth;

    //Raven
    public float RavenAttackChance;
    public float RavenAttackAttemptDelayMs;

    //Rute
    public float RuteBurningEffectDurationMs;
    public float RuteBurningEffectTickIntervalMs;
    public float RuteSelfBurningEffectTickIntervalMs;
    public float RuteSelfDamage;
    public float RuteFloorFlameDamage;
    #endregion
}
