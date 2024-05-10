using UnityEngine;

public class Kar98 : SniperWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.35f, 0f);
    }

    public override void OnReloadEnd()
    {
        base.OnReloadEnd();

        IsBoltActionPending = false;
        IsPullingBolt = false;
    }
} 
