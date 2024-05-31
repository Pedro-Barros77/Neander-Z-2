using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rui : BaseEnemy, IBurnable
{
    private float knockBackForce = 1500f;
    private float bumpDamage = 10f;
    private float bumpDistance = 1.5f;
    private float BumpDamageMultiplier = 0.5f;
    private IEnemyTarget Target;
    private bool isInBumpRange;
    private bool isBumping;
    private bool isRageing;
    private bool hasRaged;
    private float JumpDamageMultiplier = 2.5f;
    private float RageRunSpeedMultiplier = 1.5f;
    private float RageAttackSpeedMultiplier = 1.5f;
    private bool hasSpawned;
    private bool isEntering;
    private bool isWalking;
    private bool isBlinking = false;
    private float blinkStartTime;
    AudioSource musicAudio;
    private float musicStartVolume;
    private bool isHalfHealth => Health <= MaxHealth / 2 && Health > 0;
    [SerializeField]
    protected List<CustomAudio> ImpactSounds, BumpSounds, RageSounds, LaughtSounds, StepSounds, JumpSounds, JumpChargeSounds;
    protected AttackTrigger BumpTrigger, JumpTrigger;
    private CameraManagement CameraManagement;
    private bool isJumping;
    private bool isGettingUp;
    private bool JumpAfterAttack;
    private float JumpHeight = 200f;
    private float JumpStartAnimAltitude = 10f;
    private float PlayerLimitDistance = 18f;
    public float JumpAttackChance { get; set; } = 0.3f;
    public float JumpAttackAttemptDelayMs { get; set; } = 6000f;
    private float DistanceFromPlayerX;
    [SerializeField]
    SpriteRenderer CircleShadowPreview;
    private LevelData LevelData;
    protected override bool isIdle => base.isIdle && !isBumping && !isRageing && !isEntering && !isJumping && !isGettingUp;
    protected override void Start()
    {
        Type = EnemyTypes.Z_Rui;
        MovementSpeed = 0.5f;
        AccelerationSpeed = 1f;
        Health = 2000f;
        Damage = 20f;
        KillScore = 53;
        HeadshotScoreMultiplier = 1.5f;
        DeathFadeOutDelayMs = 5000f;
        DamageSoundVolume = 0.3f;
        AttackHitSoundVolume = 0.6f;
        DeathSoundVolume = 0.7f;
        BumpTrigger = transform.Find("BumpArea").GetComponent<AttackTrigger>();
        BumpTrigger.OnTagTriggered += OnTargetHit;
        JumpTrigger = transform.Find("JumpArea").GetComponent<AttackTrigger>();
        JumpTrigger.OnTagTriggered += OnTargetHit;
        CameraManagement = Camera.main.GetComponent<CameraManagement>();
        musicAudio = GameObject.Find("Screen").GetComponent<AudioSource>();
        musicStartVolume = musicAudio.volume;
        base.Start();
        WavesManager.Instance.CurrentWave.CanSpawn = false;
        HealthBar.AnimationSpeed = 60f;
        HealthBar.UseShadows = true;
        HealthBar.UseOutline = true;
        HealthBar.transform.localScale = new(HealthBar.transform.localScale.x * 0.7f, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);

        LevelData = GameObject.Find("Environment").GetComponent<LevelData>();
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
            StartCoroutine(JumpAttackTimer());
        }

        if (isHalfHealth && isJumping && RigidBody.velocity.y < 0 && transform.position.y <= JumpStartAnimAltitude)
            Animator.SetFloat("JumpAnimSpeed", 1f);

        if (isJumping)
            UpdadeJumpShadow();

        if (JumpAfterAttack && !isAttacking && !isBumping)
        {
            JumpAttack(Target);
            JumpAfterAttack = false;
        }

        if (Target != null)
            DistanceFromPlayerX = Vector3.Distance(new Vector3(transform.position.x, 0), new Vector3(Target.transform.position.x, 0));
        if (isHalfHealth && DistanceFromPlayerX > PlayerLimitDistance && !isAttacking && !isBumping && !isJumping && hasRaged)
            JumpAttack(Target);

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

        if (isInBumpRange && !isBumping && !isAttacking && !isRageing && !isJumping && hasSpawned && !isGettingUp)
            BumpAttack(Target);

        if (IsInAttackRange && !isBumping && !isAttacking && !isRageing && !isJumping && hasSpawned && !isGettingUp)
            StartAttack(Target);

        HealthBar.transform.position = transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 1.7f, 0);

        Animation();

        if ((isBumping || isAttacking) && Target != null)
        {
            var targetDirection = Target.transform.position.x - transform.position.x;
            FlipEnemy(Mathf.Sign(targetDirection));
        }
    }
    protected override void Movement(IEnemyTarget target)
    {
        if (target == null)
            return;

        var targetDir = target.transform.position.x < transform.position.x ? -1 : 1;
        if (transform.position.x < LevelXLimit.x)
            targetDir = 1;
        else if (transform.position.x > LevelXLimit.y)
            targetDir = -1;

        if (Mathf.Abs(RigidBody.velocity.x) < MovementSpeed && !IsInAttackRange && !isAttacking && !hasRaged)
            RigidBody.velocity += new Vector2(targetDir * AccelerationSpeed, 0);

        if ((Mathf.Abs(RigidBody.velocity.x) < MovementSpeed * RageRunSpeedMultiplier || (targetDir != Mathf.Sign(RigidBody.velocity.x) && isJumping)) && (!IsInAttackRange || isJumping) && !isAttacking && hasRaged)
            RigidBody.velocity += new Vector2(targetDir * AccelerationSpeed, 0);
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
    /// <summary>
    /// Função responsável por desativar o trigger do bump após um tempo.
    /// </summary>
    /// <param name="duration">O tempo que demora para desativar o trigger.</param>
    /// <returns></returns>
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
        StartCoroutine(CameraManagement.ShakeCameraEffect(500, 1.4f));
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
        {
            float _damageMultiplier = isHalfHealth ? BumpDamageMultiplier : 1f;
            Target.TakeDamage(bumpDamage * _damageMultiplier, 1, "Body", this);
        }
        else if (isJumping)
        {
            _pushForce *= 2;
            Target.TakeDamage(Damage * JumpDamageMultiplier, 1, "Body", this);
        }
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
    /// Função responsável por atualizar a sombra na queda do pulo.
    /// </summary>
    void UpdadeJumpShadow()
    {
        if (RigidBody.velocity.y >= 0)
            return;
        if (!isJumping)
            return;
        float maxHeigth = 50f;
        float floorHeigth = LevelData.BottomRightSpawnLimit.y;
        float percentege = 1 - (transform.position.y / maxHeigth);
        CircleShadowPreview.color = Color.Lerp(new(0.1f, 0.1f, 0.1f, 0), new(0.1f, 0.1f, 0.1f, 0.7f), percentege);
        CircleShadowPreview.transform.localScale = new Vector3(1, 1, 1) * (percentege + 0.3f);
        CircleShadowPreview.transform.position = new Vector3(CircleShadowPreview.transform.position.x, floorHeigth - 1);
    }
    /// <summary>
    /// Loop responsável por verificar se pode rodar o dado para o Rui pular.
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpAttackTimer()
    {
        while (IsAlive)
        {
            if (isJumping || isAttacking || isBumping || JumpAfterAttack || isGettingUp)
                yield return null;

            yield return new WaitForSeconds(JumpAttackAttemptDelayMs / 1000f);
            RollJumpAttackDice();
        }
    }
    /// <summary>
    /// Função responsável por rolar o dado para decidir se o Rui vai pular para atacar.
    /// </summary>
    void RollJumpAttackDice()
    {
        if (isJumping || isAttacking || isBumping || JumpAfterAttack || isGettingUp)
            return;

        var dice = Random.Range(0f, 1f);
        bool attack = dice < JumpAttackChance;
        if (attack)
            JumpAttack(Target);
    }
    /// <summary>
    /// Função que inicia o ataque do pulo.
    /// </summary>
    /// <param name="target">O alvo que está atacando.</param>
    private void JumpAttack(IEnemyTarget target)
    {
        if (target == null)
            return;
        if (!isJumping)
            JumpChargeSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        HitTargetsIds.Clear();
        isJumping = true;
        isRunning = false;
    }
    /// <summary>
    /// Função responsável por desativar o trigger do pulo após um tempo.
    /// </summary>
    /// <param name="duration">O tempo que demora para desativar o trigger.</param>
    /// <returns></returns>
    protected virtual IEnumerator DeactivateJumpTrigger(float duration)
    {
        yield return new WaitForSeconds(duration);
        JumpTrigger.gameObject.SetActive(false);
    }
    /// <summary>
    /// Função chamada pela animação do pulo do Rui no frame em que ele pula.
    /// </summary>
    public void OnRuiJump()
    {
        float distanceFromTarget = Target.transform.position.x - transform.position.x;
        RigidBody.AddForce(new Vector2(distanceFromTarget * 0.8f, JumpHeight), ForceMode2D.Impulse);
        MovementSpeed *= 4;
        Animator.SetFloat("JumpAnimSpeed", 0f);
        StartCoroutine(EnableJumpTriggerDelayed());
    }
    /// <summary>
    /// Função chamada pela animação do jump desse inimigo no frame em que há contato com o alvo.
    /// </summary>
    public void OnJumpHit()
    {
        if (Target == null)
            return;
        StartCoroutine(CameraManagement.ShakeCameraEffect(1000, 2.5f));
        JumpSounds.PlayRandomIfAny(AudioSource, AudioTypes.Enemies);
        StartCoroutine(DeactivateJumpTrigger(0.1f));
        MovementSpeed = MaxMovementSpeed;
        CircleShadowPreview.color = new(0, 0, 0, 0);
    }
    /// <summary>
    /// Função ao terminar o Jump.
    /// </summary>
    public void OnJumpEnd()
    {
        isJumping = false;
        isGettingUp = true;
    }
    /// <summary>
    /// Função responsável por ativar o trigger do pulo após um tempo.
    /// </summary>
    /// <returns>Tempo para ativar o trigger.</returns>
    IEnumerator EnableJumpTriggerDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        JumpTrigger.gameObject.SetActive(true);
    }
    /// <summary>
    /// Função chamada pela animação de levantar do Rui ao terminar ela.
    /// </summary>
    public void OnLiftUpEnd()
    {
        isGettingUp = false;
    }
    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();
        if (isHalfHealth && !isJumping && !isBumping && !isAttacking)
            JumpAfterAttack = true;
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
        StartCoroutine(CameraManagement.ShakeCameraEffect(250, 1, false));
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
        Animator.SetFloat("RageRunSpeedMultiplier", RageRunSpeedMultiplier);
        Animator.SetFloat("RageAttackSpeedMultiplier", RageAttackSpeedMultiplier);
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
        StartCoroutine(CameraManagement.ShakeCameraEffect(2000, 1, false));
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

            if (isJumping) Animator.SetTrigger("JumpAttack");
            else Animator.ResetTrigger("JumpAttack");

            if (isGettingUp) Animator.SetTrigger("LiftUp");
            else Animator.ResetTrigger("LiftUp");
        }
        else if (!isHalfHealth && !hasRaged && hasSpawned)
        {
            if (isAttacking) Animator.SetTrigger("Attack");
            else Animator.ResetTrigger("Attack");
        }

        if (isDying) Animator.SetTrigger("Die");
        else Animator.ResetTrigger("Die");
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
