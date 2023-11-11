using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rui : BaseEnemy
{
    private float knockBackForce = 1500f;
    private float bumpDamage = 10f;
    private float bumpDistance = 1.5f;
    private IEnemyTarget Target;
    private bool isInBumpRange;
    private bool isBumping;
    private bool isRageing;
    private bool hasRaged;
    private bool hasSpawned;
    private bool isEntering;
    private bool isWalking;
    private bool isBlinking = false;
    private float blinkStartTime;
    AudioSource musicAudio;
    private float musicStartVolume;
    private bool isHalfHealth => Health <= MaxHealth / 2 && Health > 0;
    [SerializeField]
    protected List<CustomAudio> ImpactSounds, BumpSounds, RageSounds, LaughtSounds, StepSounds;
    protected AttackTrigger BumpTrigger;
    private CameraManagement CameraManagement;
    protected override bool isIdle => base.isIdle && !isBumping && !isRageing && !isEntering;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Rui;
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 1000f;
        Damage = 20f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;
        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;
        BumpTrigger = transform.Find("BumpArea").GetComponent<AttackTrigger>();
        BumpTrigger.OnTagTriggered += OnTargetHit;
        CameraManagement = Camera.main.GetComponent<CameraManagement>();
        musicAudio = GameObject.Find("Screen").GetComponent<AudioSource>();
        musicStartVolume = musicAudio.volume;
        base.Start();
        WavesManager.Instance.CurrentWave.CanSpawn = false;
        HealthBar.AnimationSpeed = 30f;
    }

    protected override void Update()
    {
        if (!IsAlive || isDying)
        {
            Animation();
            return;
        }

        Target = GetClosestTarget();

        if (!hasSpawned && !isEntering && (isIdle || isRunning) && !isBumping)
            StartEntranceAnimation();

        if (isWalking)
        {
            float targetDir = Target.transform.position.x < transform.position.x ? -1 : 1;
            transform.Translate(new Vector2(targetDir * 0.5f * Time.unscaledDeltaTime, 0));
        }

        if (isHalfHealth && !isRageing && !hasRaged && (isIdle || isRunning) && !isBumping && hasSpawned)
        {
            isRunning = false;
            isAttacking = false;
            isBumping = false;
            Animator.ResetTrigger("Attack");
            Animator.ResetTrigger("Bump");
            StartRageAnimation();
        }

        if (isBlinking)
        {
            float timeLeft = 2 - (Time.unscaledTime - blinkStartTime);
            float timePercentage = 1 - (timeLeft / 2);
            float colorPercentage = Mathf.PingPong(30 * timePercentage * (timePercentage / 1.5f), 1.0f);
            Color color = Color.Lerp(Color.white, Constants.Colors.RedMoney, colorPercentage);

            SpriteRenderer.color = color;

            if (Time.unscaledTime - blinkStartTime >= 2)
            {
                isBlinking = false;
                SpriteRenderer.color = Color.white;
            }
        }

        if (Target != null)
        {
            float distanceX = Mathf.Abs(Target.transform.position.x - transform.position.x);
            isInBumpRange = distanceX <= bumpDistance;
        }

        if (isInBumpRange && !isBumping && !isAttacking && !isRageing && hasSpawned)
            BumpAttack(Target);

        if (IsInAttackRange && !isBumping && !isAttacking && !isRageing && hasSpawned)
            StartAttack(Target);

        HealthBar.transform.position = transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 1.7f, 0);

        Animation();

        if ((isBumping || isAttacking) && Target != null)
        {
            var targetDirection = Target.transform.position.x - transform.position.x;
            FlipEnemy(Mathf.Sign(targetDirection));
        }
    }
    /// <summary>
    /// Função que inicia o ataque do bump.
    /// </summary>
    /// <param name="target">O alvo que está atacando.</param>
    private void BumpAttack(IEnemyTarget target)
    {
        if (target == null)
            return;
        HitTargetsIds.Clear();
        isBumping = true;
    }
    /// <summary>
    /// Função chamada pela animação do bump desse inimigo no frame em que há contato com o alvo.
    /// </summary>
    public void OnBumpHit()
    {
        if (Target == null)
            return;

        BumpSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        BumpTrigger.gameObject.SetActive(true);
        StartCoroutine(DeactivateBumpTrigger(0.1f));
    }
    /// <summary>
    /// Função ao terminar o bump.
    /// </summary>
    public void OnBumpEnd()
    {
        isBumping = false;
    }
    protected virtual IEnumerator DeactivateBumpTrigger(float duration)
    {
        yield return new WaitForSeconds(duration);
        BumpTrigger.gameObject.SetActive(false);
    }

    protected override void OnAttackHit()
    {
        ImpactSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        if (isHalfHealth)
            HitTargetsIds.Clear();
        StartCoroutine(CameraManagement.ShakeCameraEffect(500, 1));
        base.OnAttackHit();
    }
    protected override void OnTargetHit(Collider2D targetCollider)
    {
        IEnemyTarget target = targetCollider.GetComponent<IEnemyTarget>();
        if (target == null)
            return;
        int targetInstanceId = target.gameObject.GetInstanceID();
        if (HitTargetsIds.Contains(targetInstanceId))
            return;

        var _pushForce = knockBackForce;
        if (isBumping)
            Target.TakeDamage(bumpDamage, 1, "Body", this);
        else if (isAttacking)
        {
            _pushForce /= 2;
            base.OnTargetHit(targetCollider);
        }

        HitTargetsIds.Add(targetInstanceId);

        Vector3 direction = new(Mathf.Sign(Target.transform.position.x - transform.position.x), 0.9f);
        if (Target is IKnockBackable knockBackable)
            knockBackable.TakeKnockBack(_pushForce, direction);
    }
    /// <summary>
    /// Função que inicia a animação de Spawn.
    /// </summary>
    private void StartEntranceAnimation()
    {
        Vector3 location = transform.position;
        Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Animator.SetTrigger("EntranceAnim");
        CameraManagement.FocusOnPosition(location, 3f, 1000f);
        isEntering = true;
        isWalking = true;
        StartCoroutine(musicAudio.Fade(0, 1500f));
        MenuController.Instance.CanPause = false;
        Time.timeScale = 0;
    }
    /// <summary>
    /// Evento chamada pela animação quando o Rui começa a rir.
    /// </summary>
    public void OnRuiLaughtStart()
    {
        LaughtSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
    }
    /// <summary>
    /// Evento chamada pela animação quando o Rui pisa no chão.
    /// </summary>
    public void OnRuiStep()
    {
        StartCoroutine(CameraManagement.ShakeCameraEffect(250, 1));
        StepSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
    }
    /// <summary>
    /// Função chamada pela animação de Spawn no frame em que o inimigo termina a animação.
    /// </summary>
    public void OnEntranceEnd()
    {
        Animator.updateMode = AnimatorUpdateMode.Normal;
        MenuController.Instance.CanPause = true;
        hasSpawned = true;
        isEntering = false;
        Time.timeScale = 1;
        CameraManagement.Unfocus();
        StartCoroutine(musicAudio.Fade(musicStartVolume, 2000f));
        WavesManager.Instance.CurrentWave.CanSpawn = true;
    }
    /// <summary>
    /// Evento chamada pela animação quando o Rui começa andar.
    /// </summary>
    public void OnWalkStart()
    {
        isWalking = true;
    }
    /// <summary>
    /// Evento chamada pela animação quando o Rui termina de andar.
    /// </summary>
    public void OnWalkEnd()
    {
        isWalking = false;
    }
    /// <summary>
    /// Função que inicia a animação de Rage.
    /// </summary>
    private void StartRageAnimation()
    {
        Vector3 location = transform.position;
        Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Animator.SetTrigger("RageAnim");
        CameraManagement.FocusOnPosition(location, 3f, 1000f);
        isRageing = true;
        MenuController.Instance.CanPause = false;
        Time.timeScale = 0;
    }
    /// <summary>
    /// Função chamada pela animação de Rage no frame em que o inimigo começa a animação.
    /// </summary>
    public void OnRuiRageStart()
    {
        RageSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        isBlinking = true;
        blinkStartTime = Time.unscaledTime;
    }
    /// <summary>
    /// Função chamada pela animação de Rage no frame em que o inimigo começa a gritar.
    /// </summary>
    public void OnRuiRageScreamStart()
    {
        StartCoroutine(CameraManagement.ShakeCameraEffect(2000, 1));
    }
    /// <summary>
    /// Função chamada pela animação de Rage no frame em que o inimigo termina a animação.
    /// </summary>
    public void OnRageEnd()
    {
        Animator.updateMode = AnimatorUpdateMode.Normal;
        isRageing = false;
        hasRaged = true;
        MenuController.Instance.CanPause = true;
        Time.timeScale = 1;
        CameraManagement.Unfocus();
    }

    protected override void SyncAnimationStates()
    {
        if (hasSpawned)
        {
            if (isBumping) Animator.SetTrigger("Bump");
            else Animator.ResetTrigger("Bump");
        }

        Animator.SetBool("isIdle", isIdle);
        Animator.SetBool("isRunning", isRunning);

        if (isHalfHealth && hasRaged && hasSpawned)
        {
            if (isAttacking) Animator.SetTrigger("FlipAttack");
            else Animator.ResetTrigger("FlipAttack");
        }
        if (!isHalfHealth && !hasRaged && hasSpawned)
        {
            if (isAttacking) Animator.SetTrigger("Attack");
            else Animator.ResetTrigger("Attack");
        }

        if (isDying) Animator.SetTrigger("Die");
        else Animator.ResetTrigger("Die");
    }
}
