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
        base.OnEnemyHit(collision);

        var target = collision.GetComponentInParent<IPlayerTarget>();

        if (target != null)
        {
            if (target.IsAlive)
            {
                IsTargetHit = true;
                LastEnemyHit = target;
            }

            var hitPosition = collision.ClosestPoint(transform.position);
            target.TakeDamage(Damage, HeadshotMultiplier, collision.name, PlayerOwner, hitPosition);
            target.OnPointHit(hitPosition, -transform.right, collision.name);
        }
    }
}
