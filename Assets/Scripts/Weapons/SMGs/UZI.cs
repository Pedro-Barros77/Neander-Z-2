using UnityEngine;

public class UZI : FullAutoWeapon
{
    protected override void Awake()
    {
        base.Awake();
        ShootVolume = 1.3f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.2f, 0f);
    }
}
