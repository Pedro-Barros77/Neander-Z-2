using System.Collections;
using UnityEngine;

public class BaseAppliedEffect : MonoBehaviour
{
    /// <summary>
    /// Se o efeito � infinito.
    /// </summary>
    public bool IsInfinite { get; protected set; }
    /// <summary>
    /// Se definido como true, SetEffect n�o tem mais efeito.
    /// </summary>
    public bool LockReset { get; set; }
    /// <summary>
    /// A dura��o total do efeito em milisegundos.
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
    /// O tempo em que o �ltimo tick ocorreu.
    /// </summary>
    public float LastTickTime { get; protected set; }
    /// <summary>
    /// O tempo restante para o efeito expirar.
    /// </summary>
    public float TimeLeft => (DurationMs / 1000) - TimeElasped;

    /// <summary>
    /// O tempo decorrido desde o in�cio do efeito.
    /// </summary>
    protected float TimeElasped => Time.time - StartTime;
    /// <summary>
    /// Se o efeito foi aplicado pelo pr�prio alvo.
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
    public IPlayerTarget EnemyOwner { get; protected set; }
    /// <summary>
    /// O dono desse efeito, se for um player.
    /// </summary>
    public IEnemyTarget PlayerOwner { get; protected set; }

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

    protected virtual void FixedUpdate()
    {
    }

    /// <summary>
    /// Define os valores de dura��o e intervalo de tick do efeito, reinicia o temporizador.
    /// </summary>
    /// <param name="durationMs"></param>
    /// <param name="tickIntervalMs"></param>
    public virtual void SetEffect(float durationMs, float tickIntervalMs, bool isInfinite = false)
    {
        if (LockReset)
            return;

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

    public virtual void SetOwner(IPlayerTarget owner)
    {
        if (LockReset)
            return;

        EnemyOwner = owner;
    }

    public virtual void SetOwner(IEnemyTarget owner)
    {
        if (LockReset)
            return;

        PlayerOwner = owner;
    }

    /// <summary>
    /// Fun��o chamada a cada intervalo de tick.
    /// </summary>
    protected virtual void OnTickEffect()
    {
        LastTickTime = Time.time;
    }

    /// <summary>
    /// Fun��o chamada quando o efeito expira (timeout).
    /// </summary>
    protected virtual void OnTimeOut()
    {
        KillSelf();
    }

    /// <summary>
    /// Destr�i o item.
    /// </summary>
    protected virtual void KillSelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
