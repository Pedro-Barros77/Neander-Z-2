using UnityEngine;

public class ShortBarrel : ShotgunWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.1f, 0f);
    }
}
