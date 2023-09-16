using UnityEngine;

public class Berreta_93 : BurstFireWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.4f, 0f);
    }
}
