using UnityEngine;

public class Rute : BaseEnemy, IKnockBackable, IBurnable
{
    private readonly float MAX_RUN_DISTANCE = 5f;
    private bool isMovingLeft = true;
    public float BurningEffectDurationMs { get; set; } = 3000f;
    public float BurningEffectTickIntervalMs { get; set; } = 500f;
    public float SelfBurningEffectTickIntervalMs { get; set; } = 500f;
    public float SelfDamage { get; set; } = 3f;
    public float FloorFlameDamage { get; set; } = 3f;
    [SerializeField]
    public GameObject FireFlamesPrefab;
    private IEnemyTarget Target;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Rute;
        MovementSpeed = 2.2f;
        AccelerationSpeed = 1f;
        Health = 40f;
        Damage = 6f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;

        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;

        base.Start();

        HealthBar.AnimationSpeed = 5f;

        ApplyBurningEffectToSelf();
    }

    protected override void Update()
    {
        if (Target == null)
            UpdateDirection();

        base.Update();

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
        if (distanceX > MAX_RUN_DISTANCE)
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
            burnFX.SetOwner(target);

        AttackHitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        burnFX.SetEffect(BurningEffectDurationMs, BurningEffectTickIntervalMs);
    }

    public override void TakeDamage(TakeDamageProps props)
    {
        if(props.DamageType == DamageTypes.Fire && !props.IsSelfDamage(this))
            return;

        base.TakeDamage(props);
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

            burnFX.SetOwner(this);
            burnFX.SetEffect(1000, SelfBurningEffectTickIntervalMs, true);
            burnFX.LockReset = true;
        }
    }

    public GameObject InstantiateMolotovPrefab()
    {
        var molotovPrefab = Resources.Load<GameObject>($"Prefabs/Weapons/Throwables/{ThrowableTypes.Molotov}");
        GameObject molotovObj = Instantiate(molotovPrefab, transform.parent);
        molotovObj.transform.position = transform.position;
        molotovObj.name = ThrowableTypes.Molotov.ToString();
        molotovObj.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        var molotov = molotovObj.GetComponent<Molotov>();
        molotov.HitSounds.Clear();
        molotov.StartSounds.Clear();
        molotov.EnemyOwner = this;
        molotov.Data = Instantiate(molotov.Data);
        molotov.Data.EffectDurationMs = 8000f;
        molotov.Data.Damage = FloorFlameDamage;
        var rb = molotovObj.GetComponent<Rigidbody2D>();
        var collider = molotovObj.GetComponent<Collider2D>();
        rb.isKinematic = false;
        collider.enabled = true;
        return molotovObj;
    }

    public void OnDieHitGround()
    {
        InstantiateMolotovPrefab();
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
