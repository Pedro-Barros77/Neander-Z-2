using System.Collections.Generic;
using UnityEngine;

public class Spas12 : ShotgunWeapon
{
    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, 0.3f, 0f);
    }

    protected override void SyncAnimationStates()
    {
        base.SyncAnimationStates();

        Animator.SetFloat("shootSpeed", FireRate / 5);
    }
}
