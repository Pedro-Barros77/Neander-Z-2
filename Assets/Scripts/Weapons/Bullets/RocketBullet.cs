using System.Linq;
using UnityEngine;

public class RocketBullet : Projectile
{
    public float ExplosionKnockbackForce { get; set; }
    public float ExplosionMaxDamageRadius { get; set; }
    public float ExplosionMinDamageRadius { get; set; }
    public float ExplosionSpriteSize { get; set; }
    public bool Exploded { get; private set; }

    [SerializeField]
    public GameObject ExplosionPrefab;

    readonly string[] IgnoreBodyPartsNames = { "Plate" };

    protected override void Start()
    {
        base.Start();
        StartForwardMovement();
        RotateTowardsDirection = true;
    }

    protected override void OnEnemyHit(Collider2D collision)
    {
        if (Exploded)
            return;

        var target = collision.GetComponentInParent<IPlayerTarget>();

        if (target != null)
        {
            var hitPosition = collision.ClosestPoint(transform.position);
            target.TakeDamage(Damage, collision.name);
            target.OnPointHit(hitPosition, -transform.right);
        }

        Explode();
    }

    protected override void OnObjectHit(Collider2D collision)
    {
        if (Exploded)
            return;

        Explode();
    }

    protected override void OnMaxDistanceReach()
    {
        Explode();

        base.OnMaxDistanceReach();
    }

    private void Explode()
    {
        var hitPosition = transform.position;

        var explosion = Instantiate(ExplosionPrefab, hitPosition, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * ExplosionSpriteSize;

        var hitObjects = Physics2D.OverlapCircleAll(hitPosition, ExplosionMinDamageRadius, TargetLayerMask);

        var enemiesHit = hitObjects.Select(x => new { target = x.GetComponentInParent<IPlayerTarget>(), collider = x }).Where(x => x.target != null).ToList();

        foreach (var hit in enemiesHit)
        {
            IPlayerTarget target = hit.target;
            Collider2D targetCollider = hit.collider;

            var enemyHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(enemyHitPoint, transform.position);

            if (distance <= ExplosionMinDamageRadius)
            {
                int targetId = target.transform.GetInstanceID();

                if (!PiercedTargetsIds.Contains(targetId))
                {
                    PiercedTargetsIds.Add(targetId);

                    var clampedDistance = Mathf.Clamp(distance, ExplosionMaxDamageRadius, ExplosionMinDamageRadius);
                    var percentage = (clampedDistance - ExplosionMaxDamageRadius) / (ExplosionMinDamageRadius - ExplosionMaxDamageRadius);

                    Damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

                    target.TakeDamage(Damage, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name);
                    target.OnPointHit(enemyHitPoint, -transform.right);
                }
            }
        }

        Sprite.enabled = false;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.gravityScale = 0;
        Collider.enabled = false;
        Exploded = true;
        StartCoroutine(KillSelfDelayed(2));
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, ExplosionMinDamageRadius);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, ExplosionMaxDamageRadius);
    //}
}