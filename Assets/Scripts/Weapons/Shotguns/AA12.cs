using System.Collections.Generic;
using UnityEngine;

public class AA12 : ShotgunWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.2f, 0f);
    }

    protected override void SyncAnimationStates()
    {
        base.SyncAnimationStates();

        Animator.SetFloat("shootSpeed", FireRate / 3);
    }
}
