using UnityEngine;

public class TakeDamageProps
{
    public DamageTypes DamageType { get; private set; }
    public float Damage { get; private set; }
    public float HeadshotMultiplier { get; private set; }
    public string BodyPartName { get; private set; }
    public IEnemyTarget PlayerAttacker { get; private set; }
    public IPlayerTarget EnemyAttacker { get; private set; }
    public Vector3? HitPosition { get; private set; }
    public Vector3? HitEffectDirection { get; private set; }
    public float ArmorPiercingPercentage { get; private set; }


    public bool IsSelfDamage(IEnemyTarget player) => PlayerAttacker == player;
    public bool IsSelfDamage(IPlayerTarget enemy) => EnemyAttacker == enemy;

    public TakeDamageProps(DamageTypes damageType, float damage, IEnemyTarget playerAttacker, IPlayerTarget enemyAttacker, float headshotMultiplier, string bodyPartName, Vector3? hitPosition = null, float armorPiercingPercentage = 0)
    {
        DamageType = damageType;
        Damage = damage;
        HeadshotMultiplier = headshotMultiplier;
        BodyPartName = bodyPartName;
        PlayerAttacker = playerAttacker;
        EnemyAttacker = enemyAttacker;
        HitPosition = hitPosition;
        ArmorPiercingPercentage = armorPiercingPercentage;
    }

    public TakeDamageProps(DamageTypes damageType, float damage, IEnemyTarget playerAttacker)
    {
        DamageType = damageType;
        Damage = damage;
        PlayerAttacker = playerAttacker;
    }

    public TakeDamageProps(DamageTypes damageType, float damage, IPlayerTarget enemyAttacker)
    {
        DamageType = damageType;
        Damage = damage;
        EnemyAttacker = enemyAttacker;
    }

    public TakeDamageProps(DamageTypes damageType, float damage, IEnemyTarget playerAttacker, float headshotMultiplier)
    {
        DamageType = damageType;
        Damage = damage;
        PlayerAttacker = playerAttacker;
        HeadshotMultiplier = headshotMultiplier;
    }

    public TakeDamageProps(DamageTypes damageType, float damage, IPlayerTarget enemyAttacker, float headshotMultiplier)
    {
        DamageType = damageType;
        Damage = damage;
        EnemyAttacker = enemyAttacker;
        HeadshotMultiplier = headshotMultiplier;
    }

    public TakeDamageProps WithBodyPart(string bodyPartName)
    {
        BodyPartName = bodyPartName;
        return this;
    }

    public TakeDamageProps WithHitPosition(Vector3 hitPosition)
    {
        HitPosition = hitPosition;
        return this;
    }

    public TakeDamageProps WithHitEffectDirection(Vector3 hitEffectDirection)
    {
        HitEffectDirection = hitEffectDirection;
        if(HitPosition == null)
            HitPosition = hitEffectDirection + Vector3.right * 0.1f;
        return this;
    }

    public TakeDamageProps WithArmorPiercingPercentage(float armorPiercingPercentage)
    {
        ArmorPiercingPercentage = Mathf.Clamp01(armorPiercingPercentage);
        return this;
    }
}
