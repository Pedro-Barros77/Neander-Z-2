using System.Collections;
using UnityEngine;

public class HealingEffect : BaseAppliedEffect
{
    public float TickHealAmount { get; set; }
    private readonly Color32 GreenTickColor = new(173, 255, 197, 255);

    protected override void OnTickEffect()
    {
        base.OnTickEffect();

        if (transform.parent == null)
            return;

        EnemyTarget?.GetHealth(TickHealAmount);
        PlayerTarget?.GetHealth(TickHealAmount);

        SetSpriteGreen();
    }

    /// <summary>
    /// Define a cor do sprite para verde.
    /// </summary>
    private void SetSpriteGreen()
    {
        EnemyTarget?.HandleSpriteColorChange(GreenTickColor);
        PlayerTarget?.HandleSpriteColorChange(GreenTickColor);

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
