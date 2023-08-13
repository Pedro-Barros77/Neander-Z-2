using Unity.Mathematics;
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

    protected Transform BulletSpawnPoint;
    protected Transform BulletsContainer;

    protected virtual void Start()
    {
        BulletSpawnPoint = transform.GetChild(0).Find("BulletSpawnPoint");
        BulletsContainer = GameObject.Find("ProjectilesContainer").transform;
    }

    protected virtual void Update()
    {
        Animation();
    }

    public virtual GameObject Shoot()
    {
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

        return bulletInstance;
    }

    private void Animation()
    {
        bool aimingLeft = math.abs(AimingScript.AimAngle) > 90;
        if (aimingLeft)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
}
