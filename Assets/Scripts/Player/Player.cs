using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Device;

public class Player : MonoBehaviour, IEnemyTarget
{
    /// <summary>
    /// A vida mбxima do jogador.
    /// </summary>
    public float MaxHealth { get; private set; }
    /// <summary>
    /// A vida atual do jogador.
    /// </summary>
    public float Health { get; private set; } = 100f;
    /// <summary>
    /// A velocidade de movimento atual do jogador.
    /// </summary>
    public float MovementSpeed { get; private set; } = 4f;
    /// <summary>
    /// A velocidade de movimento mбxima do jogador.
    /// </summary>
    public float MaxMovementSpeed { get; private set; }
    /// <summary>
    /// A velocidade de aceleraзгo do jogador.
    /// </summary>
    public float AccelerationSpeed { get; private set; } = 1f;
    /// <summary>
    /// Boost de velocidade do jogador ao correr pressionando o botгo de sprint (correr).
    /// </summary>
    public float SprintSpeedMultiplier { get; private set; } = 1.5f;
    /// <summary>
    /// A forзa do pulo do jogador.
    /// </summary>
    public float JumpForce { get; private set; } = 1800f;
    /// <summary>
    /// A forзa de rolagem da habilidade Rolada Tбtica.
    /// </summary>
    public float RollForce { get; private set; } = 1800f;
    /// <summary>
    /// O tempo de recarga da habilidade Rolada Tбtica.
    /// </summary>
    public float RollCooldownMs { get; set; } = 2000f;
    #region Stamina Stats
    /// <summary>
    /// A stamina mбxima do jogador.
    /// </summary>
    public float MaxStamina { get; private set; }
    /// <summary>
    /// A stamina atual do jogador.
    /// </summary>
    public float Stamina { get; private set; } = 100f;
    /// <summary>
    /// Quanto tempo o jogador deve esperar para comeзar a regenerar stamina.
    /// </summary>
    public float StaminaRegenDelayMs { get; private set; } = 2000f;
    /// <summary>
    /// A taxa de regeneraзгo de stamina do jogador.
    /// </summary>
    public float StaminaRegenRate { get; private set; } = 15f;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao correr.
    /// </summary>
    public float SprintStaminaDrain { get; private set; } = 2f;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao pular.
    /// </summary>
    public float JumpStaminaDrain { get; private set; } = 10f;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao atacar com uma arma corpo-a-corpo.
    /// </summary>
    public float AttackStaminaDrain { get; private set; }
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao utilizar a Rolada Tбtica.
    /// </summary>
    public float RollStaminaDrain { get; private set; } = 20f;
    /// <summary>
    /// A ъltima vez que o jogador gastou stamina.
    /// </summary>
    public float LastStaminaUse { get; private set; }
    public bool IsAlive { get; private set; }
    public bool isDying { get; private set; }
    public float DeathTime { get; private set; }
    public float DeathTimeDelayMs { get; private set; } = 5000f;
    #endregion

    /// <summary>
    /// O tipo de personagem do jogador.
    /// </summary>
    public CharacterTypes Character { get; private set; }
    /// <summary>
    /// A pontuaзгo total do jogador.
    /// </summary>
    public float Score { get; private set; }
    /// <summary>
    /// O dinheiro total do jogador.
    /// </summary>
    public float Money { get; private set; }

    /// <summary>
    /// A mochila do jogador, carrega suas armas e acessуrios.
    /// </summary>
    public Backpack Backpack { get; private set; }
    /// <summary>
    /// A arma atualmente equipada nas mгos do jogador.
    /// </summary>
    public BaseWeapon CurrentWeapon => Backpack.EquippedWeapon;
    private SpriteRenderer SpriteRenderer;
    private InGameScreen Screen;
    private Canvas WorldPosCanvas;
    private GameObject PopupPrefab;

    /// <summary>
    /// Script responsбvel por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    [SerializeField]
    public PlayerWeaponController WeaponController;
    [SerializeField]
    ProgressBar HealthBar, StaminaBar;

