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
    /// O n�mero de inimigos a serem spawnados com essa configura��o.
    /// </summary>
    public int Count;
    /// <summary>
    /// A vida m�nima dos inimigos no Random.
    /// </summary>
    public float MinHealth;
    /// <summary>
    /// A vida m�xima dos inimigos no Random.
    /// </summary>
    public float MaxHealth;
    /// <summary>
    /// A velocidade m�nima dos inimigos no Random.
    /// </summary>
    public float MinSpeed;
    /// <summary>
    /// A velocidade m�xima dos inimigos no Random.
    /// </summary>
    public float MaxSpeed;
    /// <summary>
    /// O dano m�nimo dos inimigos no Random.
    /// </summary>
    public float MinDamage;
    /// <summary>
    /// O dano m�ximo dos inimigos no Random.
    /// </summary>
    public float MaxDamage;
    /// <summary>
    /// A quantidade m�nima de score que o inimigo vale.
    /// </summary>
    public int MinKillScore;
    /// <summary>
    /// A quantidade m�xima de score que o inimigo vale.
    /// </summary>
    public int MaxKillScore;
    /// <summary>
    /// A chance de spawnar um inimigo com essas configura��es.
    /// </summary>
    public float SpawnChanceMultiplier = 1;
    /// <summary>
    /// Se a quantidade de inimigos dessa configura��o � infinita. Utilizado na wave de boss para spawnar inimigos infinitamente at� que ele seja derrotado.
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
