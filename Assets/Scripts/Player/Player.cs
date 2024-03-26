using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IEnemyTarget, IKnockBackable
{
    public PlayerData Data;

    #region Data properties forwarding

    /// <summary>
    /// A vida mбxima do jogador.
    /// </summary>
    public float MaxHealth => Data.MaxHealth;
    /// <summary>
    /// A vida atual do jogador.
    /// </summary>
    public float Health => Data.Health;
    /// <summary>
    /// A velocidade de movimento atual do jogador.
    /// </summary>
    public float MovementSpeed => Data.MovementSpeed;
    /// <summary>
    /// A velocidade de movimento mбxima do jogador.
    /// </summary>
    public float MaxMovementSpeed => Data.MaxMovementSpeed;
    /// <summary>
    /// A velocidade de aceleraзгo do jogador.
    /// </summary>
    public float AccelerationSpeed => Data.AccelerationSpeed;
    /// <summary>
    /// Boost de velocidade do jogador ao correr pressionando o botгo de sprint (correr).
    /// </summary>
    public float SprintSpeedMultiplier => Data.SprintSpeedMultiplier;
    /// <summary>
    /// A forзa do pulo do jogador.
    /// </summary>
    public float JumpForce => Data.JumpForce;
    /// <summary>
    /// A forзa de rolagem da habilidade Rolada Tбtica.
    /// </summary>
    public float RollForce => Data.RollForce;
    /// <summary>
    /// O tempo de recarga da habilidade Rolada Tбtica.
    /// </summary>
    public float RollCooldownMs => Data.RollCooldownMs;
    /// <summary>
    /// A stamina mбxima do jogador.
    /// </summary>
    public float MaxStamina => Data.MaxStamina;
    /// <summary>
    /// A stamina atual do jogador.
    /// </summary>
    public float Stamina => Data.Stamina;
    /// <summary>
    /// Quanto tempo o jogador deve esperar para comeзar a regenerar stamina.
    /// </summary>
    public float StaminaRegenDelayMs => Data.StaminaRegenDelayMs;
    /// <summary>
    /// A taxa de regeneraзгo de stamina do jogador.
    /// </summary>
    public float StaminaRegenRate => Data.StaminaRegenRate;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao correr.
    /// </summary>
    public float SprintStaminaDrain => Data.SprintStaminaDrain;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao pular.
    /// </summary>
    public float JumpStaminaDrain => Data.JumpStaminaDrain;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao atacar com uma arma corpo-a-corpo.
    /// </summary>
    public float AttackStaminaDrain => Data.AttackStaminaDrain;
    /// <summary>
    /// A taxa de drenagem de stamina do jogador ao utilizar a Rolada Tбtica.
    /// </summary>
    public float RollStaminaDrain => Data.RollStaminaDrain;
    /// <summary>
    /// O tipo de personagem do jogador.
    /// </summary>
    public CharacterTypes Character => Data.SkinData.Character;
    /// <summary>
    /// A pontuaзгo total do jogador.
    /// </summary>
    public float Score => Data.Score;
    /// <summary>
    /// O dinheiro total do jogador.
    /// </summary>
    public float Money => Data.Money;

    #endregion

    /// <summary>
    /// A ъltima vez que o jogador gastou stamina.
    /// </summary>
    /// 
    public float LastStaminaUse { get; private set; }
    public bool IsAlive { get; private set; }
    public bool IsDying { get; private set; }
    public float DeathTime { get; private set; }
    public float DeathTimeDelayMs { get; private set; } = 5000f;
    public bool IsSecondChanceActive { get; private set; }
    public Bounds Bounds { get; private set; }


    /// <summary>
    /// A mochila do jogador, carrega suas armas e acessуrios.
    /// </summary>
    public Backpack Backpack { get; private set; }
    /// <summary>
    /// A arma atualmente equipada nas mгos do jogador.
    /// </summary>
    public BaseWeapon CurrentWeapon => Backpack.EquippedWeapon;
    private InGameScreen Screen;
    private Canvas WorldPosCanvas;
    private GameObject PopupPrefab;
    private AudioSource AudioSource;
    /// <summary>
    /// Script responsбvel por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    [SerializeField]
    public PlayerWeaponController WeaponController;
    [SerializeField]
    ProgressBar HealthBar, StaminaBar;
    [SerializeField]
    GameObject SecondChanceEffect, CautiousEffect;
    [SerializeField]
    CustomAudio SecondChanceActivationSound;
    [SerializeField]
    List<CustomAudio> CautiousHitSounds;
    [SerializeField]
    GameObject AmmoDropPrefab;

    SpriteRenderer HeadSpriteRenderer, BodySpriteRenderer, LegsSpriteRenderer;

    public PlayerMovement PlayerMovement { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

    void Start()
    {
        IsAlive = true;
        Screen = GameObject.Find("Screen").GetComponent<InGameScreen>();
        Backpack = new Backpack(this, Data.InventoryData);
        RigidBody = GetComponent<Rigidbody2D>();
        AudioSource = GetComponent<AudioSource>();
        PlayerMovement = GetComponentInChildren<PlayerMovement>();
        Data.MovementSpeed = Data.MaxMovementSpeed;
        Data.Stamina = Data.MaxStamina;

        WorldPosCanvas = GameObject.Find("WorldPositionCanvas").GetComponent<Canvas>();
        PopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup");

        if (HealthBar != null)
        {
            HealthBar.SetMaxValue(MaxHealth, Health);
            HealthBar.AnimationSpeed = 20f;
        }

        StaminaBar.gameObject.SetActive(true);
        StaminaBar.AnimationSpeed = 20f;
        StaminaBar.ValueFillColor = new Color32(245, 238, 20, 255);
        StaminaBar.UseShadows = false;
        StaminaBar.UseOutline = false;
        StaminaBar.UseAnimation = false;
        StaminaBar.HideOnFull = true;
        StaminaBar.SetMaxValue(MaxStamina, MaxStamina);
        (StaminaBar.transform as RectTransform).pivot = new Vector2(0.5f, 0.5f);

        var skin = transform.Find("Skin");
        var head = skin.Find("Head");
        var body = skin.Find("Torso");
        var legs = skin.Find("Legs");

        HeadSpriteRenderer = head.GetComponent<SpriteRenderer>();
        BodySpriteRenderer = body.GetComponent<SpriteRenderer>();
        LegsSpriteRenderer = legs.GetComponent<SpriteRenderer>();

        Bounds = new Bounds(HeadSpriteRenderer.bounds.center, HeadSpriteRenderer.bounds.size);
        Bounds.Encapsulate(BodySpriteRenderer.bounds);
        Bounds.Encapsulate(LegsSpriteRenderer.bounds);
    }

    void Update()
    {
        StaminaBar.transform.position = transform.position + new Vector3(0, Bounds.size.y * 1.2f, 0);
        if (IsSecondChanceActive)
            SecondChanceEffect.transform.position = transform.position + new Vector3(0, Bounds.size.y * 0.8f, 0);
        if (Backpack.EquippedPassiveSkillType == PassiveSkillTypes.Cautious)
            CautiousEffect.transform.position = transform.position + new Vector3(0, Bounds.size.y * 1.5f, 0);

        Debug.DrawLine((transform.position - (Bounds.size)), (transform.position + (Bounds.size)), Color.red);

        UpdatePassiveSkills();

        if (Constants.EnableDevKeybinds)
        {
            if (Constants.GetActionDown(InputActions.DEBUG_IncreaseHealth))
            {
                GetHealth(20);
                GetStamina(20);
            }
            if (Constants.GetActionDown(InputActions.DEBUG_DecreaseHealth))
            {
                TakeDamage(20, 1, "", null);
            }
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

        Data.Health = Mathf.Clamp(Health + value, 0, MaxHealth);
        if (HealthBar != null)
            HealthBar.AddValue(value);
        ShowPopup(value.ToString("N1"), Color.green, transform.position + new Vector3(0, Bounds.size.y));
    }

    /// <summary>
    /// Diminui a vida e modifica a barra de vida.
    /// </summary>
    /// <param name="value">O valor a ser subtraнdo da vida.</param>
    public void TakeDamage(float value, float headshotMultiplier, string bodyPartName, IPlayerTarget attacker, Vector3? hitPosition = null, bool selfDamage = false)
    {
        if (!IsAlive)
            return;

        if (value < 0) return;

        if (selfDamage && Backpack.EquippedPassiveSkillType == PassiveSkillTypes.Cautious)
        {
            value *= Constants.CautiousDamageMultiplier;
            CautiousHitSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
            CautiousEffect.SetActive(true);
            CautiousEffect.GetComponentInChildren<Animator>().SetTrigger("CautiousEffect");
        }

        if (Health / MaxHealth >= Constants.SecondChanceHealthTrheshold
            && (Health - value) / MaxHealth < Constants.SecondChanceHealthTrheshold
            && Backpack.EquippedPassiveSkillType == PassiveSkillTypes.SecondChance
            && !IsSecondChanceActive)
        {
            IsSecondChanceActive = true;
            GetStamina(MaxStamina);
            SecondChanceEffect.SetActive(true);
            SecondChanceEffect.GetComponentInChildren<Animator>().SetTrigger("SecondChanceEffect");
            SecondChanceActivationSound.PlayIfNotNull(AudioSource, AudioTypes.Player);
            StartCoroutine(DeactivateSecondChance());
        }

        if (IsSecondChanceActive && Backpack.EquippedPassiveSkillType == PassiveSkillTypes.SecondChance)
            value *= Constants.SecondChanceDamageTakenMultiplier;

        Data.Health = Mathf.Clamp(Health - value, 0, MaxHealth);
        if (HealthBar != null)
            HealthBar.RemoveValue(value);
        ShowPopup(value.ToString("N1"), Color.yellow, hitPosition ?? transform.position + new Vector3(0, Bounds.size.y));

        if (WavesManager.Instance != null && WavesManager.Instance.CurrentWave != null && WavesManager.Instance.CurrentWave.Stats != null)
            WavesManager.Instance.CurrentWave.Stats.DamageTaken += value;

        if (Health <= 0 && IsAlive)
            Die();
    }

    private IEnumerator DeactivateSecondChance()
    {
        yield return new WaitForSeconds(Constants.SecondChanceDurationMs / 1000);
        IsSecondChanceActive = false;
        SecondChanceEffect.SetActive(false);
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
            popupSystem.Init(text, hitPosition, 1300f, textColor);
        }
    }

    /// <summary>
    /// Adiciona dinheiro ao jogador.
    /// </summary>
    /// <param name="value">O valor a ser adicionado.</param>
    public void GetMoney(float value) => Data.GetMoney(value);

    /// <summary>
    /// Retira dinheiro do jogador.
    /// </summary>
    /// <param name="value">O valor a ser retirado.</param>
    public void TakeMoney(float value) => Data.TakeMoney(value);

    /// <summary>
    /// Função que é chamada quando o jogador morre.
    /// </summary>
    protected virtual void Die()
    {
        SavesManager.UpdateWaveStats(WavesManager.Instance.CurrentWave.Data.Number, died: true);
        WavesManager.Instance.EnemiesTargets.Remove(this);
        IsAlive = false;
        IsDying = true;
        StaminaBar.gameObject.SetActive(false);
        Destroy(Backpack.EquippedPrimaryWeapon?.gameObject);
        Destroy(Backpack.EquippedSecondaryWeapon?.gameObject);
    }

    /// <summary>
    /// Aumenta a stamina e modifica a barra de stamina.
    /// </summary>
    /// <param name="value">O valor a ser adicionado а stamina.</param>
    public void GetStamina(float value)
    {
        if (value < 0) return;

        Data.Stamina = Mathf.Clamp(Stamina + value, 0, MaxStamina);
        StaminaBar.AddValue(value);
    }

    /// <summary>
    /// Diminui a stamina e modifica a barra de stamina.
    /// </summary>
    /// <param name="value">O valor a ser diminuido а stamina.</param>
    public void LoseStamina(float value)
    {
        if (value < 0) return;

        if (IsSecondChanceActive && Backpack.EquippedPassiveSkillType == PassiveSkillTypes.SecondChance)
            value *= Constants.SecondChanceStaminaDrainMultiplier;

        LastStaminaUse = Time.time;
        Data.Stamina = Mathf.Clamp(Stamina - value, 0, MaxStamina);
        StaminaBar.RemoveValue(value);
    }

    #region Animation Forwarding
    /// <summary>
    /// Função chamada pelo evento de animação, no último frame da Rolada Tática.
    /// </summary>
    public void OnRollEnd() => PlayerMovement.OnRollEnd();

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame do giro do personagem.
    /// </summary>
    public void OnTurnEnd() => PlayerMovement.OnTurnEnd();

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame do giro do personagem.
    /// </summary>
    public void OnTurnBackEnd() => PlayerMovement.OnTurnBackEnd();

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame ao cair no chão do personagem.
    /// </summary>
    public void OnFallGroundEnd() => PlayerMovement.OnFallGroundEnd();

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame ao arremessar um item.
    /// </summary>
    public void OnThrowEnd() => WeaponController.OnThrowEnd();

    #endregion
    public void TakeKnockBack(float pushForce, Vector3 direction)
    {
        RigidBody.AddForce(direction * pushForce);
    }

    public void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection, string bodyPartName)
    {
        //if (BloodSplatterPrefab == null)
        //    return;

        //if (lastBloodSplatterTime + bloodSplatterDelay > Time.time)
        //    return;

        //var bloodSplatter = Instantiate(BloodSplatterPrefab, hitPoint, Quaternion.identity, EffectsContainer);
        //bloodSplatter.transform.up = pointToDirection;
        //var bloodParticles = bloodSplatter.GetComponent<ParticleSystem>();
        //var mainBloodSystem = bloodParticles.main;
        //mainBloodSystem.startColor = new(BloodColor);
        //lastBloodSplatterTime = Time.time;
    }

    public void HandleEnemyKill(BaseEnemy enemy, string lastDamagedBodyPartName)
    {
        if (enemy == null) return;

        bool isHeadshot = lastDamagedBodyPartName == "Head";
        if (isHeadshot && Backpack.EquippedPassiveSkillType == PassiveSkillTypes.PrecisionBounty)
            CheckAmmoDrop(enemy.transform.position);
    }

    public void HandleSpriteColorChange(Color32 color)
    {
        var playerSprites = transform.Find("Skin").GetComponentsInChildren<SpriteRenderer>();

        foreach (var spriteRenderer in playerSprites)
            spriteRenderer.material.SetColor("_Color", color);
    }

    /// <summary>
    /// Sorteia se o inimigo vai dropar munição ou não.
    /// </summary>
    /// <param name="spawnPosition">A posição para spawnar a munição.</param>
    private void CheckAmmoDrop(Vector3 spawnPosition)
    {
        BulletTypes? primaryType = Backpack.EquippedPrimaryWeapon?.BulletType;
        if (primaryType == BulletTypes.Melee)
            primaryType = null;
        BulletTypes? secondaryType = Backpack.EquippedSecondaryWeapon?.BulletType;
        if (secondaryType == BulletTypes.Melee)
            secondaryType = null;

        BulletTypes ammoDropType;
        if (primaryType == null && secondaryType == null)
            return;

        if (primaryType == null)
            ammoDropType = secondaryType.Value;
        else if (secondaryType == null)
            ammoDropType = primaryType.Value;
        else
            ammoDropType = Random.Range(0, 2) == 0 ? primaryType.Value : secondaryType.Value;

        int dropCount = 0;
        float dropChance = 0;

        switch (ammoDropType)
        {
            case BulletTypes.Pistol:
                dropCount = Constants.PrecisionBountyPistolCount;
                dropChance = Constants.PrecisionBountyPistolChance;
                break;
            case BulletTypes.Shotgun:
                dropCount = Constants.PrecisionBountyShotgunCount;
                dropChance = Constants.PrecisionBountyShotgunChance;
                break;
            case BulletTypes.AssaultRifle:
                dropCount = Constants.PrecisionBountyAssaultRifleCount;
                dropChance = Constants.PrecisionBountyAssaultRifleChance;
                break;
            case BulletTypes.Sniper:
                dropCount = Constants.PrecisionBountySniperCount;
                dropChance = Constants.PrecisionBountySniperChance;
                break;
            case BulletTypes.Rocket:
                dropCount = Constants.PrecisionBountyRocketCount;
                dropChance = Constants.PrecisionBountyRocketChance;
                break;
        }

        if (Random.Range(0f, 1f) > dropChance)
            return;

        var ammoDropObj = Instantiate(AmmoDropPrefab, spawnPosition, Quaternion.identity, WorldPosCanvas.transform);
        var ammoDrop = ammoDropObj.GetComponent<AmmoDrop>();
        ammoDrop.SetDrop(
            ammoDropType,
            dropCount,
            (BulletTypes type, int count) => Data.InventoryData.SetAmmo(type, Data.InventoryData.GetAmmo(type) + count));
    }

    /// <summary>
    /// Atualiza as habilidades passivas equipadas.
    /// </summary>
    private void UpdatePassiveSkills()
    {
        if (Backpack.EquippedPassiveSkillType == PassiveSkillTypes.CrouchRecovery)
        {
            var oldFx = transform.Find("HealingEffect");
            if (PlayerMovement.isCrouching && Health < MaxHealth)
            {
                if (oldFx == null)
                {
                    var healingEffectObj = new GameObject("HealingEffect");
                    var healingEffect = healingEffectObj.AddComponent<HealingEffect>();
                    healingEffect.TickHealAmount = Constants.CrouchRecoveryHealthTick;
                    healingEffect.StartDelayMs = 1000f;
                    healingEffect.SetEffect(99000, Constants.CrouchRecoveryTickIntervalMs);
                    healingEffectObj.transform.SetParent(transform);
                }
            }
            else
            {
                if (oldFx != null)
                    Destroy(oldFx.gameObject);
            }

        }
    }
}
