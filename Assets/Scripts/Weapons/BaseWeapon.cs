using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    /// <summary>
    /// O dano causado pela arma ou seus proj�teis.
    /// </summary>
    public float Damage { get; set; }
    /// <summary>
    /// A taxa de disparo da arma (disparos por segundo).
    /// </summary>
    public float FireRate { get; set; }
    /// <summary>
    /// Capacidade m�xima do carregador.
    /// </summary>
    public int MagazineSize { get; set; }
    /// <summary>
    /// Quantidade de muni��es restantes no carregador.
    /// </summary>
    public int MagazineBullets { get; set; }
    /// <summary>
    /// Velocidade de movimento dos proj�teis.
    /// </summary>
    public float BulletSpeed { get; set; }
    /// <summary>
    /// A dist�ncia m�xima que o proj�til pode percorrer antes de ser destru�do.
    /// </summary>
    public float BulletMaxRange { get; set; }
    /// <summary>
    /// O alcance em que o proj�til causa dano m�ximo.
    /// </summary>
    public float MaxDamageRange { get; set; }
    /// <summary>
    /// A dist�ncia em que o proj�til come�a a causar dano m�nimo.
    /// </summary>
    public float MinDamageRange { get; set; }
    /// <summary>
    /// Se a arma � prim�ria ou secund�ria.
    /// </summary>
    public bool IsPrimary { get; set; }
    /// <summary>
    /// O tempo de recarga da arma, em milissegundos.
    /// </summary>
    public float ReloadTimeMs { get; set; }
    /// <summary>
    /// O tipo de proj�til que a arma dispara.
    /// </summary>
    public BulletTypes BulletType { get; set; }
    /// <summary>
    /// O tipo de arma.
    /// </summary>
    public WeaponTypes Type { get; set; }
    /// <summary>
    /// Se a arma est� sendo recarregada atualmente.
    /// </summary>
    public bool IsReloading { get; set; }

    /// <summary>
    /// Script respons�vel por controlar a arma do jogador, como mira, troca e recarregamento.
    /// </summary>
    public PlayerWeaponController PlayerWeaponController { get; set; }
    /// <summary>
    /// Jogador portador desta arma.
    /// </summary>
    public Player Player { get; set; }

    [SerializeField]
    protected GameObject BulletPrefab;
    [SerializeField]
    protected List<AudioClip> ShootSounds;

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
    /// Volume do som de disparo da arma, entre 0 e 1.
    /// </summary>
    protected float ShootVolume;
    /// <summary>
    /// �ltima vez em que a arma foi disparada.
    /// </summary>
    protected float? lastShotTime;
    /// <summary>
    /// Tempo em que a arma come�ou a ser recarregada.
    /// </summary>
    protected float? reloadStartTime;
    /// <summary>
    /// Diferen�a entre a quantidade de muni��es necess�rias para completar o carregador e a quantidade de muni��es dispon�veis na mochila.
    /// </summary>
    protected int reloadBackpackMagDiff;
    /// <summary>
    /// A propor��o entre a taxa de disparo e o tempo em milissegundos.
    /// </summary>
    protected const float FIRE_RATE_RATIO = 1000;

    protected virtual void Awake()
    {
        IsPrimary = Constants.IsPrimaryWeapon[Type];
    }

    protected virtual void Start()
    {
        BulletSpawnPoint = transform.GetChild(0).Find("BulletSpawnPoint");
        BulletsContainer = GameObject.Find("ProjectilesContainer").transform;
        AudioSource = GetComponent<AudioSource>();
        Player = PlayerWeaponController.transform.parent.GetComponent<Player>();
        Animator = transform.Find("Sprite").GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        Animation();
    }

    /// <summary>
    /// Executa o comportamento padr�o de tiro de todas as armas, como verifica��o de muni��o e firerate, audio, e instancia��o dos proj�teis.
    /// </summary>
    /// <returns>Uma lista de proj�teis disparados (no caso de escopetas) ou uma lista com apenas um proj�til para as demais armas.</returns>
    public virtual IEnumerable<GameObject> Shoot()
    {
        if (!CanShoot())
        {
            return Enumerable.Empty<GameObject>();
        }

        var angle = PlayerWeaponController.AimAngle;

        var bulletInstance = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.Euler(0f, 0f, angle), BulletsContainer);
        var bullet = bulletInstance.GetComponent<Projectile>();

        bullet.Type = BulletTypes.Pistol;
        bullet.StartPos = BulletSpawnPoint.position;
        bullet.Angle = PlayerWeaponController.AimAngle;
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;
        bullet.MaxDistance = BulletMaxRange;
        bullet.MaxDamageRange = MaxDamageRange;
        bullet.MinDamageRange = MinDamageRange;
        bullet.Init();

        lastShotTime = Time.time;
        AudioSource.volume = ShootVolume;
        var randomShootSound = ShootSounds[UnityEngine.Random.Range(0, ShootSounds.Count)];
        AudioSource.PlayClipAtPoint(randomShootSound, transform.position);

        return new List<GameObject>() { bulletInstance };
    }

    /// <summary>
    /// Se poss�vel, recarrega a arma e Inicia a anima��o de recarregamento.
    /// </summary>
    public virtual void Reload()
    {
        if (MagazineBullets == MagazineSize)
            return;

        if (Player.Backpack.GetAmmo(BulletType) <= 0)
            return;

        if (reloadStartTime != null && Time.time - ReloadTimeMs <= reloadStartTime)
            return;

        IsReloading = true;
        reloadStartTime = Time.time;

        int toLoad = MagazineSize - MagazineBullets;
        reloadBackpackMagDiff = Player.Backpack.GetAmmo(BulletType) - toLoad;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame de recarregamento da arma.
    /// </summary>
    public virtual void OnReloadEnd()
    {
        IsReloading = false;
        reloadStartTime = null;
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no frame de recarregamento da arma em que o carregador é posicionado.
    /// </summary>
    public virtual void OnReloadedChamber()
    {
        int toLoad = MagazineSize - MagazineBullets;
        if (reloadBackpackMagDiff >= 0)
        {
            MagazineBullets += toLoad;
            Player.Backpack.SetAmmo(BulletType, reloadBackpackMagDiff);
        }
        else
        {
            MagazineBullets += Player.Backpack.GetAmmo(BulletType);
            Player.Backpack.SetAmmo(BulletType, 0);
        }
    }

    /// <summary>
    /// Verifica se a arma pode ser disparada, levando em conta a muni��o, firerate e recarga.
    /// </summary>
    /// <returns>True se a arma pode disparar, caso contr�rio, false.</returns>
    public virtual bool CanShoot()
    {
        if (MagazineBullets <= 0)
            return false;

        var now = Time.time;

        if (IsReloading)
            return false;

        var delayMs = FIRE_RATE_RATIO / FireRate;
        var diff = now - (delayMs / 1000);

        if (lastShotTime != null && diff <= lastShotTime)
            return false;

        return true;
    }

    /// <summary>
    /// Processa a anima��o da arma/flip.
    /// </summary>
    protected virtual void Animation()
    {
        bool aimingLeft = math.abs(PlayerWeaponController.AimAngle) > 90;
        if (aimingLeft)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}
