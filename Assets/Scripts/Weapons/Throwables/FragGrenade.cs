using System.Linq;
using UnityEngine;

public class FragGrenade : BaseThrowable
{
    [SerializeField]
    public GameObject ExplosionPrefab;

    readonly string[] IgnoreBodyPartsNames = { "Plate" };
    readonly float ExplosionPushForce = 2200;

    protected override void Awake()
    {
        Type = ThrowableTypes.FragGrenade;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        TargetLayerMask += LayerMask.GetMask("Player");
    }


    protected override void Detonate()
    {
        Explode();

        base.Detonate();
    }

    /// <summary>
    /// Cria uma explosão e aplica dano em todos os inimigos dentro do raio de efeito.
    /// </summary>
    private void Explode()
    {
        var hitPosition = transform.position;

        var explosion = Instantiate(ExplosionPrefab, hitPosition, Quaternion.identity);
        explosion.transform.localScale = Vector3.one * Data.EffectSpriteSize;

        var hitObjects = Physics2D.OverlapCircleAll(hitPosition, EffectMinRange, TargetLayerMask);

        var enemiesHit = hitObjects.Select(x => new { target = x.GetComponentInParent<IPlayerTarget>(), collider = x }).Where(x => x.target != null).ToList();
        var playersHit = hitObjects.Select(x => new { target = x.GetComponentInParent<IEnemyTarget>(), collider = x }).Where(x => x.target != null).ToList();

        foreach (var hit in enemiesHit)
        {
            IPlayerTarget target = hit.target;

            Collider2D targetCollider = hit.collider;

            var enemyHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(enemyHitPoint, transform.position);

            if (distance > EffectMinRange)
                continue;

            if (!target.IsAlive)
                continue;

            SetTargetHit();
            int targetId = target.transform.GetInstanceID();

            if (PiercedTargetsIds.Contains(targetId))
                continue;

            PiercedTargetsIds.Add(targetId);

            if (target is IKnockBackable knockBackable)
            {
                Vector3 direction = target.transform.position - transform.position;
                knockBackable.TakeKnockBack(ExplosionPushForce, direction.normalized);
            }

            var clampedDistance = Mathf.Clamp(distance, EffectMaxRange, EffectMinRange);
            var percentage = (clampedDistance - EffectMaxRange) / (EffectMinRange - EffectMaxRange);

            float damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

            target.TakeDamage(damage, HeadshotMultiplier, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name, PlayerOwner);
            target.OnPointHit(enemyHitPoint, -transform.right, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name);
        }

        foreach (var hit in playersHit)
        {
            IEnemyTarget target = hit.target;

            Collider2D targetCollider = hit.collider;

            var playerHitPoint = targetCollider.ClosestPoint(transform.position);
            var distance = Vector2.Distance(playerHitPoint, transform.position);

            if (distance > EffectMinRange)
                continue;

            if (!target.IsAlive)
                continue;

            SetTargetHit();
            int targetId = target.gameObject.GetInstanceID();

            if (PiercedTargetsIds.Contains(targetId))
                continue;

            PiercedTargetsIds.Add(targetId);

            if (target is IKnockBackable knockBackable)
            {
                Vector3 direction = target.transform.position - transform.position;
                knockBackable.TakeKnockBack(ExplosionPushForce, direction.normalized);
            }

            var clampedDistance = Mathf.Clamp(distance, EffectMaxRange, EffectMinRange);
            var percentage = (clampedDistance - EffectMaxRange) / (EffectMinRange - EffectMaxRange);

            float damage = Mathf.Lerp(TotalDamage, MinDamage, percentage);

            target.TakeDamage(damage, HeadshotMultiplier, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name, null, selfDamage: true);
            target.OnPointHit(playerHitPoint, -transform.right, IgnoreBodyPartsNames.Contains(targetCollider.name) ? "Body" : targetCollider.name);
        }

        Sprite.enabled = false;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.gravityScale = 0;
        Collider.enabled = false;
        StartCoroutine(KillSelfDelayed(2));
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, EffectMinRange);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, EffectMaxRange);
    //}
}
