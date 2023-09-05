using UnityEngine;

public class UZI : FullAutoWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.2f, 0f);
    }
}
