using System.Collections;
using UnityEngine;

public class HealingEffect : BaseAppliedEffect
{
    public float TickHealAmount { get; set; }

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

        EnemyTarget?.GetHealth(TickHealAmount);
        PlayerTarget?.GetHealth(TickHealAmount);

        SetSpriteGreen(targetSprite);
    }

    /// <summary>
    /// Define a cor do sprite para verde.
    /// </summary>
    /// <param name="sprite">O sprite do alvo a ser alterado.</param>
    private void SetSpriteGreen(SpriteRenderer sprite)
    {
        if (sprite == null) return;

        sprite.color = new Color32(173, 255, 197, 255);

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
