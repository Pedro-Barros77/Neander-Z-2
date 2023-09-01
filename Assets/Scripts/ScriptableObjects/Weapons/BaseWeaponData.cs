using UnityEngine;

public abstract class BaseWeaponData : ScriptableObject
{
    public bool IsPrimary;
    public WeaponTypes Type;
    public BulletTypes BulletType;

    public float Damage;
    public float FireRate;
    public float BulletSpeed;

    public float BulletMaxRange;
    public float MaxDamageRange;
    public float MinDamageRange;

    public int MagazineSize;
    public int MagazineBullets;
    public float ReloadTimeMs;

    public float SwitchTimeMs;
    protected Vector3 WeaponContainerOffset;
}
