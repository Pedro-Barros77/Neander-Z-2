using UnityEngine;

public class Scar : FullAutoWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.3f, 0f);
    }
}
