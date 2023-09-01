using UnityEngine;

public class ShortBarrel : ShotgunWeapon
{
    protected override void Awake()
    {
        base.Awake();
        ShootVolume = 1f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.1f, 0f);
    }
}
