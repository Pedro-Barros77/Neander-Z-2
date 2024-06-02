using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlameThrower : LauncherWeapon
{
    ParticleSystem FlameThrowerFlames;
    [SerializeField]
    ParticleSystem SmallFire;
    [SerializeField]
    private CustomAudio ShootStart, ShootLoop, ShootEnd;
    [SerializeField]
    private AudioSource LoopAudioSource;
    public LayerMask enemyLayer;
    private ParticleSystem.Particle[] particles;
    protected List<int> HitTargetsIds = new();
    protected Dictionary<int, int> TargetsHitCount = new();
    const int HIT_MIN_COUNT_TO_BURN = 5;
    private int FrameCounter = 20;
    public float BurningEffectDurationMs { get; set; } = 7000f;
    public float BurningEffectTickIntervalMs { get; set; } = 500f;
    public const float AttackingCounterMs = 200f;
    public float TickDamageMultiplier = 0.25f;
    bool isLoopingShoot;

    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, -0.08f, 0f);
    }

    protected override void Start()
    {
        base.Start();
        FlameThrowerFlames = GetComponentInChildren<ParticleSystem>();
        particles = new ParticleSystem.Particle[FlameThrowerFlames.main.maxParticles];
        if (IsActive)
            SmallFire.Play();
    }

    protected override void Update()
    {
        base.Update();

        if (MenuController.Instance.IsGamePaused)
            return;

        if (!IsActive)
        {
            SmallFire.Stop();
            return;
        }

        if (lastShotTime + (AttackingCounterMs / 1000) < Time.time)
        {
            FlameThrowerFlames.Stop(true);
        }

        if (MagazineBullets <= 0 || IsReloading)
            SmallFire.Stop();
        HandleSoundEffect();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CheckCollision();
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
        burningEffect.TickDamage = Damage * TickDamageMultiplier;
        burnEffectObj.transform.SetParent(parent);

        return burnEffectObj;
    }

    private void CheckCollision()
    {
        FrameCounter--;

        if (FrameCounter > 0)
            return;
        FrameCounter = 20;

        HitTargetsIds.Clear();
        particles = new ParticleSystem.Particle[FlameThrowerFlames.main.maxParticles];
        int particlesCount = FlameThrowerFlames.GetParticles(particles);
        if (particlesCount > 0)
            WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);

        bool hitAnyEnemy = false;

        for (int i = 0; i < particlesCount; i++)
        {
            ParticleSystem.Particle particle = particles[i];
            //Debug.DrawLine(particle.position, Vector3.zero, Color.blue, 1f);
            particles = particles.Where(x => Vector3.Distance(particle.position, x.position) > 0.7f).ToArray();
            particlesCount = particles.Length;
            Vector3 particlePosition = particle.position;
            float radius = 0.5f;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(particlePosition, radius, enemyLayer);

            if (colliders.Any())
                hitAnyEnemy = true;
            //Debug.DrawLine(particlePosition, particlePosition + new Vector3(radius, 0), Color.blue, 1f);
            //Debug.DrawLine(particlePosition, particlePosition + new Vector3(0, radius), Color.blue, 1f);
            //Debug.DrawLine(particlePosition, particlePosition + new Vector3(-radius, 0), Color.blue, 1f);
            //Debug.DrawLine(particlePosition, particlePosition + new Vector3(0, -radius), Color.blue, 1f);
            foreach (Collider2D collider in colliders)
            {
                if (!collider.CompareTag("Enemy"))
                    continue;

                var target = collider.GetComponentInParent<IPlayerTarget>();

                if (target == null)
                    continue;

                int targetInstanceId = target.gameObject.GetInstanceID();

                if (!TargetsHitCount.ContainsKey(targetInstanceId))
                    TargetsHitCount.Add(targetInstanceId, 0);

                TargetsHitCount[targetInstanceId]++;
                if (TargetsHitCount[targetInstanceId] > HIT_MIN_COUNT_TO_BURN)
                {
                    var burnFX = target.transform.GetComponentInChildren<BurningEffect>();

                    if (burnFX == null)
                    {
                        var burnEffectObj = CreateBurningEffect(target.transform, false);
                        burnFX = burnEffectObj.GetComponent<BurningEffect>();
                    }

                    if (target != null)
                        burnFX.EnemyOwner = target;

                    burnFX.SetEffect(BurningEffectDurationMs, BurningEffectTickIntervalMs);
                }

                if (!HitTargetsIds.Contains(targetInstanceId))
                {
                    target.TakeDamage(Damage, HeadshotMultiplier, collider.name, Player);

                    HitTargetsIds.Add(targetInstanceId);
                }
            }
        }
        TargetsHitCount = TargetsHitCount.Where(x => HitTargetsIds.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);

        if (hitAnyEnemy)
            WavesManager.Instance.CurrentWave.HandlePlayerAttack(0, 1);
    }

    public override IEnumerable<GameObject> Shoot()
    {
        if (!CanShoot())
        {
            isShooting = false;
            return Enumerable.Empty<GameObject>();
        }
        Data.MagazineBullets -= 5;
        FlameThrowerFlames.Play(true);
        isShooting = true;
        lastShotTime = Time.time;

        return Enumerable.Empty<GameObject>();
    }
    public override bool CanShoot()
    {
        if (MagazineBullets <= 0 && !Constants.GetActionDown(InputActions.Shoot))
            return false;

        return base.CanShoot();
    }

    public override bool Reload()
    {
        bool reloaded = base.Reload();
        if (reloaded)
        {
            isLoopingShoot = false;
            if (isShooting || Constants.GetAction(InputActions.Shoot))
                StopShooting();
        }


        return reloaded;
    }

    public override void OnReloadEnd()
    {
        base.OnReloadEnd();
        SmallFire.Play();
        if (isShooting || Constants.GetAction(InputActions.Shoot))
        {
            isLoopingShoot = true;
            StartShooting();
        }
    }

    public override void OnShootEnd()
    {
        base.OnShootEnd();
        if (Constants.GetAction(InputActions.Shoot))
            isLoopingShoot = true;
    }

    public override bool BeforeSwitchWeapon()
    {
        if (Constants.GetAction(InputActions.Shoot))
            StopShooting();
        SmallFire.Stop();
        FlameThrowerFlames.Stop(true);
        return base.BeforeSwitchWeapon();
    }

    public override void AfterSwitchWeaponBack()
    {
        SmallFire.Play();
        base.AfterSwitchWeaponBack();
    }

    private void HandleSoundEffect()
    {
        if (NeedsReload() || IsReloading)
        {
            if (isShooting)
                StopShooting();
            return;
        }

        if (Constants.GetActionDown(InputActions.Shoot))
            StartShooting();

        if (Constants.GetActionUp(InputActions.Shoot))
            StopShooting();
    }

    private void StartShooting()
    {
        ShootStart.PlayIfNotNull(AudioSource, AudioTypes.Player);
        LoopAudioSource.volume = ShootLoop.Volume * MenuController.Instance.PlayerVolume * Constants.LoopSoundVolumeMultiplier;
    }

    private void StopShooting()
    {
        ShootEnd.PlayIfNotNull(AudioSource, AudioTypes.Player);
        isLoopingShoot = false;
        LoopAudioSource.volume = 0;
    }

    protected override void SyncAnimationStates()
    {
        Animator.SetBool("isLoopingShoot", isLoopingShoot);
        base.SyncAnimationStates();
    }
}
