using System.Collections;
using UnityEngine;

public class BurningEffect : BaseAppliedEffect
{
    public float TickDamage { get; set; }
    private readonly Color32 RedTickColor = new(255, 150, 150, 255);
    private IBurnable burnableTarget;

    protected override void Start()
    {
        base.Start();
        burnableTarget = transform.parent.GetComponent<IBurnable>();
        burnableTarget?.ActiveBurningParticles(this);
    }

    protected override void OnTickEffect()
    {
        base.OnTickEffect();

        if (transform.parent == null)
            return;

        if (EnemyTarget is IBurnable)
        {
            if (PlayerOwner != null)
                EnemyTarget?.TakeDamage(new TakeDamageProps(DamageTypes.Fire, TickDamage, PlayerOwner));
            else if (EnemyOwner != null)
                EnemyTarget?.TakeDamage(new TakeDamageProps(DamageTypes.Fire, TickDamage, EnemyOwner));
        }

        if (PlayerTarget is IBurnable)
        {
            if (PlayerOwner != null)
                PlayerTarget?.TakeDamage(new TakeDamageProps(DamageTypes.Fire, TickDamage, PlayerOwner));
            else if (EnemyOwner != null)
                PlayerTarget?.TakeDamage(new TakeDamageProps(DamageTypes.Fire, TickDamage, EnemyOwner));
        }

        if (burnableTarget != null)
            SetSpriteRed();
    }

    protected override void OnTimeOut()
    {
        base.OnTimeOut();
        burnableTarget?.DeactivateFireParticles();
    }

    /// <summary>
    /// Define a cor do sprite para vermelho.
    /// </summary>
    private void SetSpriteRed()
    {
        burnableTarget?.HandleSpriteColorChange(RedTickColor);

        StartCoroutine(ResetSpriteColor());
    }

    /// <summary>
    /// Define a cor do sprite de volta para a original, após um tempo.
    /// </summary>
    IEnumerator ResetSpriteColor()
    {
        yield return new WaitForSeconds(Mathf.Min(TickIntervalMs / 2000, 500));

        burnableTarget?.HandleSpriteColorChange(Color.white);
    }

    private void OnDestroy()
    {
        burnableTarget?.HandleSpriteColorChange(Color.white);
    }
}
