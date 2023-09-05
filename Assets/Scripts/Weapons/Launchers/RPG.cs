using UnityEngine;

public class RPG : LauncherWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.25f, 0f);
    }
}
