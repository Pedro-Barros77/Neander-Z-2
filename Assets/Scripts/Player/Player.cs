using UnityEngine;

public class Player : MonoBehaviour, IEnemyTarget
{
    /// <summary>
    /// A vida máxima do jogador.
    /// </summary>
    public float MaxHealth { get; private set; }
    /// <summary>
    /// A vida atual do jogador.
    /// </summary>
    public float Health { get; private set; } = 100f;
    
    /// <summary>
    /// A velocidade de movimento atual do jogador.
    /// </summary>
    public float MovementSpeed { get; private set; } = 5f;
    /// <summary>
    /// A velocidade de movimento máxima do jogador.
    /// </summary>
    public float MaxMovementSpeed { get; private set; }
    /// <summary>
    /// A velocidade de aceleração do jogador.
    /// </summary>
    public float AccelerationSpeed { get; private set; } = 1f;
    /// <summary>
    /// A força do pulo do jogador.
    /// </summary>
    public float JumpForce { get; private set; } = 1800f;
    /// <summary>
    /// A força de rolagem da habilidade Rolada Tática.
    /// </summary>
    public float RollForce { get; private set; } = 1800f;
    /// <summary>
    /// O tempo de recarga da habilidade Rolada Tática.
    /// </summary>
    public float RollCooldownMs { get; set; } = 2000f;
    #region Stamina Stats
    /// <summary>
    /// A stamina máxima do jogador.
    /// </summary>
    public float MaxStamina { get; private set; }
    /// <summary>
    /// A stamina atual do jogador.
    /// </summary>
    public float Stamina { get; set; }
    /// <summary>
    /// Quanto tempo o jogador deve esperar para começar a regenerar stamina.
    /// </summary>
    public float StaminaRegenDelayMs { get; set; }
    /// <summary>
    /// A taxa de regeneração de stamina do jogador.
    /// </summary>
    public float StaminaRegenRate { get; set; }
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao correr.
    /// </summary>
    public float SprintStaminaDrain { get; set; } 
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao pular.
    /// </summary>
    public float JumpStaminaDrain { get; set; } 
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao atacar com uma arma corpo-a-corpo.
    /// </summary>
    public float AttackStaminaDrain { get; set; } 
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao utilizar a Rolada Tática.
    /// </summary>
    public float RollStaminaDrain { get; set; } 
    /// <summary>
    /// A última vez que o jogador gastou stamina.
    /// </summary>
    public Time LastStaminaUse { get; set; }
    #endregion

    /// <summary>
    /// O tipo de personagem do jogador.
    /// </summary>
    public CharacterTypes Character { get; private set; }
    /// <summary>
    /// A pontuação total do jogador.
    /// </summary>
    public float Score { get; private set; }
    /// <summary>
    /// O dinheiro total do jogador.
    /// </summary>
    public decimal Money { get; private set; }

    /// <summary>
    /// A mochila do jogador, carrega suas armas e acessórios.
    /// </summary>
    public Backpack Backpack { get; private set; }
    /// <summary>
    /// A arma atualmente equipada nas mãos do jogador.
    /// </summary>
    public BaseWeapon CurrentWeapon => Backpack.EquippedWeapon;

    /// <summary>
    /// Script responsável por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    [SerializeField]
    public PlayerWeaponController WeaponController;
    [SerializeField]
    ProgressBar HealthBar;

    // Start is called before the first frame update
    void Start()
    {
        Backpack = new Backpack(this);
        MaxMovementSpeed = MovementSpeed;
        MaxHealth = Health;
        Backpack.AddWeapon(WeaponTypes.Colt_1911);

        HealthBar.SetMaxValue(MaxHealth, true);
        HealthBar.UseAnimation = true;
        HealthBar.AnimationSpeed = 20f;
        WavesManager.Instance.EnemiesTargets.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetHealth(20);
        }
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            TakeDamage(20);
        }
    }

    public void GetHealth(float value)
    {
        if (value < 0) return;

        Health = Mathf.Clamp(Health + value, 0, MaxHealth);
        HealthBar.AddValue(value);
    }

    public void TakeDamage(float value)
    {
        if (value < 0) return;

        Health = Mathf.Clamp(Health - value, 0, MaxHealth);
        HealthBar.RemoveValue(value);
    }
}
