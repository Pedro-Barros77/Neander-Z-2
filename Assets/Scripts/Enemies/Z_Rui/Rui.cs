using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rui : BaseEnemy
{
    private float knockBackForce = 1500f;
    private float bumpDamage = 10f;
    private float bumpDistance = 2f;
    private IEnemyTarget Target;
    private bool isInBumpRange;
    private bool isBumping;
    private bool isHalfHealth => Health <= MaxHealth / 2;
    [SerializeField]
    protected List<CustomAudio> ImpactSounds, BumpSounds;
    protected AttackTrigger BumpTrigger;
    private CameraManagement CameraManagement;
    protected override bool isIdle => base.isIdle && !isBumping;
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

        base.Start();

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
        if (Target != null)
        {
            float distanceX = Mathf.Abs(Target.transform.position.x - transform.position.x);
            isInBumpRange = distanceX <= bumpDistance;
        }

        if (isInBumpRange && !isBumping && !isAttacking)
            BumpAttack(Target);

        if (IsInAttackRange && !isBumping && !isAttacking)
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

        BumpSounds.PlayRandomIfAny(AudioSource);

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
        ImpactSounds.PlayRandomIfAny(AudioSource);
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
            Target.TakeDamage(bumpDamage, "Body");
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

    protected override void SyncAnimationStates()
    {
        if (isBumping) Animator.SetTrigger("Bump");
        else Animator.ResetTrigger("Bump");

        Animator.SetBool("isIdle", isIdle);
        Animator.SetBool("isRunning", isRunning);

        if (isHalfHealth)
        {
            if (isAttacking) Animator.SetTrigger("FlipAttack");
            else Animator.ResetTrigger("FlipAttack");
        }
        else
        {
            if (isAttacking) Animator.SetTrigger("Attack");
            else Animator.ResetTrigger("Attack");
        }

        if (isDying) Animator.SetTrigger("Die");
        else Animator.ResetTrigger("Die");
    }
}
