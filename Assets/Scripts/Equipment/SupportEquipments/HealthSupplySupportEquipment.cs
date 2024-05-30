using System;
using UnityEngine;

public class HealthSupplySupportEquipment : BaseSupportEquipment
{
    [SerializeField]
    GameObject PackagePrefab, FlareGunProjectilePrefab;

    [SerializeField]
    CustomAudio FlareShootAudio;

    Transform ProjectileSpawnPoint;
    AudioSource AudioSource;
    Animator Animator;
    Vector3 PackageSpawnPoint;
    override protected void Start()
    {
        base.Start();
        AudioSource = GetComponent<AudioSource>();
        Animator = GetComponentInChildren<Animator>();
        ProjectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
    }

    public override void OnTrigger()
    {
        base.OnTrigger();
        FlareShootAudio.PlayIfNotNull(AudioSource, AudioTypes.Player, point: Camera.main.transform.position.WithZ(-5));
        PackageSpawnPoint = transform.position - (Vector3.right * transform.localPosition.x) + (Vector3.up * 15);
        var projectile = Instantiate(FlareGunProjectilePrefab, ProjectileSpawnPoint.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 30, ForceMode2D.Impulse);
    }

    public override void OnTriggerEnd()
    {
        base.OnTriggerEnd();
        var packageObj = Instantiate(PackagePrefab, PackageSpawnPoint, Quaternion.identity);
        var supplyPackage = packageObj.GetComponent<HealthSupplyPackage>();
        supplyPackage.Player = Player;
        OnFinishedUsing?.Invoke();
        gameObject.SetActive(false);
    }

    public override void Use(Action onFinishedUsing)
    {
        base.Use(onFinishedUsing);

        Animator.SetTrigger("Use");
    }
}
