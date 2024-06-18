using UnityEngine;

[CreateAssetMenu(fileName = "New Throwable", menuName = "Neander Z/Weapons/Throwable", order = 8)]
public class BaseThrowableData : AutoRevertSO
{
    public ThrowableTypes Type;
    public int MaxCount;

    public float Damage;
    public float MinDamage;
    public float HitDamage;
    public float HeadshotMultiplier;
    public float ThrowForce;
    public float FuseTimeoutMs;
    public bool DetonateOnImpact;
    public bool StartFuseOnCook;
    public bool RotateToFaceVelocity;
    public float EffectMaxRange;
    public float EffectMinRange;
    public float EffectDurationMs;
    public float EffectDecoupledDurationMs;
    public float EffectTickIntervalMs;
    public float EffectSpriteSize = 1;
    public float ArmorPiercingPercentage;
}
