using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreScreen : MonoBehaviour
{
    public Player Player { get; set; }
    public List<GameObject> StoreItems { get; private set; }
    public StoreItem SelectedItem { get; private set; }
    public Color32 GreenMoney { get; set; } = new(72, 164, 80, 255);
    public Color32 RedMoney { get; set; } = new(205, 86, 99, 255);

    [SerializeField]
    TextMeshProUGUI PlayerMoneyText, PreviewTitleText, PreviewPriceText, PreviewDescriptionText, PreviewIsPrimaryText, PreviewTagsText, PreviewHeadshotMultiplierText, PreviewMgazineBulletsText, PreviewPelletsCountText, PreviewDispersionText;
    [SerializeField]
    Image PreviewIcon, PreviewBulletIcon;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon;
    [SerializeField]
    Button BuyButton, TestItemButton;

    void Start()
    {
        StoreItems = GameObject.FindGameObjectsWithTag("StoreItem").ToList();
        Player = GameObject.FindAnyObjectByType<Player>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (Player != null)
        {
            PlayerMoneyText.text = $"$ {Player.Money:N2}";
            PlayerMoneyText.color = Player.Money > 0 ? GreenMoney : RedMoney;

            if (SelectedItem != null && SelectedItem.Data != null)
            {
                PreviewPriceText.color = SelectedItem.Data.CanAfford ? GreenMoney : RedMoney;
                BuyButton.interactable = SelectedItem.Data.CanAfford;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Player.GetMoney(100);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Player.TakeMoney(100);
        }
    }

    public void SelectItem(StoreItem item)
    {
        if (SelectedItem != null)
            SelectedItem.Deselect();

        SelectedItem = item;
        SelectedItem.Select();

        PreviewIcon.sprite = item.Data.Icon;
        PreviewTitleText.text = item.Data.Title;
        PreviewPriceText.text = $"$ {item.Data.Price:N2}";
        PreviewDescriptionText.text = item.Data.Description;
        PreviewTagsText.text = string.Join("  |  ", item.Data.Tags).Replace("_", "-");

        if (SelectedItem.Data.IsWeapon)
        {
            var data = item.Data as StoreWeaponData;
            PreviewIsPrimaryText.text = data.IsPrimary ? "-Primary" : "-Secondary";
            PreviewHeadshotMultiplierText.text = data.HeadshotMultiplier.ToString();
            PreviewMgazineBulletsText.text = data.MagazineBullets.ToString();
            PreviewPelletsCountText.text = data.PelletsCount.ToString();
            PreviewDispersionText.text = data.Dispersion.ToString();

            PreviewPelletsCountText.transform.parent.gameObject.SetActive(data.PelletsCount > 0);
            PreviewDispersionText.transform.parent.gameObject.SetActive(data.Dispersion > 0);

            PreviewBulletIcon.sprite = data.BulletType switch
            {
                BulletTypes.Pistol => PistolBulletIcon,
                BulletTypes.Shotgun => ShotgunBulletIcon,
                BulletTypes.AssaultRifle => RifleAmmoIcon,
                BulletTypes.Sniper => SniperAmmoIcon,
                BulletTypes.Rocket => RocketAmmoIcon,
                BulletTypes.Melee => MeleeAmmoIcon,
                _ => null,
            };
        }
    }
}
