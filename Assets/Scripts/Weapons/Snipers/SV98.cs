using UnityEngine;

public class SV98 : SniperWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.3f, 0f);
    }
} 
