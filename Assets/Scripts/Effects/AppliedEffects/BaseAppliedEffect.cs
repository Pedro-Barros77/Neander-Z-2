using UnityEngine;

public class BaseAppliedEffect : MonoBehaviour
{
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

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        var now = Time.time;

        if (LastTickTime + (TickIntervalMs / 1000) < now)
            OnTickEffect();

        if (TimeElasped >= DurationMs / 1000)
            OnTimeOut();
    }

    /// <summary>
    /// Define os valores de duração e intervalo de tick do efeito, reinicia o temporizador.
    /// </summary>
    /// <param name="durationMs"></param>
    /// <param name="tickIntervalMs"></param>
    public virtual void SetEffect(float durationMs, float tickIntervalMs)
    {
        DurationMs = durationMs;
        TickIntervalMs = tickIntervalMs;
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
