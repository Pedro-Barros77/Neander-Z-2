using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseThrowable : MonoBehaviour
{
    [SerializeField]
    public BaseThrowableData Data;

    #region Data Properties Forwarding

    /// <summary>
    /// O dano causado pela item.
    /// </summary>
    public float Damage => Data.Damage;
    /// <summary>
    /// O dano mínimo causado pelo item.
    /// </summary>
    public float MinDamage => Data.MinDamage;
    /// <summary>
    /// O dano causado quando o item acerta um alvo.
    /// </summary>
    public float HitDamage => Data.HitDamage;
    /// <summary>
    /// O multiplicador de dano causado por um acerto na cabeça.
    /// </summary>
    public float HeadshotMultiplier => Data.HeadshotMultiplier;
    /// <summary>
    /// A força em que o item é arremessado.
    /// </summary>
    public float ThrowForce => Data.ThrowForce;
    /// <summary>
    /// O tempo em millisegundos em que o item é detonado após ser arremessado (ou iniciar o cozimento).
    /// </summary>
    public float FuseTimeoutMs => Data.FuseTimeoutMs;
    /// <summary>
    /// Se o efeito do item é detonado no cimpacto com um inimigo ou objeto
    /// </summary>
    public bool DetonateOnImpact => Data.DetonateOnImpact;
    /// <summary>
    /// Se é iniciada a contagem regressiva do detonamento ao iniciar o cozimento.
    /// </summary>
    public bool StartFuseOnCook => Data.StartFuseOnCook;
    /// <summary>
    /// Se o item deve rotacionar para a direção da velocidade.
    /// </summary>
    public bool RotateToFaceVelocity => Data.RotateToFaceVelocity;
    /// <summary>
    /// A distância em que o efeito do item tem efetividade máxima.
    /// </summary>
    public float EffectMaxRange => Data.EffectMaxRange;
    /// <summary>
    /// A distância em que o efeito do item tem efetividade mínima.
    /// </summary>
    public float EffectMinRange => Data.EffectMinRange;
    /// <summary>
    /// A duração do efeito do item.
    /// </summary>
    public float EffectDurationMs => Data.EffectDurationMs;
    /// <summary>
    /// A duração do efeito do item após o item ser destruído.
    /// </summary>
    public float EffectDecoupledDurationMs => Data.EffectDecoupledDurationMs;
    /// <summary>
    /// Intervalo de tempo em que o efeito do item é causado.
    /// </summary>
    public float EffectTickIntervalMs => Data.EffectTickIntervalMs;
    /// <summary>
    /// O quanto de dano irá ignorar a armadura do alvo. Ex: 100 de dano e 0.5 de ArmorPiercingPercentage, 50 de dano irá direto para o alvo.
    /// </summary>
    public float ArmorPiercingPercentage => Data.ArmorPiercingPercentage;
    public bool Placeable => Data.Placeable;

    #endregion

    #region Properties

    /// <summary>
    /// O tipo de item arremessável.
    /// </summary>
    public ThrowableTypes Type { get; protected set; }
    /// <summary>
    /// O dano total causado pelo item.
    /// </summary>
    public float TotalDamage { get; protected set; }
    /// <summary>
    /// Se o item está sendo cozinhado atualmente.
    /// </summary>
    public bool IsCooking { get; protected set; }
    /// <summary>
    /// Se o item foi arremessado.
    /// </summary>
    public bool IsThrown { get; protected set; }
    /// <summary>
    /// Se o item está ativado e pronto para detonar.
    /// </summary>
    public bool IsActivated { get; protected set; }
    /// <summary>
    /// Se o item foi detonado.
    /// </summary>
    public bool HasDetonated { get; protected set; }
    /// <summary>
    /// A direção em que o jogador está virado, 1 para direita, -1 para esquerda.
    /// </summary>
    public float PlayerFlipDir { get; set; } = 1;
    /// <summary>
    /// Script responsável por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    public PlayerWeaponController PlayerWeaponController { get; set; }
    /// <summary>
    /// Se ao menos um alvo foi atingido pelo item ou seu efeito.
    /// </summary>
    public bool IsTargetHit { get; private set; }
    /// <summary>
    /// O tempo em que o cozimento foi iniciado.
    /// </summary>
    public float CookStartTime { get; protected set; }
    /// <summary>
    /// Jogador portador deste item.
    /// </summary>
    public Player Player { get; set; }
    /// <summary>
    /// O dono deste item, se for um inimigo.
    /// </summary>
    public IPlayerTarget EnemyOwner { get; set; }
    /// <summary>
    /// O dono deste item, se for um jogador.
    /// </summary>
    public IEnemyTarget PlayerOwner { get; set; }
    /// <summary>
    /// O componente Rigidbody2D deste item.
    /// </summary>
    [HideInInspector]
    public Rigidbody2D Rigidbody;
    /// <summary>
    /// Container para spawnar efeitos como explosões, fogo, etc.
    /// </summary>
    protected Transform EffectsContainer { get; set; }

    #endregion

    [SerializeField]
    public List<CustomAudio> StartSounds, ThrowSounds, HitSounds, DetonateSounds;

    #region Gameobject Components

    /// <summary>
    /// O componente Animator do item.
    /// </summary>
    protected Animator Animator;
    /// <summary>
    /// O componente SpriteRenderer do item.
    /// </summary>
    protected SpriteRenderer Sprite;
    /// <summary>
    /// Emissor de audio deste item.
    /// </summary>
    protected AudioSource AudioSource;
    /// <summary>
    /// Objeto vazio na cena que contém todos os projéteis instanciados.
    /// </summary>
    protected Transform ProjectilesContainer;
    /// <summary>
    /// Referência ao componente Collider2D deste item.
    /// </summary>
    protected Collider2D Collider;
    /// <summary>
    /// As camadas em que o item pode colidir.
    /// </summary>
    protected LayerMask TargetLayerMask;
    protected bool Collided => lastHitTime != 0;
    protected bool CollidedWithEnemy => Collided && PiercedTargetsIds.Any();
    protected float DistanceTraveled => Vector3.Distance(ThrowPosition, transform.position);

    #endregion

    #region Control Variables


    protected float throwTime;
    protected List<int> PiercedTargetsIds = new();
    protected float lastHitTime;
    protected float hitSoundIntervalMs = 100;
    protected Vector3 ThrowPosition;

    #endregion

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        var sprite = transform.Find("Sprite");
        Sprite = sprite.GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        ProjectilesContainer = GameObject.Find("ProjectilesContainer").transform;
        Collider = GetComponent<Collider2D>();
        TargetLayerMask = LayerMask.GetMask("Enemy", "Environment", "PlayerEnvironment");
        EffectsContainer = GameObject.Find("EffectsContainer").transform;
    }

    protected virtual void Start()
    {
        TotalDamage = Damage;
        Player = PlayerWeaponController?.transform.parent.GetComponent<Player>();
        IsCooking = true;
        if (StartFuseOnCook)
            CookStartTime = Time.time;

        WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);

        StartSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
    }

    protected virtual void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        CheckFuse();

        if (RotateToFaceVelocity && !Collided)
            ApplyRotationToVelocity();

        Animation();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Collider.isTrigger)
            HandleCollision(collision.collider);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsActivated)
            return;

        if (Collider.isTrigger)
            HandleCollision(collision);
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
    }

    /// <summary>
    /// Lida com colisões do item.
    /// </summary>
    /// <param name="collision">O collider do objeto com que o item colidiu.</param>
    protected virtual void HandleCollision(Collider2D collision)
    {
        if (!gameObject.activeSelf)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            var target = collision.GetComponentInParent<IPlayerTarget>();
            if (target == null)
                return;

            int targetId = target.transform.GetInstanceID();

            if (!PiercedTargetsIds.Contains(targetId))
                OnEnemyHit(collision);
        }
        else if (collision.gameObject.CompareTag("Environment"))
            OnObjectHit(collision);

        var now = Time.time;
        if (lastHitTime == 0 || lastHitTime + (hitSoundIntervalMs / 1000) < now)
        {
            HitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
            lastHitTime = now;
        }
    }

    /// <summary>
    /// Função chamada quando o item colide com um objeto do tipo "Environment".
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnObjectHit(Collider2D collision)
    {
        if (DetonateOnImpact)
            Detonate();
    }

    /// <summary>
    /// Função chamada quando o item colide com um objeto do tipo "Enemy".
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnEnemyHit(Collider2D collision)
    {
        var target = collision.GetComponentInParent<IPlayerTarget>();
        if (target != null)
        {
            int targetId = target.transform.GetInstanceID();
            PiercedTargetsIds.Add(targetId);
        }

        if (DetonateOnImpact)
            Detonate();
    }

    /// <summary>
    /// Verifica se o fusível do item já queimou e ele deve ser detonado.
    /// </summary>
    protected virtual void CheckFuse()
    {
        if (FuseTimeoutMs <= 0 || HasDetonated || (!StartFuseOnCook && !IsThrown))
            return;

        float elapsedTime = Time.time - (StartFuseOnCook ? CookStartTime : throwTime);
        bool timedOut = elapsedTime >= FuseTimeoutMs / 1000f;

        if (timedOut)
            Detonate();
    }

    /// <summary>
    /// Detona o item, ativando seu efeito.
    /// </summary>
    protected virtual void Detonate()
    {
        if(!IsActivated)
            return;

        HasDetonated = true;

        if (IsCooking)
            PlayerWeaponController.OnThrowEnd();

        DetonateSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
        KillSelf();
    }

    /// <summary>
    /// Função chamada quando o item é lançado.
    /// </summary>
    public virtual void Throw()
    {
        IsThrown = true;
        IsActivated = true;
        throwTime = Time.time;
        ThrowPosition = transform.position;
        IsCooking = false;
        var currentScale = transform.lossyScale;
        transform.parent = ProjectilesContainer;
        transform.localScale = currentScale;

        ThrowSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);

        Rigidbody.AddForce(transform.right * ThrowForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Destroi o item.
    /// </summary>
    protected virtual void KillSelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Marca como alvo atingido e processa a pontuação do jogador.
    /// </summary>
    protected virtual void SetTargetHit()
    {
        if (!IsTargetHit && PlayerOwner != null)
            WavesManager.Instance.CurrentWave.HandlePlayerAttack(0, 1);
        IsTargetHit = true;
    }

    /// <summary>
    /// Destroi o item após um delay.
    /// </summary>
    /// <param name="delaySeconds">Delay em segundos para esperar antes de destruir o item.</param>
    /// <returns></returns>
    protected IEnumerator KillSelfDelayed(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        KillSelf();
    }

    /// <summary>
    /// Processa a animaçãoo do item/flip.
    /// </summary>
    protected virtual void Animation()
    {
        if (!IsThrown)
        {
            bool aimingLeft = PlayerWeaponController?.IsAimingLeft ?? false;
            float absoluteYPosition = Mathf.Abs(transform.localPosition.y);
            if (aimingLeft)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, absoluteYPosition, transform.localPosition.z);
                transform.localScale = new Vector3(1, -1, 1);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, -absoluteYPosition, transform.localPosition.z);
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (Animator != null)
        {
            SyncAnimationStates();
        }
    }

    /// <summary>
    /// Rotaciona o item de acordo com a direção do movimento.
    /// </summary>
    protected virtual void ApplyRotationToVelocity()
    {
        Vector2 velocity = Rigidbody.velocity;

        if (velocity.sqrMagnitude > 0.01f) // Ensure there's enough velocity to calculate direction
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /// <summary>
    /// Sinconiza os estados da animação do Animator com as variáveis de controle.
    /// </summary>
    protected virtual void SyncAnimationStates()
    {
    }
}
