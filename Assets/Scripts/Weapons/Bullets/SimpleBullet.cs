using UnityEngine;
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
            if (target.IsAlive)
                IsTargetHit = true;

            var hitPosition = collision.ClosestPoint(transform.position);
            target.TakeDamage(Damage, collision.name, PlayerOwner, hitPosition);
            target.OnPointHit(hitPosition, -transform.right, collision.name);
        }

        base.OnEnemyHit(collision);
    }
}
