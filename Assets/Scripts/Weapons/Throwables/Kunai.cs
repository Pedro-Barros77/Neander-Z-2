using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Kunai : BaseThrowable
{
    public readonly float StickThresholdAngle = 10;
    public readonly string[] TagsToStick = new string[] { "Environment", "Enemy" };

    [SerializeField]
    public List<CustomAudio> StickSounds, EnemyHitSounds;

    Transform StickPoint;
    IPlayerTarget StuckEnemyParent;
    bool startedKillSelf;
    Collider2D EnemiesTrigger;

    protected override void Awake()
    {
        Type = ThrowableTypes.Kunai;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        StickPoint = transform.Find("StickPoint");
        EnemiesTrigger = transform.Find("EnemiesTrigger").GetComponent<Collider2D>();
    }

    protected override void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        if (Collided && !startedKillSelf)
        {
            StuckEnemyParent ??= GetComponentInParent<IPlayerTarget>();

            if (StuckEnemyParent != null)
            {
                if (!StuckEnemyParent.IsAlive)
                {
                    Unstick();
                    StartCoroutine(KillSelfDelayed(10));
                    startedKillSelf = true;
                }
            }
            else
            {
                StartCoroutine(KillSelfDelayed(10));
                startedKillSelf = true;
            }
        }

        base.Update();
    }

    public override void Throw()
    {
        base.Throw();

        Collider.enabled = true;
        EnemiesTrigger.enabled = true;
    }

    protected override void OnEnemyHit(Collider2D collision)
    {
        if (CollidedWithEnemy)
            return;

        base.OnEnemyHit(collision);

        var target = collision.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            SetTargetHit();
            EnemyHitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
            var hitPosition = collision.ClosestPoint(transform.position);

            var damageProps = new TakeDamageProps(DamageTypes.Impact, Damage, PlayerOwner, HeadshotMultiplier)
                .WithBodyPart(collision.name)
                .WithHitPosition(hitPosition)
                .WithHitEffectDirection(-transform.right)
                .WithArmorPiercingPercentage(ArmorPiercingPercentage);

            target.TakeDamage(damageProps);
        
            EnemiesTrigger.enabled = false;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Collided && DistanceTraveled > 0.5f)
            CheckStickedToSurface(collision);

        base.OnCollisionEnter2D(collision);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        var target = collision.gameObject.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            StickToSurface(collision.gameObject, collision.ClosestPoint(StickPoint.position));
            transform.SetParent(collision.transform); // Stick the projectile to the surface
        }

        HandleCollision(collision);

        base.OnTriggerEnter2D(collision);
    }

    private void CheckStickedToSurface(Collision2D collision)
    {
        if (!TagsToStick.Any(collision.gameObject.CompareTag))
            return;

        Vector2 normal = collision.contacts[0].normal;
        Vector2 velocity = Rigidbody.velocity;

        float impactAngle = Vector2.Angle(velocity, -normal);
        float relativeImpactAngle = Mathf.Abs(impactAngle - 90);

        if (relativeImpactAngle > StickThresholdAngle)
        {
            StickToSurface(null, new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y));
            StickSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
        }
    }

    private void StickToSurface(GameObject surfaceObj, Vector3? hitPos)
    {
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0f;
        Rigidbody.isKinematic = true;
        Rigidbody.simulated = false;

        if (surfaceObj != null)
            transform.SetParent(surfaceObj.transform);

        if (hitPos.HasValue)
            transform.position = hitPos.Value + (transform.position - StickPoint.transform.position);
    }

    private void Unstick()
    {
        Rigidbody.isKinematic = false;
        Rigidbody.simulated = true;
        transform.SetParent(ProjectilesContainer);
    }
}
