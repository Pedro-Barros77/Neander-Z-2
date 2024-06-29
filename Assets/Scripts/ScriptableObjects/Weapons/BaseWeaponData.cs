using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeaponData : AutoRevertSO
{
    public bool IsPrimary;
    public WeaponClasses WeaponClass;
    public WeaponTypes Type;
    public BulletTypes BulletType;
    public FireModes FireMode;
    public ReloadTypes ReloadType;

    public float Damage;
    public float MinDamage;
    public float HeadshotMultiplier;
    public float FireRate;
    public float BulletSpeed;

    public float MaxDamageRange;
    public float MinDamageRange;
    public float BulletMaxRange;

    public int MagazineSize;
    public int MagazineBullets;
    public float ReloadTimeMs;

    public float SwitchTimeMs;
    public float GravityScale;
    public bool FlyInfinitely = true;

    public List<WeaponUpgradeGroup> Upgrades;

    protected Vector3 WeaponContainerOffset;

    [Serializable]
    public class WeaponUpgradeGroup
    {
        public WeaponAttributes Attribute;
        public List<WeaponUpgrade> UpgradeSteps;

        [Serializable]
        public class WeaponUpgrade
        {
            public float Price;
            public float Value;
        }
    }
}
