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

            var damageProps = new TakeDamageProps(DamageTypes.Impact, Damage, PlayerOwner, HeadshotMultiplier)
                .WithBodyPart(collision.name)
                .WithHitPosition(hitPosition)
                .WithHitEffectDirection(-transform.right);

            target.TakeDamage(damageProps);
        }
    }
}
