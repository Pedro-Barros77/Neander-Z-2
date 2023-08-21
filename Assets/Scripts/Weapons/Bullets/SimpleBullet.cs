using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleBullet : Projectile
{
    protected override void Start()
    {
        base.Start();
        StartForwardMovement();
    }

    protected override void OnEnemyHit(Collider2D collision)
    {
        var target = collision.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            target.TakeDamage(Damage, collision.name);
            target.OnPointHit(collision.ClosestPoint(transform.position), -transform.right);
        }

        base.OnEnemyHit(collision);
    }
}
