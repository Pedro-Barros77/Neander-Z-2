using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseEnemy : MonoBehaviour, IPlayerTarget
{
    /// <summary>
    /// A vida máxima do inimigo.
    /// </summary>
    public float MaxHealth { get; protected set; }
    /// <summary>
    /// A vida atual do inimigo.
    /// </summary>
    public float Health { get; protected set; } = 100f;
    /// <summary>
    /// O dano causado pelo inimigo.
    /// </summary>
    public float Damage { get; protected set; }
    /// <summary>
    /// A velocidade de movimento atual do inimigo.
    /// </summary>
    public float MovementSpeed { get; protected set; } = 1f;
    /// <summary>
    /// A velocidade de movimento máxima do inimigo.
    /// </summary>
    public float MaxMovementSpeed { get; protected set; }
    /// <summary>
    /// A velocidade de aceleração do inimigo.
    /// </summary>
    public float AccelerationSpeed { get; protected set; } = 1f;
    /// <summary>
    /// O multiplicador de dano recebido por um tiro na cabeça desse inimigo.
    /// </summary>
    public float HeadshotDamageMultiplier { protected get; set; }
    /// <summary>
    /// A pontuação recebida por matar esse inimigo.
    /// </summary>
    public int KillScore { get; protected set; }
    /// <summary>
    /// O multiplicador de pontos recebidos por um tiro na cabeça desse inimigo.
    /// </summary>
    public float HeadshotScoreMultiplier { get; protected set; }
    /// <summary>
    /// O tipo de inimigo.
    /// </summary>
    public EnemyTypes Type { get; protected set; }
    /// <summary>
    /// Se o inimigo está vivo.
    /// </summary>
    public bool IsAlive { get; protected set; } = true;
    /// <summary>
    /// Infica se esse inimigo está na distância de ataque de um alvo.
    /// </summary>
    protected bool IsInAttackRange { get; set; }
    /// <summary>
    /// Hora da morte desse inimigo.
    /// </summary>
    protected float? DeathTime { get; set; }
    /// <summary>
    /// O tempo que leva para esse inimigo desaparecer após ser morto.
    /// </summary>
    protected float DeathFadeOutDelayMs { get; set; }
    /// <summary>
    /// A opacidade atual do inimigo.
    /// </summary>
    protected float CurrentSpriteAlpha { get; set; } = 1;
    /// <summary>
    /// O volume do som de dano desse inimigo.
    /// </summary>
    protected float DamageSoundVolume { get; set; } = 0.5f;
    /// <summary>
    /// O volume do som de início do ataque desse inimigo.
    /// </summary>
    protected float AttackStartSoundVolume { get; set; } = 1;
    /// <summary>
    /// O volume do som de acerto do ataque desse inimigo.
    /// </summary>
    protected float AttackHitSoundVolume { get; set; } = 1;
    /// <summary>
    /// O volume do som de morte desse inimigo.
    /// </summary>
    protected float DeathSoundVolume { get; set; } = 1;
    /// <summary>
    /// Lista de alvos disponíveis para esse inimigo perseguir e atacar.
    /// </summary>
    protected List<IEnemyTarget> Targets => WavesManager.Instance.EnemiesTargets;
    /// <summary>
    /// Referência ao componente Rigidbody2D desse inimigo.
    /// </summary>
    protected Rigidbody2D RigidBody;
    /// <summary>
    /// Referência ao componente SpriteRenderer desse inimigo.
    /// </summary>
    protected SpriteRenderer SpriteRenderer;
    /// <summary>
    /// Referência ao componente Animator desse inimigo.
    /// </summary>
    protected Animator Animator;
    /// <summary>
    /// Referência ao script AttackTrigger do objeto AttackArea desse inimigo. É ativado por um breve momento e retorna os alvos que foram colididos.
    /// </summary>
    protected AttackTrigger AttackTrigger;
    /// <summary>
    /// Barra de vida desse inimigo.
    /// </summary>
    protected ProgressBar HealthBar;
    /// <summary>
    /// Emissor de audio do inimigo.
    /// </summary>
    protected AudioSource AudioSource;

    protected Canvas WorldPosCanvas;
    [SerializeField]
    protected GameObject HealthBarPrefab, BloodSplatterPrefab;
    [SerializeField]
    protected List<AudioClip> DamageSounds, AttackStartSounds, AttackHitSounds, DeathSounds;
    protected GameObject PopupPrefab;

    protected Transform EffectsContainer;
    protected bool isRunning;
    protected bool isAttacking;
    protected bool isDying;
    protected bool isIdle => !isRunning && !isAttacking && !isDying;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        MaxMovementSpeed = MovementSpeed;
        MaxHealth = Health;
        Animator = GetComponent<Animator>();
        RigidBody = GetComponent<Rigidbody2D>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        AudioSource = GetComponent<AudioSource>();
        WorldPosCanvas = GameObject.Find("WorldPositionCanvas").GetComponent<Canvas>();
        AttackTrigger = transform.Find("AttackArea").GetComponent<AttackTrigger>();
        AttackTrigger.OnTagTriggered += OnTargetHit;
        EffectsContainer = GameObject.Find("EffectsContainer").transform;
        PopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup");

        HealthBar = Instantiate(HealthBarPrefab, WorldPosCanvas.transform).GetComponent<ProgressBar>();
        HealthBar.gameObject.name = $"{Type}-HealthBar";
        HealthBar.SetMaxValue(MaxHealth, MaxHealth);
        HealthBar.UseShadows = false;
        HealthBar.UseOutline = false;
        HealthBar.HideOnFull = true;

        float enemyWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        var healthbarImage = HealthBar.transform.GetChild(0).GetComponent<Image>();
        float healthBarWidth = healthbarImage.preferredWidth * healthbarImage.transform.localScale.x;
        float scaleFactor = (enemyWidth / healthBarWidth) * 0.7f;
        HealthBar.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        (HealthBar.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
    }

    protected virtual void Update()
    {
        if (!IsAlive || isDying)
        {
            Animation();
            return;
        }

        var closestTarget = GetClosestTarget();
        if (IsInAttackRange)
            StartAttack(closestTarget);

        HealthBar.transform.position = transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 1.7f, 0);

        Animation();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsAlive)
            return;

        var closestTarget = GetClosestTarget();
        Movement(closestTarget);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAlive)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            IsInAttackRange = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsAlive)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            IsInAttackRange = false;
        }
    }

    /// <summary>
    /// Função para exibir o popup com devidos parâmetros.
    /// </summary>
    /// <param name="text">Texto a ser exibido</param>
    /// <param name="textColor">A cor que o popup vai ser exibido</param>
    /// <param name="hitPosition">A posição que o popup vai aparecer</param>
    private void ShowPopup(string text, Color32 textColor, Vector3 hitPosition)
    {
        var popup = Instantiate(PopupPrefab, hitPosition, Quaternion.identity, WorldPosCanvas.transform);
        var popupSystem = popup.GetComponent<PopupSystem>();
        if (popupSystem != null)
        {
            popupSystem.Init(text, hitPosition, 2000f, textColor);
        }
    }

    /// <summary>
    /// Diminui a vida e modifica a barra de vida.
    /// </summary>
    /// <param name="value">O valor a ser subtraído da vida.</param>
    /// <param name="bodyPartName">O nome da parte do corpo (GameObject) do inimigo que foi atingida.</param>
    public virtual void TakeDamage(float value, string bodyPartName, Vector3? hitPosition = null)
    {
        if (value < 0 || Health <= 0) return;

        Color32 color;

        switch (bodyPartName)
        {
            case "Head":
                value *= HeadshotDamageMultiplier;
                color = Color.red;
                break;

            default:
                color = Color.yellow;
                break;
        }

        ShowPopup(value.ToString("0"), color, hitPosition ?? transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2));

        if (DamageSounds.Any())
        {
            var randomDamageSound = DamageSounds[Random.Range(0, DamageSounds.Count)];
            AudioSource.PlayOneShot(randomDamageSound, DamageSoundVolume);
        }

        Health = Mathf.Clamp(Health - value, 0, MaxHealth);
        HealthBar.RemoveValue(value);

        if (Health <= 0)
            Die(bodyPartName);
    }

    /// <summary>
    /// Aumenta a vida e modifica a barra de vida.
    /// </summary>
    /// <param name="value">O valor a ser adicionado à vida.</param>
    public virtual void GetHealth(float value)
    {
        if (value < 0 || Health >= MaxHealth) return;

        Health = Mathf.Clamp(Health + value, 0, MaxHealth);
        HealthBar.AddValue(value);
        ShowPopup(value.ToString("0"), Color.green, transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2));
    }

    /// <summary>
    /// Inicia a animação de morte do inimigo.
    /// </summary>
    /// <param name="lastDamagedBodyPartName"></param>
    protected virtual void Die(string lastDamagedBodyPartName)
    {
        IsAlive = false;
        isDying = true;
        DeathTime = Time.time;
        isRunning = false;
        isAttacking = false;

        if (DeathSounds.Any())
        {
            var randomDeathSound = DeathSounds[Random.Range(0, DeathSounds.Count)];
            AudioSource.PlayOneShot(randomDeathSound, DeathSoundVolume);
        }

        if (DeathFadeOutDelayMs > 0)
        {
            StartCoroutine(StartDeathFadeOutCountDown());
        }
    }

    protected float DistanceFrom(Transform target) => Mathf.Abs(target.position.x - transform.position.x);
    protected float DistanceFrom(IEnemyTarget target) => DistanceFrom(target.transform);
    /// <summary>
    /// Retorna o alvo mais próximo desse inimigo, dentre as opções na lista de alvos no WavesManager.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnemyTarget GetClosestTarget() => Targets.FirstOrDefault(a => DistanceFrom(a) == Targets.Min(DistanceFrom));

    /// <summary>
    /// Executa a movimentação desse inimigo.
    /// </summary>
    /// <param name="target">O alvo que determinará a direção de movimento do inimigo.</param>
    protected virtual void Movement(IEnemyTarget target)
    {
        if (target == null)
            return;

        var targetDir = target.transform.position.x < transform.position.x ? -1 : 1;

        if (Mathf.Abs(RigidBody.velocity.x) < MovementSpeed && !IsInAttackRange && !isAttacking)
            RigidBody.velocity += new Vector2(targetDir * AccelerationSpeed, 0);
    }

    /// <summary>
    /// Função chamada ao atingir um alvo com o ataque.
    /// </summary>
    /// <param name="targetCollider">O collider do alvo atacado.</param>
    protected virtual void OnTargetHit(Collider2D targetCollider)
    {
        IEnemyTarget target = targetCollider.GetComponent<IEnemyTarget>();
        if (target == null)
            return;

        if (AttackHitSounds.Any())
        {
            var randomHitSound = AttackHitSounds[Random.Range(0, AttackHitSounds.Count)];
            AudioSource.PlayOneShot(randomHitSound, AttackHitSoundVolume);
        }

        target.TakeDamage(Damage, "");
    }

    public virtual void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection)
    {
        if (BloodSplatterPrefab == null)
            return;

        var bloodSplatter = Instantiate(BloodSplatterPrefab, hitPoint, Quaternion.identity, EffectsContainer);
        bloodSplatter.transform.up = pointToDirection;
    }

    /// <summary>
    /// Inicia a animação de ataque
    /// </summary>
    /// <param name="target">O alvo que está atacando.</param>
    protected virtual void StartAttack(IEnemyTarget target)
    {
        if (target == null)
            return;

        isAttacking = true;

        if (AttackStartSounds.Any())
        {
            var randomAttackStartSound = AttackStartSounds[Random.Range(0, AttackStartSounds.Count)];
            AudioSource.PlayOneShot(randomAttackStartSound, AttackStartSoundVolume);
        }

        var targetDir = target.transform.position.x < transform.position.x ? -1 : 1;
        FlipEnemy(Mathf.Sign(targetDir));
    }

    /// <summary>
    /// Desativa o trigger de ataque em alguns segundos.
    /// </summary>
    /// <param name="duration">A duração em segundos em que o trigger ficará ativado antes de desativar.</param>
    protected virtual IEnumerator DeactivateAttackTrigger(float duration)
    {
        yield return new WaitForSeconds(duration);
        AttackTrigger.gameObject.SetActive(false);
    }

    /// <summary>
    /// Inicia a contagem regressiva para o inimigo desaparecer, depois faz com que fique transparente gradativamente, depois destroi o GameObject.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator StartDeathFadeOutCountDown()
    {
        Destroy(HealthBar.gameObject);

        if (DeathFadeOutDelayMs > 0)
            yield return new WaitForSeconds(DeathFadeOutDelayMs / 1000f);

        while (CurrentSpriteAlpha > 0)
        {
            CurrentSpriteAlpha -= 10f * Time.deltaTime;

            Color spriteColor = SpriteRenderer.color;
            spriteColor.a = CurrentSpriteAlpha;
            SpriteRenderer.color = spriteColor;

            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Função chamada pela animação de ataque desse inimigo no frame em que há contato com o alvo.
    /// </summary>
    protected virtual void OnAttackHit()
    {
        AttackTrigger.gameObject.SetActive(true);
        StartCoroutine(DeactivateAttackTrigger(0.1f));
    }

    /// <summary>
    /// Função chamada no final da animação de ataque desse inimigo.
    /// </summary>
    protected virtual void OnAttackEnd()
    {
        isAttacking = false;
    }

    /// <summary>
    /// Processa a lógica de animação do jogador.
    /// </summary>
    protected virtual void Animation()
    {
        float movementDir = RigidBody.velocity.x;
        bool isMoving = Mathf.Abs(movementDir) > 0.1;

        if (isMoving && !isAttacking)
        {
            FlipEnemy(Mathf.Sign(movementDir));

            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        SyncAnimationStates();
    }

    /// <summary>
    /// Inverte a direção do inimigo.
    /// </summary>
    /// <param name="direction">1 para direita, -1 para esquerda.</param>
    protected void FlipEnemy(float direction)
    {
        if (direction == 0) return;

        if (direction < 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    /// <summary>
    /// Sinconiza os estados da animação do Animator com as variáveis de controle.
    /// </summary>
    private void SyncAnimationStates()
    {
        Animator.SetBool("isIdle", isIdle);
        Animator.SetBool("isRunning", isRunning);

        if (isAttacking) Animator.SetTrigger("Attack");
        else Animator.ResetTrigger("Attack");

        if (isDying) Animator.SetTrigger("Die");
        else Animator.ResetTrigger("Die");
    }
}
