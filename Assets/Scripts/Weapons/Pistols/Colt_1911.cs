using UnityEngine;

public class Colt_1911 : SemiAutoWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.4f, 0f);
    }
}
