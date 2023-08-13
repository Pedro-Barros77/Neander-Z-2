using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleBullet : Projectile
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveForward();
    }
}
