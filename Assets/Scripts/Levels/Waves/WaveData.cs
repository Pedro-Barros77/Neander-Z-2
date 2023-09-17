using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Neander Z/Wave", order = 4)]
public class WaveData : AutoRevertSO
{
    /// <summary>
    /// O n�mero da wave.
    /// </summary>
    public int Number;
    /// <summary>
    /// O t�tulo da wave.
    /// </summary>
    public string Title;
    /// <summary>
    /// A descri��o da wave.
    /// </summary>
    public string Description;
    /// <summary>
    /// O grupo de inimigos da wave.
    /// </summary>
    public List<EnemyGroup> EnemyGroups;
    /// <summary>
    /// O n�mero m�nimo de inimigos vivos ao mesmo tempo.
    /// </summary>
    public int MinEnemiesAlive;
    /// <summary>
    /// O n�mero m�ximo de inimigos vivos ao mesmo tempo.
    /// </summary>
    public int MaxEnemiesAlive;
    /// <summary>
    /// O tempo m�nimo para esperar antes de spawnar mais inimigos, em caso de inatividade.
    /// </summary>
    public float MinSpawnDelayMs;
    /// <summary>
    /// O tempo m�ximo para esperar antes de spawnar mais inimigos, em caso de inatividade.
    /// </summary>
    public float MaxSpawnDelayMs;
    /// <summary>
    /// O n�mero m�nimo de inimigos em cada spawn.
    /// </summary>
    public int MinSpawnCount;
    /// <summary>
    /// O n�mero m�ximo de inimigos em cada spawn.
    /// </summary>
    public int MaxSpawnCount;
    /// <summary>
    /// O tempo a esperar antes de come�ar a wave.
    /// </summary>
    public float StartDelayMs = 2000;
    /// <summary>
    /// O tempo a esperar depois de terminar a wave.
    /// </summary>
    public float EndDelayMs = 1500;
    /// <summary>
    /// O multiplicador de dinheiro ganho nesta wave.
    /// </summary>
    public float MoneyMultiplier = 1;
}
