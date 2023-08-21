using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleBullet : Projectile
{
    protected override void Start()
    {
        base.Start();
        MoveForward();
    }

    protected override void OnEnemyHit(Collider2D collision)
    {
        var target = collision.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            target.TakeDamage(Damage, collision.name);
        }

        base.OnEnemyHit(collision);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
