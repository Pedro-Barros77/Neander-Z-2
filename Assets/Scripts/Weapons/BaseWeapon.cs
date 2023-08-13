using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public float Damage { get; set; }
    public float FireRate { get; set; }
    public int MagazineSize { get; set; }
    public int MagazineBullets{ get; set; }
    public float BulletSpeed { get; set; }
    public bool IsPrimary { get; set; }
    public float BulletMaxRange { get; set; }
    public float ReloadTimeMs { get; set; }

    [SerializeField]
    PlayerAiming AimingScript;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        var angle = AimingScript.AimAngle;
    }
}
