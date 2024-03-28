using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rute : BaseEnemy, IKnockBackable
{
    private bool isMovingLeft = true;
    private float maxDistanceRunning = 5f;
    private bool isRunningAround = true;
    private float BurningEffectDurationMs = 3000f;
    private float SelfBurningEffectDurationMs = 3000f;
    private float BurningEffectTickIntervalMs = 500f;
    private float SelfBurningEffectTickIntervalMs = 1000f;
    private float SelfDamage = 3f;
    public IPlayerTarget EnemyOwner { get; set; }
    private IEnemyTarget Target;
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

        base.Update();

        ApplyBurningEffectToSelf();

        Animation();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.CompareTag("Environment"))
        {
            isMovingLeft = !isMovingLeft;
        }
        if (!isDying)
            AttackTrigger.gameObject.SetActive(true);
            StartCoroutine(DeactivateAttackTrigger(0.1f));
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

    }

    /// <summary>
    /// Cria o efeito de queimadura no alvo.
    /// </summary>
    /// <param name="parent">O pai do efeito (alvo).</param>
    /// <returns>O objeto criado.</returns>
    private GameObject CreateBurningEffect(Transform parent, bool selfDamage = false)
    {
        var burnEffectObj = new GameObject("BurningEffect");
        var burningEffect = burnEffectObj.AddComponent<BurningEffect>();
        if (selfDamage)
            burningEffect.TickDamage = SelfDamage;
        else
            burningEffect.TickDamage = Damage;
        burnEffectObj.transform.SetParent(parent);

        return burnEffectObj;
    }

    protected override void OnTargetHit(Collider2D targetCollider)
    {
        IEnemyTarget target = targetCollider.GetComponent<IEnemyTarget>();

        if (target == null || isDying)
            return;

        var burnFX = target.transform.GetComponentInChildren<BurningEffect>();
        if (burnFX == null)
        {
            var burnEffectObj = CreateBurningEffect(target.transform, false);
            burnFX = burnEffectObj.GetComponent<BurningEffect>();
        }

        if (target != null)
            burnFX.PlayerOwner = target;
      
        AttackHitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        burnFX.SetEffect(BurningEffectDurationMs, BurningEffectTickIntervalMs);
    }

    private void ApplyBurningEffectToSelf()
    {
        if (!isDying)
        {
            var burnFX = GetComponentInChildren<BurningEffect>();

            if (burnFX == null)
            {
                var burnEffectObj = CreateBurningEffect(transform, true);
                burnFX = burnEffectObj.GetComponent<BurningEffect>();
            }

            burnFX.EnemyOwner = EnemyOwner;
            burnFX.SetEffect(SelfBurningEffectDurationMs, SelfBurningEffectTickIntervalMs);
        }
    }

}
