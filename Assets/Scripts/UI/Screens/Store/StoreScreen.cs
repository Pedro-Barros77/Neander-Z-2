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
    public StoreTabs ActiveTab { get; private set; } = StoreTabs.Weapons;
    public readonly Color32 RedMoney = new(205, 86, 99, 255);
    public readonly Color32 GreenMoney = new(72, 164, 80, 255);

    [SerializeField]
    TextMeshProUGUI PlayerMoneyText, PreviewTitleText, PreviewPriceText, PreviewDescriptionText, PreviewIsPrimaryText, PreviewTagsText, PreviewHeadshotMultiplierText, PreviewMgazineBulletsText, PreviewPelletsCountText, PreviewDispersionText;
    [SerializeField]
    Image PreviewIcon, PreviewBulletIcon;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon, ActiveTabImage, InactiveTabImage;
    [SerializeField]
    Button BuyButton, TestItemButton;
    [SerializeField]
    GameObject PreviewPanelContent, EmptyPreviewPanel, WeaponsContent, ItemsContent, PerksContent, BackpackContent, WeaponsTab, ItemsTab, PerksTab, BackpackTab;

    bool hasItem => SelectedItem != null && SelectedItem.Data != null;

    void Start()
    {
        StoreItems = GameObject.FindGameObjectsWithTag("StoreItem").ToList();
        Player = GameObject.FindAnyObjectByType<Player>(FindObjectsInactive.Include);
    }

    void Update()
    {
        PreviewPanelContent.SetActive(hasItem);
        EmptyPreviewPanel.SetActive(!hasItem);

        if (Player != null)
        {
            PlayerMoneyText.text = $"$ {Player.Money:N2}";
            PlayerMoneyText.color = Player.Money > 0 ? GreenMoney : RedMoney;

            if (hasItem)
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

    /// <summary>
    /// Função chamada pelo StoreItem quando o mesmo é selecionado.
    /// </summary>
    /// <param name="item">O Item a ser selecionado.</param>
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

    /// <summary>
    /// Função chamada quando o player clica fora de qualquer elemento UI (Clique no background)..
    /// </summary>
    public void OnClickOutside()
    {
        if (SelectedItem != null)
        {
            SelectedItem.Deselect();
            SelectedItem = null;
        }
    }

    /// <summary>
    /// Troca a aba da loja ativa.
    /// </summary>
    /// <param name="tab">O index da aba a ser ativada.</param>
    public void ChangeTab(int tabIndex) => ChangeTab((StoreTabs)tabIndex);

    /// <summary>
    /// Troca a aba da loja ativa.
    /// </summary>
    /// <param name="tab">A aba a ser ativada.</param>
    public void ChangeTab(StoreTabs tab)
    {
        ActiveTab = tab;
        if (SelectedItem != null)
        {
            SelectedItem.Deselect();
            SelectedItem.Animator.Play("Normal");
            SelectedItem = null;
        }

        WeaponsContent.SetActive(tab == StoreTabs.Weapons);
        ItemsContent.SetActive(tab == StoreTabs.Items);
        PerksContent.SetActive(tab == StoreTabs.Perks);
        BackpackContent.SetActive(tab == StoreTabs.Backpack);

        WeaponsTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);
        ItemsTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);
        PerksTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);
        BackpackTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);

        SetTabActive(WeaponsTab, tab == StoreTabs.Weapons);
        SetTabActive(ItemsTab, tab == StoreTabs.Items);
        SetTabActive(PerksTab, tab == StoreTabs.Perks);
        SetTabActive(BackpackTab, tab == StoreTabs.Backpack);
    }

    /// <summary>
    /// Atualiza o estilo da aba, quando esta ativa ou inativa.
    /// </summary>
    /// <param name="tab">O GameObject da aba a ser estilizada.</param>
    /// <param name="active">Se ela está ativa ou não.</param>
    public void SetTabActive(GameObject tab, bool active)
    {
        var rect = tab.GetComponent<RectTransform>();
        var image = tab.GetComponent<Image>();
        var labelText = tab.GetComponentInChildren<TextMeshProUGUI>();

        if (active)
            tab.transform.SetAsLastSibling();

        rect.offsetMax = new Vector2(rect.offsetMax.x, active ? 0 : -5);
        image.sprite = active ? ActiveTabImage : InactiveTabImage;
        labelText.fontSize = active ? 18 : 15;
    }
}
