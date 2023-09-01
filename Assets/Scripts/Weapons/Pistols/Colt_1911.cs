using UnityEngine;

public class Colt_1911 : SemiAutoWeapon
{
    protected override void Awake()
    {
        base.Awake();
        ShootVolume = 0.4f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.4f, 0f);
    }
}
