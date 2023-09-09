using System.Collections.Generic;
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
    /// O dano causado quando o item acerta um alvo.
    /// </summary>
    public float HitDamage => Data.HitDamage;
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
    /// A distância em que o efeito do item tem efetividade máxima.
    /// </summary>
    public float EffectMaxRange => Data.EffectMaxRange;
    /// <summary>
    ///A distância em que o efeito do item tem efetividade mínima.
    /// </summary>
    public float EffectMinRange => Data.EffectMinRange;

    #endregion

    #region Properties

    /// <summary>
    /// Se o item está sendo cozinhado atualmente.
    /// </summary>
    public bool IsCooking { get; protected set; }
    /// <summary>
    /// Se o item foi arremessado.
    /// </summary>
    public bool IsThrown { get; set; }
    /// <summary>
    /// A direção em que o jogador está virado, 1 para direita, -1 para esquerda.
    /// </summary>
    public float PlayerFlipDir { get; set; } = 1;
    /// <summary>
    /// Script responsável por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    public PlayerWeaponController PlayerWeaponController { get; set; }
    /// <summary>
    /// Jogador portador deste item.
    /// </summary>
    public Player Player { get; set; }

    #endregion

    [SerializeField]
    protected List<CustomAudio> CookSounds, ThrowSounds, HitSounds, DetonateSounds;

    #region Gameobject Components

    protected Rigidbody2D Rigidbody;
    /// <summary>
    /// O componente Animator do item.
    /// </summary>
    protected Animator Animator;
    /// <summary>
    /// Emissor de audio deste item.
    /// </summary>
    protected AudioSource AudioSource;
    /// <summary>
    /// Referência ao componente SpriteRenderer deste item.
    /// </summary>
    protected SpriteRenderer SpriteRenderer;
    /// <summary>
    /// Objeto vazio na cena que contém todos os projéteis instanciados.
    /// </summary>
    protected Transform ProjectilesContainer;

    #endregion

    #region Control Variables


    /// <summary>
    /// O tempo em que o cozimento foi iniciado.
    /// </summary>
    protected int cookStartTime;

    #endregion

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        var sprite = transform.Find("Sprite");
        SpriteRenderer = sprite.GetComponentInChildren<SpriteRenderer>();
        Animator = GetComponent<Animator>();
        AudioSource = GetComponent<AudioSource>();
        ProjectilesContainer = GameObject.Find("ProjectilesContainer").transform;
    }

    protected virtual void Start()
    {
        Player = PlayerWeaponController.transform.parent.GetComponent<Player>();
    }

    protected virtual void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        Animation();
    }

    public virtual void Throw()
    {
        IsThrown = true;
        var currentScale = transform.lossyScale;
        transform.parent = ProjectilesContainer;
        transform.localScale = currentScale;

        Rigidbody.AddForce(transform.right * ThrowForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Processa a animaçãoo do item/flip.
    /// </summary>
    protected virtual void Animation()
    {
        if (!IsThrown)
        {
            bool aimingLeft = PlayerWeaponController.IsAimingLeft;
            float absoluteYPosition = Mathf.Abs(transform.localPosition.y);
            if (aimingLeft)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, absoluteYPosition, transform.localPosition.z);
                transform.localScale = new Vector3(PlayerFlipDir, -1, 1);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, -absoluteYPosition, transform.localPosition.z);
                transform.localScale = new Vector3(PlayerFlipDir, 1, 1);
            }
        }

        if (Animator != null)
        {
            SyncAnimationStates();
        }
    }

    /// <summary>
    /// Sinconiza os estados da animação do Animator com as variáveis de controle.
    /// </summary>
    protected virtual void SyncAnimationStates()
    {
    }
}
