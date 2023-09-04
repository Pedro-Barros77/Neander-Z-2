using UnityEngine;

public class RPG : LauncherWeapon
{
    protected override void Awake()
    {
        base.Awake();
        ShootVolume = 0.4f;
        EmptyChamberVolume = 0.3f;
        WeaponContainerOffset = new Vector3(0f, 0.25f, 0f);
    }
}
