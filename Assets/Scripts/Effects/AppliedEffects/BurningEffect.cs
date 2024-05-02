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

        EnemyTarget?.TakeDamage(TickDamage, 1, "", null, selfDamage: SelfAppliedEffect);
        PlayerTarget?.TakeDamage(TickDamage, 1, "", PlayerOwner);

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
        EnemyTarget?.HandleSpriteColorChange(RedTickColor);
        PlayerTarget?.HandleSpriteColorChange(RedTickColor);

        StartCoroutine(ResetSpriteColor());
    }

    /// <summary>
    /// Define a cor do sprite de volta para a original, após um tempo.
    /// </summary>
    IEnumerator ResetSpriteColor()
    {
        yield return new WaitForSeconds(Mathf.Min(TickIntervalMs / 2000, 500));

        EnemyTarget?.HandleSpriteColorChange(Color.white);
        PlayerTarget?.HandleSpriteColorChange(Color.white);
    }

    private void OnDestroy()
    {
        EnemyTarget?.HandleSpriteColorChange(Color.white);
        PlayerTarget?.HandleSpriteColorChange(Color.white);
    }
}
