using System.Collections;
using UnityEngine;

public class BurningEffect : BaseAppliedEffect
{
    public float TickDamage { get; set; }

    SpriteRenderer targetSprite;
    Color32 targetStartColor;

    protected override void Start()
    {
        base.Start();

        if (EnemyTarget != null)
        {
            targetSprite = EnemyTarget.gameObject.GetComponent<SpriteRenderer>();
            targetStartColor = targetSprite.color;
        }

        if (PlayerTarget != null)
        {
            targetSprite = PlayerTarget.gameObject.GetComponent<SpriteRenderer>();
            targetStartColor = targetSprite.color;
        }
    }

    protected override void OnTickEffect()
    {
        base.OnTickEffect();

        if (transform.parent == null)
            return;

        EnemyTarget?.TakeDamage(TickDamage, 1, "", null, selfDamage: SelfAppliedEffect);
        PlayerTarget?.TakeDamage(TickDamage, 1, "", PlayerOwner);

        SetSpriteRed(targetSprite);
    }

    /// <summary>
    /// Define a cor do sprite para vermelho.
    /// </summary>
    /// <param name="sprite">O sprite do alvo a ser alterado.</param>
    private void SetSpriteRed(SpriteRenderer sprite)
    {
        if (sprite == null) return;

        sprite.color = new Color32(255, 200, 200, 255);

        StartCoroutine(ResetSpriteColor(sprite));
    }

    /// <summary>
    /// Define a cor do sprite de volta para a original, após um tempo.
    /// </summary>
    /// <param name="sprite">O sprite do alvo a ser alterado.</param>
    IEnumerator ResetSpriteColor(SpriteRenderer sprite)
    {
        yield return new WaitForSeconds(Mathf.Min(TickIntervalMs / 2000, 500));

        sprite.color = targetStartColor;
    }

    private void OnDestroy()
    {
        if (targetSprite != null)
            targetSprite.color = targetStartColor;
    }
}
