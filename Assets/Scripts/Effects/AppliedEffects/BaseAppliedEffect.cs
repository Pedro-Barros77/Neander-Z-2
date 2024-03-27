using System.Collections;
using UnityEngine;

public class BaseAppliedEffect : MonoBehaviour
{
    /// <summary>
    /// Se o efeito é infinito.
    /// </summary>
    public bool IsInfinite { get; protected set; }
    /// <summary>
    /// A duração total do efeito em milisegundos.
    /// </summary>
    public float DurationMs { get; protected set; }
    /// <summary>
    /// O tempo em que o efeito foi iniciado.
    /// </summary>
    public float StartTime { get; protected set; }
    /// <summary>
    /// O intervalo de tick do efeito em milisegundos.
    /// </summary>
    public float TickIntervalMs { get; protected set; }
    /// <summary>
    /// O tempo de delay para o primeiro tick do efeito em milisegundos.
    /// </summary>
    public float StartDelayMs { get; set; }
    /// <summary>
    /// O tempo em que o último tick ocorreu.
    /// </summary>
    public float LastTickTime { get; protected set; }

    /// <summary>
    /// O tempo decorrido desde o início do efeito.
    /// </summary>
    protected float TimeElasped => Time.time - StartTime;
    /// <summary>
    /// O tempo restante para o efeito expirar.
    /// </summary>
    protected float TimeLeft => (DurationMs / 1000) - TimeElasped;
    /// <summary>
    /// Se o efeito foi aplicado pelo próprio alvo.
    /// </summary>
    public bool SelfAppliedEffect => (EnemyOwner != null && EnemyOwner == PlayerTarget) || (PlayerOwner != null && PlayerOwner == EnemyTarget);

    /// <summary>
    /// O alvo do efeito, se for um player/torreta etc.
    /// </summary>
    protected virtual IEnemyTarget EnemyTarget { get; set; }
    /// <summary>
    /// O alvo do efeito, se for um inimigo.
    /// </summary>
    protected virtual IPlayerTarget PlayerTarget { get; set; }

    /// <summary>
    /// O dono desse efeito, se for um inimigo.
    /// </summary>
    public IPlayerTarget EnemyOwner { get; set; }
    /// <summary>
    /// O dono desse efeito, se for um player.
    /// </summary>
    public IEnemyTarget PlayerOwner { get; set; }

    protected virtual void Start()
    {
        EnemyTarget = GetComponentInParent<IEnemyTarget>();
        PlayerTarget = GetComponentInParent<IPlayerTarget>();
    }

    protected virtual void Update()
    {
        if (StartTime == 0)
            return;

        var now = Time.time;

        if (LastTickTime + (TickIntervalMs / 1000) < now)
            OnTickEffect();

        if (!IsInfinite && TimeElasped >= DurationMs / 1000)
            OnTimeOut();
    }

    /// <summary>
    /// Define os valores de duração e intervalo de tick do efeito, reinicia o temporizador.
    /// </summary>
    /// <param name="durationMs"></param>
    /// <param name="tickIntervalMs"></param>
    public virtual void SetEffect(float durationMs, float tickIntervalMs, bool isInfinite = false)
    {
        DurationMs = durationMs;
        TickIntervalMs = tickIntervalMs;
        IsInfinite = isInfinite;

        if (StartDelayMs == 0)
            StartTime = Time.time;
        else
            StartCoroutine(StartEffectDelayed());
    }

    /// <summary>
    /// Aguarda o StartDelayMs antes de iniciar o efeito.
    /// </summary>
    protected virtual IEnumerator StartEffectDelayed()
    {
        yield return new WaitForSeconds(StartDelayMs / 1000);

        StartTime = Time.time;
    }

    /// <summary>
    /// Função chamada a cada intervalo de tick.
    /// </summary>
    protected virtual void OnTickEffect()
    {
        LastTickTime = Time.time;
    }

    /// <summary>
    /// Função chamada quando o efeito expira (timeout).
    /// </summary>
    protected virtual void OnTimeOut()
    {
        KillSelf();
    }

    /// <summary>
    /// Destrói o item.
    /// </summary>
    protected virtual void KillSelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
