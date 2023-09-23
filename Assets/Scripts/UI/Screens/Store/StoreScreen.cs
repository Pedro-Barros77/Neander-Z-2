using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreScreen : MonoBehaviour
{
    public List<GameObject> StoreItems { get; private set; }
    public StoreItem SelectedItem { get; private set; }
    public StoreTabs ActiveTab { get; private set; } = StoreTabs.Weapons;
    public readonly Color32 RedMoney = new(205, 86, 99, 255);
    public readonly Color32 GreenMoney = new(72, 164, 80, 255);

    [SerializeField]
    public PlayerData PlayerData;
    [SerializeField]
    TextMeshProUGUI PlayerMoneyText, PreviewTitleText, PreviewPriceText, PreviewDescriptionText, PreviewIsPrimaryText, PreviewTagsText, PreviewHeadshotMultiplierText, PreviewMgazineBulletsText, PreviewPelletsCountText, PreviewDispersionText;
    [SerializeField]
    Image PreviewIcon, PreviewBulletIcon;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon, ActiveTabImage, InactiveTabImage;
    [SerializeField]
    Button BuyButton, TestItemButton, BtnReady;
    [SerializeField]
    GameObject StorePanel, PreviewPanelContent, EmptyPreviewPanel, InventorySlotsPanel, InventoryPreviewPanel, InventoryPreviewEmptyPanel, WeaponsContent, ItemsContent, PerksContent, InventoryContent, WeaponsTab, ItemsTab, PerksTab, InventoryTab;
    [SerializeField]
    CustomAudio PurchaseSound;

    TextMeshProUGUI PreviewBtnBuyText, BtnReadyText;
    AudioSource audioSource;
    Animator storePanelAnimator;
    GameObject PopupPrefab;
    Canvas WorldPosCanvas;
    bool hasItem => SelectedItem != null && SelectedItem.Data != null;

    void Start()
    {
        StoreItems = GameObject.FindGameObjectsWithTag("StoreItem").ToList();
        storePanelAnimator = StorePanel.GetComponent<Animator>();
        PreviewBtnBuyText = BuyButton.GetComponentInChildren<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
        PopupPrefab = Resources.Load<GameObject>("Prefabs/UI/Popup");
        WorldPosCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        BtnReadyText = BtnReady.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        InventoryPreviewPanel.SetActive(hasItem);
        InventoryPreviewEmptyPanel.SetActive(!hasItem);
        PreviewPanelContent.SetActive(hasItem);
        EmptyPreviewPanel.SetActive(!hasItem);

        PreviewPanelContent.transform.parent.gameObject.SetActive(ActiveTab != StoreTabs.Inventory);
        InventoryPreviewPanel.transform.parent.gameObject.SetActive(ActiveTab == StoreTabs.Inventory);
        InventorySlotsPanel.SetActive(ActiveTab == StoreTabs.Inventory);


        if (PlayerData != null)
        {
            PlayerMoneyText.text = $"$ {PlayerData.Money:N2}";
            PlayerMoneyText.color = PlayerData.Money > 0 ? GreenMoney : RedMoney;

            if (hasItem)
            {
                PreviewPriceText.color = SelectedItem.Data.CanAfford ? GreenMoney : RedMoney;
                BuyButton.interactable = SelectedItem.Data.CanAfford && !SelectedItem.Data.MaxedUp && !SelectedItem.Data.Purchased;
                PreviewPriceText.text = $"$ {SelectedItem.Data.Price - SelectedItem.Data.Discount:N2}";
                if (SelectedItem.Data.Amount > 1)
                    PreviewBtnBuyText.text = $"Buy +{SelectedItem.Data.Amount}";
                else
                    PreviewBtnBuyText.text = "Buy";

                if (SelectedItem.Data.MaxedUp)
                {
                    BuyButton.interactable = false;
                    PreviewBtnBuyText.text = "Max";
                }
                if (SelectedItem.Data.Purchased)
                {
                    BuyButton.interactable = false;
                    PreviewBtnBuyText.text = "Purchased";
                }
            }

            if (PlayerData.InventoryData.PrimaryWeaponsSelection.Count + PlayerData.InventoryData.SecondaryWeaponsSelection.Count > 0)
            {
                BtnReady.interactable = true;
                BtnReadyText.text = "Ready!";
            }
            else
            {
                BtnReady.interactable = false;
                BtnReadyText.text = "No Weapon";
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayerData.GetMoney(100);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerData.TakeMoney(100);
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
        PreviewDescriptionText.text = item.Data.Description;
        PreviewTagsText.text = string.Join("  |  ", item.Data.Tags).Replace("_", "-");

        if (SelectedItem.Data.IsWeapon)
        {
            var data = item.Data as StoreWeaponData;
            PreviewIsPrimaryText.text = data.IsPrimary ? "-Primary" : "-Secondary";
            PreviewHeadshotMultiplierText.text = data.HeadshotMultiplier.ToString("N1");
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
        InventoryContent.SetActive(tab == StoreTabs.Inventory);

        WeaponsTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);
        ItemsTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);
        PerksTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);
        InventoryTab.transform.SetSiblingIndex((int)StoreTabs.Weapons);

        SetTabActive(WeaponsTab, tab == StoreTabs.Weapons);
        SetTabActive(ItemsTab, tab == StoreTabs.Items);
        SetTabActive(PerksTab, tab == StoreTabs.Perks);
        SetTabActive(InventoryTab, tab == StoreTabs.Inventory);
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

    /// <summary>
    /// Começa a sair da loja.
    /// </summary>
    public void ExitStore()
    {
        storePanelAnimator.SetTrigger("Exit");
        StartCoroutine(ExitStoreAfterAnimation());
    }

    /// <summary>
    /// Sai da loja após a animação de saída.
    /// </summary>
    public IEnumerator ExitStoreAfterAnimation()
    {
        yield return new WaitForSeconds(1f);

        MenuController.Instance.ChangeScene(SceneNames.Graveyard, LoadSceneMode.Single);
    }

    /// <summary>
    /// Função para exibir o popup com devidos parâmetros.
    /// </summary>
    /// <param name="text">Texto a ser exibido</param>
    /// <param name="textColor">A cor que o popup vai ser exibido</param>
    /// <param name="hitPosition">A posição que o popup vai aparecer</param>
    private void ShowPopup(string text, Color32 textColor, Vector3 hitPosition)
    {
        var popup = Instantiate(PopupPrefab, hitPosition, Quaternion.identity, WorldPosCanvas.transform);
        var popupSystem = popup.GetComponent<PopupSystem>();
        if (popupSystem != null)
        {
            popupSystem.Init(text, hitPosition, 22000f, textColor, 50);
        }
    }

    /// <summary>
    /// Função chamada quando o player clica no botão de comprar.
    /// </summary>
    public void BuyItem()
    {
        if (SelectedItem == null || SelectedItem.Data == null)
            return;

        if (!SelectedItem.Data.CanAfford)
            return;

        bool purchased = false;

        if (SelectedItem.Data.IsWeapon)
            purchased = BuyWeapon();

        if (SelectedItem.Data.IsAmmo)
            purchased = BuyAmmo();

        if (SelectedItem.Data.IsThrowable)
            purchased = BuyThrowable();

        switch (SelectedItem.Data.name)
        {
            case "FirstAidKit":
                if (PlayerData.Health < PlayerData.MaxHealth)
                {
                    PlayerData.Health = Mathf.Clamp(PlayerData.Health + SelectedItem.Data.Amount, 0, PlayerData.MaxHealth);
                    purchased = true;
                }
                break;

            case "MedKit":
                if (PlayerData.Health < PlayerData.MaxHealth)
                {
                    PlayerData.Health = PlayerData.MaxHealth;
                    purchased = true;
                }
                break;
        }

        if (purchased)
        {
            float value = SelectedItem.Data.Price - SelectedItem.Data.Discount;

            PlayerData.TakeMoney(value);
            audioSource.PlayOneShot(PurchaseSound.Audio, PurchaseSound.Volume);
            ShowPopup($"-{value:N2}", Color.red, PlayerMoneyText.transform.position);
        }
    }

    /// <summary>
    /// Compra a arma selecionada.
    /// </summary>
    private bool BuyWeapon()
    {
        var data = SelectedItem.Data as StoreWeaponData;

        if (PlayerData.InventoryData.HasWeapon(data.WeaponType))
            return false;

        if (data.IsPrimary)
        {
            PlayerData.InventoryData.UnequipAllWeapons(true, true);
            PlayerData.InventoryData.UnequipAllWeapons(false, true);

            PlayerData.InventoryData.PrimaryWeaponsSelection.Add(new(data.WeaponType, WeaponEquippedSlot.Primary));
        }
        else
        {
            PlayerData.InventoryData.UnequipAllWeapons(false, false);

            PlayerData.InventoryData.SecondaryWeaponsSelection.Add(new(data.WeaponType, WeaponEquippedSlot.Secondary));
        }

        return true;
    }

    /// <summary>
    /// Compra a munição selecionada.
    /// </summary>
    /// <returns>Se o item foi comprado com sucesso.</returns>
    private bool BuyAmmo()
    {
        var data = SelectedItem.Data as StoreAmmoData;

        if (data.Amount <= 0)
            return false;

        int currentAmmo = PlayerData.InventoryData.GetAmmo(data.BulletType);
        int maxAmmo = PlayerData.InventoryData.GetMaxAmmo(data.BulletType);

        if (currentAmmo >= maxAmmo)
            return false;

        if (currentAmmo + data.Amount > maxAmmo)
        {
            int diff = maxAmmo - currentAmmo;
            PlayerData.InventoryData.SetAmmo(data.BulletType, currentAmmo + diff);
        }

        PlayerData.InventoryData.SetAmmo(data.BulletType, (int)data.Amount + currentAmmo);

        return true;
    }

    /// <summary>
    /// Compra o item arremessável selecionado.
    /// </summary>
    /// <returns>Se o item foi comprado com sucesso.</returns>
    private bool BuyThrowable()
    {
        var data = SelectedItem.Data as StoreThrowableData;

        if (data.Amount <= 0)
            return false;

        PlayerData.InventoryData.UnequipAllThrowables();

        bool hasThrowable = PlayerData.InventoryData.HasThrowable(data.ThrowableType);

        if (!hasThrowable)
            PlayerData.InventoryData.ThrowableItemsSelection.Add(new(data.ThrowableType, (int)data.Amount, true));
        else
        {
            var throwable = PlayerData.InventoryData.ThrowableItemsSelection.Find(t => t.Type == data.ThrowableType);
            throwable.Count += (int)data.Amount;
            throwable.IsEquipped = true;
        }

        return true;
    }
}
