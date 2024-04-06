using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDrop : MonoBehaviour
{
    public BulletTypes Type { get; private set; }
    public int Count { get; private set; }
    public bool Collected { get; private set; }
    public delegate void OnCollect(BulletTypes ammoType, int count);
    public OnCollect CollectFunction { get; set; }

    [SerializeField]
    List<CustomAudio> CollectSounds;
    [SerializeField]
    Image AmmoIcon;
    [SerializeField]
    TextMeshProUGUI CountText;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, FuelAmmoIcon;

    Animator Animator;
    AudioSource AudioSource;
    Rigidbody2D Rigidbody;

    void Start()
    {
        Animator = GetComponentInChildren<Animator>();
        AudioSource = GetComponent<AudioSource>();
        Rigidbody = GetComponent<Rigidbody2D>();
        Rigidbody.AddForce(new Vector2(0, 450));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Collect();
    }

    /// <summary>
    /// Inicializa o drop de munição.
    /// </summary>
    /// <param name="ammoType">O tipo de munição.</param>
    /// <param name="count">A quantidade de munição.</param>
    /// <param name="onCollectFunction">Função que será chamada quando o player coletar a munição.</param>
    public void SetDrop(BulletTypes ammoType, int count, OnCollect onCollectFunction)
    {
        Type = ammoType;
        Count = count;
        CountText.text = $"x{Count}";

        AmmoIcon.sprite = ammoType switch
        {
            BulletTypes.Pistol => PistolBulletIcon,
            BulletTypes.Shotgun => ShotgunBulletIcon,
            BulletTypes.AssaultRifle => RifleAmmoIcon,
            BulletTypes.Sniper => SniperAmmoIcon,
            BulletTypes.Rocket => RocketAmmoIcon,
            BulletTypes.Fuel => FuelAmmoIcon,
            _ => null,
        };

        CollectFunction += onCollectFunction;
    }

    /// <summary>
    /// Coleta a munição.
    /// </summary>
    public void Collect()
    {
        if (Collected)
            return;

        Collected = true;
        Animator.SetTrigger("Collect");
        CollectSounds.PlayRandomIfAny(AudioSource, AudioTypes.Player);
        CollectFunction?.Invoke(Type, Count);
        StartCoroutine(KillSelfDelayed());
    }

    IEnumerator KillSelfDelayed()
    {
        yield return new WaitForSeconds(3f);

        Destroy(gameObject);
    }
}
