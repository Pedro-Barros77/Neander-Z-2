using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    /// <summary>
    /// O dano causado pela arma ou seus projéteis.
    /// </summary>
    public float Damage { get; set; }
    /// <summary>
    /// A taxa de disparo da arma (disparos por segundo).
    /// </summary>
    public float FireRate { get; set; }
    /// <summary>
    /// Capacidade máxima do carregador.
    /// </summary>
    public int MagazineSize { get; set; }
    /// <summary>
    /// Quantidade de munições restantes no carregador.
    /// </summary>
    public int MagazineBullets { get; set; }
    /// <summary>
    /// Velocidade de movimento dos projéteis.
    /// </summary>
    public float BulletSpeed { get; set; }
    /// <summary>
    /// A distância máxima que o projétil pode percorrer antes de ser destruído.
    /// </summary>
    public float BulletMaxRange { get; set; }
    /// <summary>
    /// O alcance em que o projétil causa dano máximo.
    /// </summary>
    public float MaxDamageRange { get; set; }
    /// <summary>
    /// A distância em que o projétil começa a causar dano mínimo.
    /// </summary>
    public float MinDamageRange { get; set; }
    /// <summary>
    /// Se a arma é primária ou secundária.
    /// </summary>
    public bool IsPrimary { get; set; }
    /// <summary>
    /// O tempo de recarga da arma, em milissegundos.
    /// </summary>
    public float ReloadTimeMs { get; set; }
    /// <summary>
    /// O tipo de projétil que a arma dispara.
    /// </summary>
    public BulletTypes BulletType { get; set; }
    /// <summary>
    /// O tipo de arma.
    /// </summary>
    public WeaponTypes Type { get; set; }
    /// <summary>
    /// Se a arma está sendo recarregada atualmente.
    /// </summary>
    public bool IsReloading { get; set; }

    /// <summary>
    /// Script responsável por controlar a arma do jogador, como mira, troca e recarregamento.
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
    /// A posição em que os projéteis devem ser instanciados, à frente do cano da arma.
    /// </summary>
    protected Transform BulletSpawnPoint;
    /// <summary>
    /// Objeto vazio na cena que contém todos os projéteis instanciados.
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
    /// Última vez em que a arma foi disparada.
    /// </summary>
    protected float? lastShotTime;
    /// <summary>
    /// Tempo em que a arma começou a ser recarregada.
    /// </summary>
    protected float? reloadStartTime;
    /// <summary>
    /// A proporção entre a taxa de disparo e o tempo em milissegundos.
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
    }

    protected virtual void Update()
    {
        Animation();
    }

    /// <summary>
    /// Executa o comportamento padrão de tiro de todas as armas, como verificação de munição e firerate, audio, e instanciação dos projéteis.
    /// </summary>
    /// <returns>Uma lista de projéteis disparados (no caso de escopetas) ou uma lista com apenas um projétil para as demais armas.</returns>
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
        AudioSource.clip = ShootSounds[UnityEngine.Random.Range(0, ShootSounds.Count)];
        AudioSource.volume = ShootVolume;
        AudioSource.Play();

        return new List<GameObject>() { bulletInstance };
    }

    /// <summary>
    /// Se possível, recarrega a arma e Inicia a animação de recarregamento.
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
        int diff = Player.Backpack.GetAmmo(BulletType) - toLoad;

        if (diff >= 0)
        {
            MagazineBullets += toLoad;
            Player.Backpack.SetAmmo(BulletType, diff);
        }
        else
        {
            MagazineBullets += Player.Backpack.GetAmmo(BulletType);
            Player.Backpack.SetAmmo(BulletType, 0);
        }

        // DEBUG, setar essa linha no final da animação de reload
        reloadStartTime = null;
    }

    /// <summary>
    /// Verifica se a arma pode ser disparada, levando em conta a munição, firerate e recarga.
    /// </summary>
    /// <returns>True se a arma pode disparar, caso contrário, false.</returns>
    public virtual bool CanShoot()
    {
        if (MagazineBullets <= 0)
        {
            return false;
        }

        var now = Time.time;

        if (reloadStartTime != null && now - ReloadTimeMs <= reloadStartTime)
            return false;

        var delayMs = FIRE_RATE_RATIO / FireRate;
        var diff = now - (delayMs / 1000);

        if (lastShotTime != null && diff <= lastShotTime)
            return false;

        return true;
    }

    /// <summary>
    /// Processa a animação da arma/flip.
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
