using UnityEngine;

public class M16 : BurstFireWeapon
{
    protected override void Awake()
    {
        base.Awake();
        ShootVolume = 0.4f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.2f, 0f);
    }
}
