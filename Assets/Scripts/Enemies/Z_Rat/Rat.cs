using UnityEngine;

public class Rat : BaseEnemy, IKnockBackable, IBurnable
{
    private int AttackCounter;
    private int ZigZagCounter;
    private float ZigZagSpeedMultiplier;
    private float ZigZagDistance;
    private bool IsMovingLeft;

    private const int MAX_ATTACK_COUNT = 3;

    private const int MAX_ZIGZAG_COUNT = 5;

    private const float MIN_ZIGZAG_SPEED_MULTIPLIER = 0.6f;
    private const float MAX_ZIGZAG_SPEED_MULTIPLIER = 1.9f;

    private const float MIN_ZIGZAG_DISTANCE = 0.5f;
    private const float MAX_ZIGZAG_DISTANCE = 3;

    private float? ZigZagDestinationX = null;

    protected override void Start()
    {
        Type = EnemyTypes.Z_Rat;
        MovementSpeed = 6f;
        AccelerationSpeed = 1.5f;
        Health = 1f;
        Damage = 4f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        base.Start();

        HealthBar.AnimationSpeed = 5f;
    }
    protected override void Update()
    {
        if (!IsAlive || isDying)
        {
            if (DeathTime + (DeathFadeOutDelayMs / 1000) + 3 < Time.time)
            {
                DeathFadeOutDelayMs = 0;
                StartCoroutine(StartDeathFadeOutCountDown());
            }
            Animation();
            return;
        }

        var closestTarget = GetClosestTarget();
        if (closestTarget != null && IsInAttackRange && ZigZagCounter == 0 && AttackCounter > 0)
            StartAttack(closestTarget);

        if (HealthBar != null)
            HealthBar.transform.position = transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 1.7f, 0);

        if (isAttacking && closestTarget != null)
        {
            var targetDirection = closestTarget.transform.position.x - transform.position.x;
            FlipEnemy(Mathf.Sign(targetDirection));
        }

        Animation();
    }

    protected override void Movement(IEnemyTarget target)
    {
        if (target == null)
            return;

        if (ZigZagCounter == 0 && AttackCounter == 0)
            ZigZagCounter = Random.Range(1, MAX_ZIGZAG_COUNT + 1);

        if (ZigZagCounter > 0)
            ZigZag(target);
        else
        {
            IsMovingLeft = target.transform.position.x < transform.position.x;
            Move();
        }
    }

    void Move()
    {
        float speed = MovementSpeed;
        if (ZigZagCounter > 0)
            speed *= ZigZagSpeedMultiplier;

        if (Mathf.Abs(RigidBody.velocity.x) < speed && (!IsInAttackRange || ZigZagCounter > 0) && !isAttacking)
            RigidBody.velocity += new Vector2((IsMovingLeft ? -1 : 1) * AccelerationSpeed, 0);
    }

    void ZigZag(IEnemyTarget target)
    {
        if (ZigZagCounter == 0)
            return;

        if (ZigZagSpeedMultiplier == 0)
            ZigZagSpeedMultiplier = Random.Range(MIN_ZIGZAG_SPEED_MULTIPLIER, MAX_ZIGZAG_SPEED_MULTIPLIER);

        if (ZigZagDistance == 0)
            ZigZagDistance = Random.Range(MIN_ZIGZAG_DISTANCE, MAX_ZIGZAG_DISTANCE);

        if (ZigZagDestinationX == null)
            ZigZagDestinationX = Random.Range(target.transform.position.x - ZigZagDistance, target.transform.position.x + ZigZagDistance);
        else
        {
            if ((IsMovingLeft && transform.position.x <= ZigZagDestinationX) || (!IsMovingLeft && transform.position.x > ZigZagDestinationX))
            {
                ZigZagCounter--;
                ZigZagDestinationX = null;
                ZigZagSpeedMultiplier = 0;
                ZigZagDistance = 0;
                IsMovingLeft = !IsMovingLeft;

                if (ZigZagCounter == 0)
                    AttackCounter = Random.Range(1, MAX_ATTACK_COUNT + 1);

                Move();
                return;
            }
        }

        IsMovingLeft = ZigZagDestinationX < transform.position.x;

        Move();
    }

    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();
        AttackCounter--;
    }
    public void ActiveBurningParticles(BurningEffect burnFx)
    {
        BodyFlames.StartFire(burnFx, true);
    }

    public void DeactivateFireParticles()
    {
        BodyFlames.StopFire();
    }
}
