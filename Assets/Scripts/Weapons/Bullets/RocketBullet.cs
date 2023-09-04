using System.Linq;
using UnityEngine;

public class RocketBullet : Projectile
{
    public float ExplosionKnockbackForce { get; set; }
    public float ExplosionMaxDamageRadius { get; set; }
    public float ExplosionMinDamageRadius { get; set; }
    public float ExplosionSize { get; set; }
    public bool Exploded { get; private set; }

    [SerializeField]
    public GameObject ExplosionPrefab;

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

        Explode(collision);
    }

    protected override void OnObjectHit(Collider2D collision)
    {
        if (Exploded)
            return;

        Explode(collision);
    }

    private void Explode(Collider2D collision)
    {
        var hitPosition = collision.ClosestPoint(transform.position);

        var explosion = Instantiate(ExplosionPrefab, hitPosition, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * ExplosionSize;

        var hitObjects = Physics2D.OverlapCircleAll(hitPosition, ExplosionMinDamageRadius, TargetLayerMask);

        var enemies = hitObjects.Select(x => x.GetComponentInParent<IPlayerTarget>()).Where(x => x != null).ToList();

        foreach (var enemy in enemies)
        {
            var enemyCollider = enemy.transform.GetComponent<Collider2D>();
            var enemyHitPoint = enemyCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(enemyHitPoint, hitPosition);
            if (distance <= ExplosionMinDamageRadius)
            {
                var damageMultiplier = Mathf.Clamp01((distance - ExplosionMinDamageRadius) / (ExplosionMaxDamageRadius - ExplosionMinDamageRadius));
                Damage *= damageMultiplier;
                enemy.TakeDamage(Damage, collision.name);
                enemy.OnPointHit(enemyHitPoint, -transform.right);
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
