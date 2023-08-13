using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public float Damage { get; set; }
    public float FireRate { get; set; }
    public int MagazineSize { get; set; }
    public int MagazineBullets { get; set; }
    public float BulletSpeed { get; set; }
    public float MaxDamageRange { get; set; }
    public float MinDamageRange { get; set; }
    public bool IsPrimary { get; set; }
    public float BulletMaxRange { get; set; }
    public float ReloadTimeMs { get; set; }
    public BulletTypes BulletType { get; set; }

    [SerializeField]
    protected PlayerAiming AimingScript;
    [SerializeField]
    protected GameObject BulletPrefab;
    [SerializeField]
    protected List<AudioClip> ShootSounds;

    protected Transform BulletSpawnPoint;
    protected Transform BulletsContainer;
    protected AudioSource AudioSource;
    protected float ShootVolume;

    protected float? lastShotTime;
    protected float? lastReloadTime;
    protected const float FIRE_RATE_RATIO = 1000;

    protected virtual void Start()
    {
        BulletSpawnPoint = transform.GetChild(0).Find("BulletSpawnPoint");
        BulletsContainer = GameObject.Find("ProjectilesContainer").transform;
        AudioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
        Animation();
    }

    public virtual IEnumerable<GameObject> Shoot()
    {
        if (!CanShoot())
        {
            return Enumerable.Empty<GameObject>();
        }

        var angle = AimingScript.AimAngle;

        var bulletInstance = Instantiate(BulletPrefab, BulletSpawnPoint.position, Quaternion.Euler(0f, 0f, angle), BulletsContainer);
        var bullet = bulletInstance.GetComponent<Projectile>();

        bullet.Type = BulletTypes.Pistol;
        bullet.StartPos = BulletSpawnPoint.position;
        bullet.Angle = AimingScript.AimAngle;
        bullet.Speed = BulletSpeed;
        bullet.Damage = Damage;
        bullet.MaxDistance = BulletMaxRange;
        bullet.Init();

        lastShotTime = Time.time;
        AudioSource.clip = ShootSounds[UnityEngine.Random.Range(0, ShootSounds.Count)];
        AudioSource.volume = ShootVolume;
        AudioSource.Play();

        return new List<GameObject>() { bulletInstance };
    }

    public virtual bool CanShoot()
    {
        if (MagazineBullets <= 0)
        {
            return false;
        }

        var now = Time.time;

        if (lastReloadTime != null && now - ReloadTimeMs <= lastReloadTime)
            return false;

        var delayMs = FIRE_RATE_RATIO / FireRate;
        var diff = now - (delayMs/1000);

        if (lastShotTime != null && diff <= lastShotTime)
            return false;

        return true;
    }

    protected virtual void Animation()
    {
        bool aimingLeft = math.abs(AimingScript.AimAngle) > 90;
        if (aimingLeft)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}
