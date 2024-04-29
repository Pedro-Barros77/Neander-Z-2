using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Chainsaw : MeleeWeapon
{
    [SerializeField]
    private AudioSource StartupAudioSource, IdleAudioSource, RunAudioSource, TransitionAudioSource, HitAudioSource;

    [SerializeField]
    private CustomAudio StartupSound, IdleLoopSound, RunStartSound, RunLoopSound, TurnOffSound;

    private float currentEngineAcceleration, accelerationSpeed = 4, runToIdleDeaccSpeed = 1;

    float idleRange, runRange;

    int attackHitCounter = 5; // 5 hits before playing a hit sound
    EngineStates engineState = EngineStates.Off;

    private enum EngineStates
    {
        Off,
        TurningOn,
        Idle,
        IdleToRun,
        Running,
        RunToIdle,
        TurningOff
    }

    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, -0.18f, 0f);
        IdleAudioSource.volume = 0;
        RunAudioSource.volume = 0;
    }

    protected override void Start()
    {
        idleRange = StartupSound.Audio.length;
        runRange = idleRange + RunStartSound.Audio.length;

        base.Start();
        TurnOn();
    }

    protected override void Update()
    {
        base.Update();

        HandleEngineAcceleration();

        if (IsOff)
            return;

        if (MagazineBullets <= 0)
            TurnOff();
    }

    public override IEnumerable<GameObject> Shoot()
    {
        var emptyBullets = Enumerable.Empty<GameObject>();

        if (!CanShoot())
            return emptyBullets;

        AttackTrigger.gameObject.SetActive(true);
        Data.MagazineBullets--;

        AddedTargetHitScore = false;
        HitTargetsIds.Clear();

        WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);
        isShooting = true;
        lastShotTime = Time.time;

        ShootSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);

        return emptyBullets;
    }

    public override bool CanShoot()
    {
        if (!IsActive)
            return false;

        if (isShooting)
            return false;

        if (MagazineBullets <= 0)
            return false;

        var now = Time.time;

        if (IsReloading)
            return false;

        if (IsSwitchingWeapon)
            return false;

        if (ReloadStartTime != null && Time.time - ReloadTimeMs <= ReloadStartTime)
            return false;

        var delayMs = FIRE_RATE_RATIO / FireRate;
        var diff = now - (delayMs / 1000);

        if (lastShotTime != null && diff <= lastShotTime)
            return false;

        return true;
    }

    public override bool Reload()
    {
        bool willReload = base.Reload();

        if (willReload)
            TurnOff();
        return willReload;
    }

    public override void OnReloadEnd()
    {
        base.OnReloadEnd();
        if (Constants.GetAction(InputActions.Shoot))
        {
            engineState = EngineStates.Idle;
            StartRunning();
        }
        else
            TurnOn();
    }

    protected override void SyncAnimationStates()
    {
        Animator.SetFloat("shootSpeed", FireRate / 10);
        Animator.SetFloat("reloadSpeed", 1000 / ReloadTimeMs);

        if (IsReloading) Animator.SetTrigger("Reload");
        else Animator.ResetTrigger("Reload");

        if (isShooting) Animator.SetTrigger("Shoot");
        else Animator.ResetTrigger("Shoot");
    }

    public override bool BeforeSwitchWeapon()
    {
        isShooting = false;
        IsSwitchingWeapon = true;

        Animator.SetFloat("reloadSpeed", 0);

        TurnOff();

        return true;
    }

    public override void AfterSwitchWeaponBack()
    {
        base.AfterSwitchWeaponBack();
        if (MagazineBullets > 0)
            TurnOn();
    }

    public override bool NeedsReload()
    {
        return MagazineBullets <= 0;
    }

    public override void OnShootEnd()
    {
        base.OnShootEnd();
        AttackTrigger.gameObject.SetActive(false);
    }

    protected override void OnTargetHit(Collider2D targetCollider)
    {
        IPlayerTarget target = targetCollider.GetComponentInParent<IPlayerTarget>();
        if (target == null)
            return;

        int targetInstanceId = target.gameObject.GetInstanceID();

        if (HitTargetsIds.Contains(targetInstanceId))
            return;

        Vector2 hitPosition = targetCollider.ClosestPoint(AttackTrigger.transform.position);
        target.TakeDamage(Damage, HeadshotMultiplier, targetCollider.name, Player, hitPosition);
        target.OnPointHit(hitPosition, -transform.right, targetCollider.name);

        if (target.IsAlive)
        {
            if (!AddedTargetHitScore)
                WavesManager.Instance.CurrentWave.HandlePlayerAttack(0, 1);
            AddedTargetHitScore = true;
        }

        HitTargetsIds.Add(targetInstanceId);

        attackHitCounter--;

        if (attackHitCounter <= 0)
        {
            attackHitCounter = 5;
            HitSounds.PlayRandomIfAny(HitAudioSource, AudioTypes.Player);
            return;
        }
    }

    bool IsOff => engineState == EngineStates.Off || engineState == EngineStates.TurningOff;

    void TurnOn()
    {
        if (!IsOff)
            return;

        StartupSound.PlayIfNotNull(StartupAudioSource, AudioTypes.Player);
        engineState = EngineStates.TurningOn;
    }

    void TurnOff()
    {
        if (IsOff)
            return;

        TurnOffSound.PlayIfNotNull(StartupAudioSource, AudioTypes.Player);
        engineState = EngineStates.TurningOff;
    }

    void StartRunning()
    {
        if (IsOff)
            return;

        engineState = EngineStates.IdleToRun;
    }

    void ReleaseAccelerator()
    {
        if (IsOff)
            return;

        engineState = EngineStates.RunToIdle;
    }

    void HandleEngineAcceleration()
    {
        if (engineState == EngineStates.Off)
            return;

        if (Constants.GetActionDown(InputActions.Shoot))
            StartRunning();

        if (Constants.GetActionUp(InputActions.Shoot))
            ReleaseAccelerator();

        IdleAudioSource.volume = 0;
        RunAudioSource.volume = 0;
        IdleAudioSource.pitch = 1;
        RunAudioSource.pitch = 1;

        switch (engineState)
        {
            case EngineStates.TurningOn:
                currentEngineAcceleration += accelerationSpeed * Time.deltaTime;
                if (currentEngineAcceleration >= idleRange)
                {
                    currentEngineAcceleration = idleRange;
                    engineState = EngineStates.Idle;
                }
                break;

            case EngineStates.TurningOff:
                currentEngineAcceleration -= accelerationSpeed * Time.deltaTime;
                if (currentEngineAcceleration <= 0)
                {
                    currentEngineAcceleration = 0;
                    engineState = EngineStates.Off;
                }
                break;

            case EngineStates.Idle:
                currentEngineAcceleration = idleRange;
                IdleAudioSource.volume = 1;
                break;

            case EngineStates.IdleToRun:
                currentEngineAcceleration += accelerationSpeed * Time.deltaTime;
                IdleAudioSource.volume = runRange * 1.26f - currentEngineAcceleration;
                IdleAudioSource.pitch = Mathf.Lerp(1, 1.5f, 1 - (runRange - currentEngineAcceleration));
                RunAudioSource.volume = Mathf.Clamp(1f - (runRange - currentEngineAcceleration), 0.3f, 1);
                RunAudioSource.pitch = Mathf.Lerp(0.6f, 1, 1 - (runRange - currentEngineAcceleration));
                if (currentEngineAcceleration >= runRange)
                {
                    currentEngineAcceleration = runRange;
                    engineState = EngineStates.Running;
                }
                break;

            case EngineStates.Running:
                currentEngineAcceleration = runRange;
                RunAudioSource.volume = 1;
                break;

            case EngineStates.RunToIdle:
                currentEngineAcceleration -= runToIdleDeaccSpeed * Time.deltaTime;
                IdleAudioSource.volume = runRange * 1.26f - currentEngineAcceleration;
                IdleAudioSource.pitch = Mathf.Lerp(1, 1.5f, 1 - (runRange - currentEngineAcceleration));
                RunAudioSource.volume = 1 - (runRange - currentEngineAcceleration);
                RunAudioSource.pitch = Mathf.Lerp(0.6f, 1, 1 - (runRange - currentEngineAcceleration));
                if (currentEngineAcceleration <= idleRange)
                {
                    currentEngineAcceleration = idleRange;
                    engineState = EngineStates.Idle;
                }
                break;
        }
    }
}
