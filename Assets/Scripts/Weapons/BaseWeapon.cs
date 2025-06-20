using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField]
    public BaseWeaponData Data;

    #region Data Properties Forwarding

    /// <summary>
    /// O dano causado pela arma ou seus proj�teis.
    /// </summary>
    public float Damage => Data.Damage;
    /// <summary>
    /// O dano mínimo causado pela arma ou seus projéteis, uma vez que o alcance de MinDamageRange é atingido.
    /// </summary>
    public float MinDamage => Data.MinDamage;
    /// <summary>
    /// O multiplicador de dano causado por um tiro na cabeça.
    /// </summary>
    public float HeadshotMultiplier => Data.HeadshotMultiplier;
    /// <summary>
    /// A taxa de disparo da arma (disparos por segundo).
    /// </summary>
    public float FireRate => Data.FireRate;
    /// <summary>
    /// Capacidade m�xima do carregador.
    /// </summary>
    public int MagazineSize => Data.MagazineSize;
    /// <summary>
    /// Quantidade de muni��es restantes no carregador.
    /// </summary>
    public int MagazineBullets => Data.MagazineBullets;
    /// <summary>
    /// Velocidade de movimento dos proj�teis.
    /// </summary>
    public float BulletSpeed => Data.BulletSpeed;
    /// <summary>
    /// Se o proj'etil voa infinitamente para frente.
    /// </summary>
    public bool FlyInfinitely => Data.FlyInfinitely;
    /// <summary>
    /// O multiplicador de gravidade aplicada ao projétil.
    /// </summary>
    public float GravityScale => Data.GravityScale;
    /// <summary>
    /// A dist�ncia m�xima que o proj�til pode percorrer antes de ser destru�do.
    /// </summary>
    public float BulletMaxRange => Data.BulletMaxRange;
    /// <summary>
    /// O alcance em que o proj�til causa dano m�ximo.
    /// </summary>
    public float MaxDamageRange => Data.MaxDamageRange;
    /// <summary>
    /// A dist�ncia em que o proj�til come�a a causar dano m�nimo.
    /// </summary>
    public float MinDamageRange => Data.MinDamageRange;
    /// <summary>
    /// Se a arma � prim�ria ou secund�ria.
    /// </summary>
    public bool IsPrimary => Data.IsPrimary;
    /// <summary>
    /// O tempo de recarga da arma, em milissegundos.
    /// </summary>
    public float ReloadTimeMs => Data.ReloadTimeMs;
    /// <summary>
    /// O tempo de troca da arma, em milissegundos.
    /// </summary>
    public float SwitchTimeMs => Data.SwitchTimeMs;
    /// <summary>
    /// O tipo de proj�til que a arma dispara.
    /// </summary>
    public BulletTypes BulletType => Data.BulletType;
    /// <summary>
    /// O tipo de arma.
    /// </summary>
    public WeaponTypes Type => Data.Type;
    /// <summary>
    /// Modo de disparo da arma.
    /// </summary>
    public FireModes FireMode => Data.FireMode;
    /// <summary>
    /// Tipo de recarregamento da arma.
    /// </summary>
    public ReloadTypes ReloadType => Data.ReloadType;

    #endregion

    #region Properties

    /// <summary>
    /// Se esta arma é a arma ativa, usada pelo jogador atualmente.
    /// </summary>
    public bool IsActive { get; set; }
    /// <summary>
    /// Se a arma est� sendo recarregada atualmente.
    /// </summary>
    public bool IsReloading { get; protected set; }
    /// <summary>
    /// Tempo em que a arma come�ou a ser recarregada.
    /// </summary>
    public float? ReloadStartTime { get; protected set; }
    /// <summary>
    /// Se a arma está sendo trocada atualmente.
    /// </summary>
    public bool IsSwitchingWeapon { get; set; }
    /// <summary>
    /// A direção em que o jogador está virado, 1 para direita, -1 para esquerda.
    /// </summary>
    public float PlayerFlipDir { get; set; } = 1;
    /// <summary>
    /// Script responsável por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    public PlayerWeaponController PlayerWeaponController { get; set; }
    /// <summary>
    /// Jogador portador desta arma.
    /// </summary>
    public Player Player { get; set; }
    /// <summary>
    /// Define o offset da posição do container da arma em relação ao jogador.
    /// </summary>
    protected Vector3 WeaponContainerOffset { get; set; }

    #endregion

    [SerializeField]
    protected GameObject BulletPrefab;
    [SerializeField]
    protected List<CustomAudio> ShootSounds, ExtraSoundEffects;
    [SerializeField]
    protected CustomAudio EmptyChamberSound;
    [SerializeField]
    protected GameObject SmokeParticlesPrefab;

    #region Gameobject Components

    /// <summary>
    /// O componente Animator da arma.
    /// </summary>
    protected Animator Animator;
    /// <summary>
    /// A posi��o em que os proj�teis devem ser instanciados, � frente do cano da arma.
    /// </summary>
    protected Transform BulletSpawnPoint;
    /// <summary>
    /// Objeto vazio na cena que cont�m todos os proj�teis instanciados.
    /// </summary>
    protected Transform BulletsContainer;
    /// <summary>
    /// Emissor de audio da arma
    /// </summary>
    protected AudioSource AudioSource;
    /// <summary>
    /// Referência ao componente SpriteRenderer da arma.
    /// </summary>
    protected SpriteRenderer SpriteRenderer;
    /// <summary>
    /// Referência ao componente SpriteRenderer da mão dessa arma.
    /// </summary>
    protected SpriteRenderer HandSpriteRenderer;

    /// <summary>
    /// Referência ao componente ShadowCaster2D da arma.
    /// </summary>
    protected ShadowCaster2D ShadowCaster;
    /// <summary>
    /// Flash de luz do disparo da arma.
    /// </summary>
    protected Light2D FlashLight, InnerFlashLight;

    /// <summary>
    /// O objeto que representa o pente/carregador da arma.
    /// </summary>
    protected GameObject MagDrop;
    /// <summary>
    /// O objeto que representa o cartucho/cápsula da bala.
    /// </summary>
    protected GameObject CartridgeDrop;

    #endregion

    #region Control Variables

    /// <summary>
    /// A intensidade inicial do flash de luz.
    /// </summary>
    protected float FlashLightStartIntensity;
    /// <summary>
    /// Se o flash de luz está diminuindo a intensidade atualmente.
    /// </summary>
    protected bool isDecreasingFlashIntensity;
    /// <summary>
    /// �ltima vez em que a arma foi disparada.
    /// </summary>
    protected float? lastShotTime;
    /// <summary>
    /// Diferen�a entre a quantidade de muni��es necess�rias para completar o carregador e a quantidade de muni��es dispon�veis na mochila.
    /// </summary>
    protected int reloadBackpackMagDiff;
    /// <summary>
    /// A propor��o entre a taxa de disparo e o tempo em milissegundos.
    /// </summary>
    protected const float FIRE_RATE_RATIO = 1000;

    protected bool isShooting;
    protected bool isHandColorSet;

    #endregion

    protected virtual void Awake()
    {
        var sprite = transform.Find("Sprite");
        SpriteRenderer = sprite.GetComponent<SpriteRenderer>();
        HandSpriteRenderer = sprite.transform.Find("Hand").GetComponent<SpriteRenderer>();
        ShadowCaster = sprite.GetComponent<ShadowCaster2D>();
        Animator = sprite.GetComponent<Animator>();

        var flashLight = sprite.Find("FlashLight");
        if (flashLight != null)
            FlashLight = flashLight.GetComponent<Light2D>();

        BulletSpawnPoint = sprite.Find("BulletSpawnPoint");
        BulletsContainer = GameObject.Find("ProjectilesContainer").transform;
        AudioSource = GetComponent<AudioSource>();

        if (FlashLight != null)
        {
            InnerFlashLight = FlashLight.transform.Find("InnerFlashLight").GetComponent<Light2D>();
            FlashLightStartIntensity = FlashLight.intensity;
            FlashLight.intensity = 0f;
            InnerFlashLight.intensity = 0f;
            FlashLight.gameObject.SetActive(false);
            InnerFlashLight.gameObject.SetActive(false);
        }

        MagDrop = sprite.transform.Find("MagDrop")?.gameObject;
        CartridgeDrop = sprite.transform.Find("CartridgeDrop")?.gameObject;
    }

    protected virtual void Start()
    {
        Player = PlayerWeaponController.transform.parent.GetComponent<Player>();
    }

    protected virtual void Update()
    {
        if (MenuController.Instance.IsGamePaused)
            return;

        // Debug Reload Time
        //if (ReloadStartTime != null)
        //    Debug.Log("Reload time: " + (Time.time - ReloadStartTime.Value));

        Animation();

        if (IsActive)
            SetWeaponOffsets();

        ToggleVisible(IsActive);

        //debug laser
        //Debug.DrawLine(BulletSpawnPoint.position, BulletSpawnPoint.position + BulletSpawnPoint.right * 100f, Color.red);
    }

    protected virtual void FixedUpdate()
    {

    }

    /// <summary>
    /// Executa o comportamento padr�o de tiro de todas as armas, como verificação de munição e firerate, audio, etc.
    /// </summary>
    /// <returns>Uma lista de proj�teis disparados (no caso de escopetas) ou uma lista com apenas um proj�til para as demais armas.</returns>
    public virtual IEnumerable<GameObject> Shoot()
    {
        if (!CanShoot())
        {
            isShooting = false;
            return Enumerable.Empty<GameObject>();
        }

        isShooting = true;

        if (!isDecreasingFlashIntensity)
            StartCoroutine(DecreaseFlashIntensity());

        var angle = PlayerWeaponController.AimAngleDegrees;
        var particlesRotation = Quaternion.Euler(angle + 180, -90f, 0f);
        Instantiate(SmokeParticlesPrefab, BulletSpawnPoint.position, particlesRotation, BulletsContainer);

        lastShotTime = Time.time;

        ShootSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);

        var bullets = CreateBullets(angle);

        return bullets;
    }

    protected virtual List<GameObject> CreateBullets(float angleDegrees)
    {
        var bulletInstance = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.Euler(0f, 0f, angleDegrees), BulletsContainer);
        var bullet = bulletInstance.GetComponent<Projectile>();

        bullet.Type = BulletType;
        bullet.StartPos = BulletSpawnPoint.position;
        bullet.AngleDegrees = PlayerWeaponController.AimAngleDegrees;
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;
        bullet.MinDamage = MinDamage;
        bullet.MaxDistance = BulletMaxRange;
        bullet.MaxDamageRange = MaxDamageRange;
        bullet.MinDamageRange = MinDamageRange;
        bullet.PlayerOwner = Player;
        bullet.HeadshotMultiplier = HeadshotMultiplier;
        bullet.OnBulletKill += OnBulletKill;
        bullet.ShotTime = Time.time;
        bullet.FlyInfinitely = FlyInfinitely;
        bullet.GravityScale = GravityScale;
        bullet.Init();

        WavesManager.Instance.CurrentWave.HandlePlayerAttack(1, 0);

        return new List<GameObject>() { bulletInstance };
    }

    /// <summary>
    /// Diminui a intensidade do brilho/flash do disparo da arma com o tempo.
    /// </summary>
    /// <returns></returns>
    IEnumerator DecreaseFlashIntensity()
    {
        FlashLight.gameObject.SetActive(true);
        InnerFlashLight.gameObject.SetActive(true);
        FlashLight.intensity = FlashLightStartIntensity;
        InnerFlashLight.intensity = FlashLightStartIntensity * 2;

        while (FlashLight.intensity > 0)
        {
            isDecreasingFlashIntensity = true;
            FlashLight.intensity -= FlashLightStartIntensity * 30 * Time.deltaTime;
            InnerFlashLight.intensity = FlashLight.intensity * 2;
            yield return new WaitForSeconds(0.01f);
        }

        isDecreasingFlashIntensity = false;
        FlashLight.gameObject.SetActive(false);
        InnerFlashLight.gameObject.SetActive(false);
    }

    /// <summary>
    /// Se poss�vel, recarrega a arma e Inicia a anima��o de recarregamento.
    /// </summary>
    public virtual bool Reload()
    {
        if (MagazineBullets == MagazineSize)
            return false;

        if (Player.Backpack.GetAmmo(BulletType) <= 0)
            return false;

        if (IsSwitchingWeapon)
            return false;

        if (IsReloading)
            return false;

        if (ReloadStartTime != null && Time.time - ReloadTimeMs <= ReloadStartTime)
            return false;

        IsReloading = true;
        ReloadStartTime = Time.time;

        int toLoad = MagazineSize - MagazineBullets;
        reloadBackpackMagDiff = Player.Backpack.GetAmmo(BulletType) - toLoad;

        return true;
    }

    /// <summary>
    /// Define a arma como visível ou invisível, caso o jogador não esteja com ela na mão.
    /// </summary>
    /// <param name="visible">Se a arma está visível  ou não.</param>
    protected virtual void ToggleVisible(bool visible)
    {
        SpriteRenderer.enabled = visible;
        if (HandSpriteRenderer != null)
            HandSpriteRenderer.enabled = visible;
        if (ShadowCaster != null)
            ShadowCaster.enabled = visible;
        IsActive = visible;
    }

    /// <summary>
    /// Define se a arma está sem munição e deve exibir o alerta de recarregar.
    /// </summary>
    /// <returns>Se a arma precisa de recarregamento ou não.</returns>
    public virtual bool NeedsReload()
    {
        return MagazineBullets <= 0;
    }

    /// <summary>
    /// Função chamada antes de trocar de arma, desativando o sprite desta arma.
    /// </summary>
    public virtual bool BeforeSwitchWeapon()
    {
        isShooting = false;
        IsSwitchingWeapon = true;

        Animator.SetFloat("reloadSpeed", 0);

        return true;
    }

    /// <summary>
    /// Função chamada após trocar de arma de volta, ativando o sprite desta arma.
    /// </summary>
    public virtual void AfterSwitchWeaponBack()
    {
        IsSwitchingWeapon = true;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame de recarregamento da arma.
    /// </summary>
    public virtual void OnReloadEnd()
    {
        IsReloading = false;
        ReloadStartTime = null;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame de pump da arma.
    /// </summary>
    public virtual void OnPumpEnd()
    {
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no frame de tiro da arma.
    /// </summary>
    public virtual void OnShootEnd()
    {
        isShooting = false;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no frame de recarregamento da arma em que o carregador é posicionado.
    /// </summary>
    public virtual void OnReloadedChamber()
    {
        int toLoad = MagazineSize - MagazineBullets;
        if (reloadBackpackMagDiff >= 0)
        {
            Data.MagazineBullets += toLoad;
            Player.Backpack.SetAmmo(BulletType, reloadBackpackMagDiff);
        }
        else
        {
            Data.MagazineBullets += Player.Backpack.GetAmmo(BulletType);
            Player.Backpack.SetAmmo(BulletType, 0);
        }
    }

    /// <summary>
    /// Executa um efeito sonoro extra da arma, para animações de recarregamento, tiro, etc.
    /// </summary>
    /// <param name="index">O índice do som a ser tocado da lista.</param>
    /// <param name="volume">O volume para tocar este som, de 0 a 1.</param>
    public virtual void PlayExtraSoundEffect(int index, float volume = 1f)
    {
        ExtraSoundEffects.PlayAtIndex(index, AudioSource, AudioTypes.Player);
    }

    /// <summary>
    /// Função chamada pelo evento de animação durante o reload, para spawnar o pente/carregador da arma.
    /// </summary>
    public virtual void SpawnMagDrop()
    {
        if (MagDrop == null)
            return;
        var mag = Instantiate(MagDrop, MagDrop.transform.position, MagDrop.transform.rotation);
        var weaponDrop = mag.GetComponent<WeaponDrop>();
        if (weaponDrop != null)
        {
            weaponDrop.TargetScale = MagDrop.transform.lossyScale;
            weaponDrop.SpawnDirection = PlayerWeaponController.IsAimingLeft ? -1 : 1;
        }
        mag.SetActive(true);
    }

    /// <summary>
    /// Função chamada pelo evento de animação durante o tiro, para spawnar o cartucho/cápsula da bala.
    /// </summary>
    public virtual void SpawnCartridgeDrop()
    {
        if(CartridgeDrop == null)
            return;
        var cartridge = Instantiate(CartridgeDrop, CartridgeDrop.transform.position, CartridgeDrop.transform.rotation);
        var weaponDrop = cartridge.GetComponent<WeaponDrop>();
        if (weaponDrop != null)
        {
            weaponDrop.TargetScale = CartridgeDrop.transform.lossyScale;
            weaponDrop.SpawnDirection = PlayerWeaponController.IsAimingLeft ? -1 : 1;
        }
        cartridge.SetActive(true);
    }

    /// <summary>
    /// Verifica se a arma pode ser disparada, levando em conta a muni��o, firerate e recarga.
    /// </summary>
    /// <returns>True se a arma pode disparar, caso contr�rio, false.</returns>
    public virtual bool CanShoot()
    {
        if (!IsActive)
            return false;

        if (MagazineBullets <= 0)
        {
            if (!IsReloading)
                EmptyChamberSound.PlayIfNotNull(AudioSource, AudioTypes.Player, point: transform.position);

            return false;
        }

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

    /// <summary>
    /// Atualiza a posição da arma, de acordo com o offset definido.
    /// </summary>
    public virtual void SetWeaponOffsets()
    {
        PlayerWeaponController.SetWeaponOffset(WeaponContainerOffset);
    }

    /// <summary>
    /// Define a cor de pele da mão do jogador nesta arma.
    /// </summary>
    /// <param name="color">A cor de pele a ser definida na mão.</param>
    public virtual void SetHandSkinColor(Color32 color)
    {
        if (isHandColorSet)
            return;

        if (HandSpriteRenderer != null)
        {
            HandSpriteRenderer.color = color;
            isHandColorSet = true;
        }
    }

    /// <summary>
    /// Processa a anima��o da arma/flip.
    /// </summary>
    protected virtual void Animation()
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

        Animator.SetBool("isActive", IsActive);
        if (IsActive)
            SyncAnimationStates();
    }

    /// <summary>
    /// Sinconiza os estados da animação do Animator com as variáveis de controle.
    /// </summary>
    protected virtual void SyncAnimationStates()
    {
        Animator.SetFloat("shootSpeed", FireRate / 10);
        Animator.SetFloat("reloadSpeed", 1000 / ReloadTimeMs);

        if (IsReloading) Animator.SetTrigger("Reload");
        else Animator.ResetTrigger("Reload");

        if (isShooting) Animator.SetTrigger("Shoot");
        else Animator.ResetTrigger("Shoot");
    }

    /// <summary>
    /// Função chamada quando cada bala disparada é destruída (ao colidir com algo, alcançar o limite, etc).
    /// </summary>
    /// <param name="projectile">A instância do projétil em si, antes de ser destruído.</param>
    /// <param name="playerTarget">O último alvo inimigo que o projétil acertou, nulo caso não tenha acertado.</param>
    /// <param name="enemyTarget">O último alvo player que o projétil acertou, nulo caso não tenha acertado.</param>
    protected virtual void OnBulletKill(Projectile projectile, IPlayerTarget playerTarget, IEnemyTarget enemyTarget)
    {
        if (playerTarget != null)
            WavesManager.Instance.CurrentWave.HandlePlayerAttack(0, 1);
    }
}
