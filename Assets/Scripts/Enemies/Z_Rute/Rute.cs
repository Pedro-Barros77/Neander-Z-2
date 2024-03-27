using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rute : BaseEnemy, IKnockBackable
{
    private IEnemyTarget Target;
    private bool isMovingLeft = true;
    private float maxDistanceRunning = 5f;
    private bool isRunningAround = true;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Rute;
        MovementSpeed = 2.2f;
        AccelerationSpeed = 1f;
        Health = 30f;
        Damage = 6f;
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
        if (Target == null)
            UpdateDirection();

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
        if (closestTarget != null && IsInAttackRange)
            StartAttack(closestTarget);

        if (HealthBar != null)
            HealthBar.transform.position = transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 1.7f, 0);

        //if (isAttacking && closestTarget != null)
        //{
        //    var targetDirection = closestTarget.transform.position.x - transform.position.x;
        //    FlipEnemy(Mathf.Sign(targetDirection));
        //}

        Animation();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Environment"))
        {
            isMovingLeft = !isMovingLeft;
        }
    }

    protected override void Movement(IEnemyTarget target)
    {
        if (Target == null)
            return;

        if (!IsAlive || isDying)
            return;
        float distanceX = Vector3.Distance(new Vector3(transform.position.x, 0), new Vector3(Target.transform.position.x, 0));
        if (distanceX > maxDistanceRunning)
            UpdateDirection();

        if (Mathf.Abs(RigidBody.velocity.x) < MovementSpeed)
        {
            RigidBody.velocity += new Vector2(isMovingLeft ? -1 : 1 * AccelerationSpeed, 0);
        }
    }

    private void UpdateDirection()
    {
        Target = GetClosestTarget();
        if (Target != null)
            isMovingLeft = Target.transform.position.x < transform.position.x;
    }

    protected override void StartAttack(IEnemyTarget target)
    {
        if (isAttacking)
            return;

        if (target == null)
            return;

        isAttacking = true;
        HitTargetsIds.Clear();

        AttackStartSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);

    }
    protected override void OnTargetHit(Collider2D targetCollider)
    {
        base.OnTargetHit(targetCollider);
    }

    protected override void OnAttackHit()
    {
        base.OnAttackHit();
    }

    protected override void SyncAnimationStates()
    {
        isAttacking = false;
        base.SyncAnimationStates();
    }

}
