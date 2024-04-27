using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Chainsaw : MeleeWeapon
{
    [SerializeField]
    private AudioSource SecondaryAudioSource, HitAudioSource;

    [SerializeField]
    private CustomAudio StartupSound, IdleLoopSound, RevStartSound, RevLoopSound, CutStartSound, CutLoopSound, CutToRevSound, RevToIdleSound, TurnOffSound;

    bool isCutting, isOn;
    int attackEndMissCounter = 2, attackHitCounter = 5;

    protected override void Awake()
    {
        base.Awake();
        WeaponContainerOffset = new Vector3(0f, -0.18f, 0f);
    }

    protected override void Start()
    {
        base.Start();
        AudioSource.clip = null;
        SecondaryAudioSource.clip = null;
        TurnOn();
    }

    protected override void Update()
    {
        base.Update();

        if (!isOn)
            return;

        if (MagazineBullets <= 0)
            TurnOff();

        if (Constants.GetActionDown(InputActions.Shoot))
            StartRevving();

        if (Constants.GetActionUp(InputActions.Shoot))
        {
            isCutting = false;
            attackEndMissCounter = 3;
            RevLoopSound.PlayIfNotNull(SecondaryAudioSource, AudioTypes.Player, false);
            FadeSounds(RevLoopSound, IdleLoopSound, 0.2f, RevLoopSound.Audio.length * 0.9f, true);
        }

        if (attackEndMissCounter <= 0 && isCutting)
        {
            isCutting = false;
            attackEndMissCounter = 3;
            CutLoopSound.PlayIfNotNull(SecondaryAudioSource, AudioTypes.Player);
            FadeSounds(CutLoopSound, RevLoopSound, 0.4f, CutLoopSound.Audio.length * 0.4f, true);
        }
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
            isOn = true;
            StartRevving();
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
        attackEndMissCounter--;
    }

    protected override void OnTargetHit(Collider2D targetCollider)
    {
        IPlayerTarget target = targetCollider.GetComponentInParent<IPlayerTarget>();
        if (target == null)
            return;

        int targetInstanceId = target.gameObject.GetInstanceID();

        if (HitTargetsIds.Contains(targetInstanceId))
            return;

        if (attackHitCounter <= 0)
        {
            attackHitCounter = 5;
            HitSounds.PlayRandomIfAny(HitAudioSource, AudioTypes.Player);
            return;
        }

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

        if (!isCutting && isOn)
        {
            CutStartSound.PlayIfNotNull(SecondaryAudioSource, AudioTypes.Player);
            FadeSounds(CutStartSound, CutLoopSound, 0.3f, CutStartSound.Audio.length * 0.9f, true);
            isCutting = true;
        }
        attackHitCounter--;
        attackEndMissCounter = 3;
    }

    void TurnOn()
    {
        if (isOn)
            return;

        isOn = true;
        StartupSound.PlayIfNotNull(SecondaryAudioSource, AudioTypes.Player);
        FadeSounds(StartupSound, IdleLoopSound, 0.1f, StartupSound.Audio.length, true);
    }

    void TurnOff()
    {
        if (!isOn)
            return;

        isCutting = false;
        isOn = false;
        IdleLoopSound.PlayIfNotNull(SecondaryAudioSource, AudioTypes.Player);
        FadeSounds(IdleLoopSound, TurnOffSound, 0, 0);
    }

    void StartRevving()
    {
        RevStartSound.PlayIfNotNull(SecondaryAudioSource, AudioTypes.Player, false);
        FadeSounds(RevStartSound, RevLoopSound, 0.3f, RevStartSound.Audio.length * 0.97f, true);
    }

    void FadeSounds(CustomAudio oldSound, CustomAudio newSound, float fadeDuration, float delayMs, bool loop = false)
    {
        StopAllCoroutines();
        AudioSource.volume = 0;
        SecondaryAudioSource.volume = 0;
        StartCoroutine(FadeSounds(SecondaryAudioSource, AudioSource, oldSound, newSound, fadeDuration, delayMs, loop));
    }

    System.Collections.IEnumerator FadeSounds(AudioSource from, AudioSource to, CustomAudio oldSound, CustomAudio newSound, float fadeDuration, float delayMs, bool loop = false)
    {
        yield return new WaitForSeconds(delayMs / 1000);

        float timeElapsed = 0;

        to.clip = newSound.Audio;
        to.loop = loop;
        newSound.PlayIfNotNull(to, AudioTypes.Player, false);

        while (timeElapsed < fadeDuration)
        {
            float t = timeElapsed / fadeDuration;
            to.volume = Mathf.Lerp(0, newSound.Volume, t);
            from.volume = Mathf.Lerp(oldSound.Volume, 0, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        from.Stop();
    }
}