    void Start()
    {
        IsAlive = true;
        Screen = GameObject.Find("Screen").GetComponent<InGameScreen>();
        Backpack = new Backpack(this);
        Backpack.AddWeapon(WeaponTypes.ShortBarrel);
        WeaponController.SwitchWeapon(0);
        MaxMovementSpeed = MovementSpeed;
        MaxHealth = Health;
        MaxStamina = Stamina;
        WorldPosCanvas = GameObject.Find("WorldPositionCanvas").GetComponent<Canvas>();
        PopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup");
        SpriteRenderer = GetComponent<SpriteRenderer>();

        //float weaponContainerHeight = CurrentWeapon.WeaponContainerOffset.y;
        //var clipCrouch = animator.runtimeAnimatorController.animationClips.FirstOrDefault(x => x.name == "Carlos_Crouch");
        //var key = new Keyframe(0, weaponContainerHeight);
        //clipCrouch.SetCurve("", typeof(Transform), "WeaponContainer.localPosition.y", new AnimationCurve(key));

        HealthBar.SetMaxValue(MaxHealth, true);
        HealthBar.AnimationSpeed = 20f;
        StaminaBar.gameObject.SetActive(true);
        StaminaBar.AnimationSpeed = 20f;
        StaminaBar.ValueFillColor = new Color32(245, 238, 20, 255);
        StaminaBar.UseShadows = false;
        StaminaBar.UseOutline = false;
        StaminaBar.UseAnimation = false;
        StaminaBar.HideOnFull = true;
        StaminaBar.SetMaxValue(MaxStamina, true);
        (StaminaBar.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);
        WavesManager.Instance.EnemiesTargets.Add(this);
    }

    void Update()
    {
        StaminaBar.transform.position = transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 1.7f, 0);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetHealth(20);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TakeDamage(20, "");
        }
    }

    private void FixedUpdate()
    {
        if (!IsAlive)
            return;

        var staminaCooledDown = LastStaminaUse + (StaminaRegenDelayMs / 1000) <= Time.time;

        if (staminaCooledDown)
            GetStamina(StaminaRegenRate * Time.deltaTime);
    }

    /// <summary>
    /// Função para mostrar o painel de game over, quando o player morre.
    /// </summary>
    public void ShowGameOverPanel()
    {
        Screen.ShowGameOverPanel();
    }

    /// <summary>
    /// Aumenta a vida e modifica a barra de vida.
    /// </summary>
    /// <param name="value">O valor a ser adicionado а vida.</param>
    public void GetHealth(float value)
    {
        if (value < 0) return;

        Health = Mathf.Clamp(Health + value, 0, MaxHealth);
        HealthBar.AddValue(value);
        ShowPopup(value.ToString("0"), Color.green, transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2));
    }

    /// <summary>
    /// Diminui a vida e modifica a barra de vida.
    /// </summary>
    /// <param name="value">O valor a ser subtraнdo da vida.</param>
    public void TakeDamage(float value, string bodyPartName, Vector3? hitPosition = null)
    {
        if (value < 0) return;

        Health = Mathf.Clamp(Health - value, 0, MaxHealth);
        HealthBar.RemoveValue(value);
        ShowPopup(value.ToString("0"), Color.yellow, hitPosition ?? transform.position + new Vector3(0, SpriteRenderer.bounds.size.y / 2));

        if (Health <= 0 && IsAlive)
            Die();
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
    /// Adiciona dinheiro ao jogador.
    /// </summary>
    /// <param name="value">O valor a ser adicionado.</param>
    public void GetMoney(float value)
    {
        if (value < 0) return;

        Money += value;
    }

    /// <summary>
    /// Retira dinheiro do jogador.
    /// </summary>
    /// <param name="value">O valor a ser retirado.</param>
    public void TakeMoney(float value)
    {
        if (value < 0) return;

        Money -= value;
    }

    /// <summary>
    /// Função que é chamada quando o jogador morre.
    /// </summary>
    protected virtual void Die()
    {
        IsAlive = false;
        isDying = true;
        StaminaBar.gameObject.SetActive(false);
        Destroy(Backpack.EquippedPrimaryWeapon.gameObject);
        Destroy(Backpack.EquippedSecondaryWeapon.gameObject);
    }

    /// <summary>
    /// Aumenta a stamina e modifica a barra de stamina.
    /// </summary>
    /// <param name="value">O valor a ser adicionado а stamina.</param>
    public void GetStamina(float value)
    {
        if (value < 0) return;

        Stamina = Mathf.Clamp(Stamina + value, 0, MaxStamina);
        StaminaBar.AddValue(value);
    }

    /// <summary>
    /// Diminui a stamina e modifica a barra de stamina.
    /// </summary>
    /// <param name="value">O valor a ser diminuido а stamina.</param>
    public void LoseStamina(float value)
    {
        if (value < 0) return;

        LastStaminaUse = Time.time;
        Stamina = Mathf.Clamp(Stamina - value, 0, MaxStamina);
        StaminaBar.RemoveValue(value);
    }
}
