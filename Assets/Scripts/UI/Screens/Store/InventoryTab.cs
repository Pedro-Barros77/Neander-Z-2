using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour
{
    StoreScreen storeScreen;

    [SerializeField]
    Transform weaponsColumn, throwablesColumn, itemsColumn, skillsColumn;
    [SerializeField]
    GameObject StoreItemPrefab;

    [SerializeField]
    TextMeshProUGUI PistolAmmo, PistolMaxAmmo, ShotgunAmmo, ShotgunMaxAmmo, RifleAmmo, RifleMaxAmmo, SniperAmmo, SniperMaxAmmo, RocketAmmo, RocketMaxAmmo;
    [SerializeField]
    TextMeshProUGUI PreviewTitleText, PreviewHeadshotMultiplierText, PreviewMagazineBulletsText, PreviewPelletsCountText, PreviewDispersionText, PreviewBtnSellText;
    [SerializeField]
    Image PreviewBulletIcon;
    [SerializeField]
    Button BtnSwitchWeapons, BtnSell;
    [SerializeField]
    SectionedBar DamageBar, FireRateBar, ReloadSpeedBar, RangeBar, BulletSpeedBar;
    [SerializeField]
    BaseButton BtnUpgradeDamage, BtnUpgradeFireRate, BtnUpgradeReloadSpeed, BtnUpgradeRange, BtnUpgradeBulletSpeed, BtnUpgradeHeadshotMultiplier, BtnUpgradeMagazineCapacity, BtnUpgradeBallinsDispersion;

    TextMeshProUGUI UpgradeDamagePrice, UpgradeFireRatePrice, UpgradeReloadSpeedPrice, UpgradeRangePrice, UpgradeBulletSpeedPrice, UpgradeHeadshotMultiplierPrice, UpgradeMagazineCapacityPrice, UpgradeBallinsDispersionPrice;

    [SerializeField]
    public InventorySlot PrimarySlot, SecondarySlot, GrenadeSlot, DeployableSlot, SupportSlot, AbilitySlot, SkillSlot;

    CanvasGroup PistolAmmoGroup, ShotgunAmmoGroup, RifleAmmoGroup, SniperAmmoGroup, RocketAmmoGroup;

    InventoryData Inventory => storeScreen.PlayerData.InventoryData;
    bool columnsLayoutDirty, loaded;

    void Start()
    {
        PistolAmmoGroup = PistolAmmo.transform.parent.GetComponent<CanvasGroup>();
        ShotgunAmmoGroup = ShotgunAmmo.transform.parent.GetComponent<CanvasGroup>();
        RifleAmmoGroup = RifleAmmo.transform.parent.GetComponent<CanvasGroup>();
        SniperAmmoGroup = SniperAmmo.transform.parent.GetComponent<CanvasGroup>();
        RocketAmmoGroup = RocketAmmo.transform.parent.GetComponent<CanvasGroup>();

        storeScreen = GetComponent<StoreScreen>();
        if (storeScreen.ActiveTab == StoreTabs.Inventory)
            LoadAll();

        BtnUpgradeDamage.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.Damage, button, hovered);
        BtnUpgradeFireRate.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.FireRate, button, hovered);
        BtnUpgradeReloadSpeed.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.ReloadSpeed, button, hovered);
        BtnUpgradeRange.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.Range, button, hovered);
        BtnUpgradeBulletSpeed.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.BulletSpeed, button, hovered);
        BtnUpgradeHeadshotMultiplier.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.HeadshotMultiplier, button, hovered);
        BtnUpgradeMagazineCapacity.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.MagazineSize, button, hovered);
        BtnUpgradeBallinsDispersion.HoverEvent += (BaseButton button, bool hovered) => OnHoverUpgrade(WeaponAttributes.BallinsConcentration, button, hovered);

        UpgradeDamagePrice = BtnUpgradeDamage.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeFireRatePrice = BtnUpgradeFireRate.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeReloadSpeedPrice = BtnUpgradeReloadSpeed.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeRangePrice = BtnUpgradeRange.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeBulletSpeedPrice = BtnUpgradeBulletSpeed.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeHeadshotMultiplierPrice = BtnUpgradeHeadshotMultiplier.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeMagazineCapacityPrice = BtnUpgradeMagazineCapacity.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeBallinsDispersionPrice = BtnUpgradeBallinsDispersion.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (storeScreen.PlayerData != null)
        {
            PistolAmmo.text = Inventory.PistolAmmo.ToString();
            PistolMaxAmmo.text = $"/{Inventory.MaxPistolAmmo}";
            PistolAmmo.color = Constants.GetAlertColor(Inventory.PistolAmmo, Inventory.MaxPistolAmmo, 0.2f);

            ShotgunAmmo.text = Inventory.ShotgunAmmo.ToString();
            ShotgunMaxAmmo.text = $"/{Inventory.MaxShotgunAmmo}";
            ShotgunAmmo.color = Constants.GetAlertColor(Inventory.ShotgunAmmo, Inventory.MaxShotgunAmmo, 0.2f);

            RifleAmmo.text = Inventory.RifleAmmo.ToString();
            RifleMaxAmmo.text = $"/{Inventory.MaxRifleAmmo}";
            RifleAmmo.color = Constants.GetAlertColor(Inventory.RifleAmmo, Inventory.MaxRifleAmmo, 0.2f);

            SniperAmmo.text = Inventory.SniperAmmo.ToString();
            SniperMaxAmmo.text = $"/{Inventory.MaxSniperAmmo}";
            SniperAmmo.color = Constants.GetAlertColor(Inventory.SniperAmmo, Inventory.MaxSniperAmmo, 0.2f);

            RocketAmmo.text = Inventory.RocketAmmo.ToString();
            RocketMaxAmmo.text = $"/{Inventory.MaxRocketAmmo}";
            RocketAmmo.color = Constants.GetAlertColor(Inventory.RocketAmmo, Inventory.MaxSniperAmmo, 0.2f);

            if (storeScreen.hasItem && storeScreen.SelectedItem.Data is StoreWeaponData storeWeapon && UpgradeDamagePrice != null)
            {
                void SetUpgrade(WeaponAttributes attribute, BaseButton button, TextMeshProUGUI priceText)
                {
                    var upgradeMap = GetWeaponUpgradeMap(storeWeapon.WeaponData, attribute);
                    var upgradeStep = GetWeaponUpgradeStep(storeWeapon.WeaponData, attribute);

                    if (upgradeMap == null)
                        return;

                    if (upgradeStep == null)
                    {
                        priceText.text = "MAX";
                        button.button.interactable = false;
                        return;
                    }

                    if (upgradeStep.Price > storeScreen.PlayerData.Money)
                    {
                        priceText.color = Constants.Colors.RedMoney;
                        button.button.interactable = false;
                    }
                    else
                    {
                        priceText.color = Constants.Colors.GreenMoney;
                        button.button.interactable = true;
                    }

                    priceText.text = $"$ {upgradeStep.Price:N2}";
                }

                SetUpgrade(WeaponAttributes.Damage, BtnUpgradeDamage, UpgradeDamagePrice);
                SetUpgrade(WeaponAttributes.FireRate, BtnUpgradeFireRate, UpgradeFireRatePrice);
                SetUpgrade(WeaponAttributes.ReloadSpeed, BtnUpgradeReloadSpeed, UpgradeReloadSpeedPrice);
                SetUpgrade(WeaponAttributes.Range, BtnUpgradeRange, UpgradeRangePrice);
                SetUpgrade(WeaponAttributes.BulletSpeed, BtnUpgradeBulletSpeed, UpgradeBulletSpeedPrice);
                SetUpgrade(WeaponAttributes.HeadshotMultiplier, BtnUpgradeHeadshotMultiplier, UpgradeHeadshotMultiplierPrice);
                SetUpgrade(WeaponAttributes.MagazineSize, BtnUpgradeMagazineCapacity, UpgradeMagazineCapacityPrice);
                SetUpgrade(WeaponAttributes.BallinsConcentration, BtnUpgradeBallinsDispersion, UpgradeBallinsDispersionPrice);
            }
        }

        BtnSwitchWeapons.interactable = PrimarySlot.Data == null || (PrimarySlot.Data is StoreWeaponData primSlotWeapon && !primSlotWeapon.WeaponData.IsPrimary);

        if (storeScreen.ActiveTab == StoreTabs.Inventory)
        {
            if (!loaded)
                LoadAll();

            if (columnsLayoutDirty)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(weaponsColumn.transform.parent.GetComponent<RectTransform>());
                columnsLayoutDirty = false;
            }
        }

    }

    /// <summary>
    /// Troca as armas entre os slots prim�rio e secund�rio.
    /// </summary>
    public void SwitchWeapons()
    {
        if (PrimarySlot.Data is StoreWeaponData primSlotWeapon && primSlotWeapon.WeaponData.IsPrimary)
            return;

        StoreItem primary = PrimarySlot.Item, secondary = SecondarySlot.Item;

        PrimarySlot.DropItem(secondary);
        SecondarySlot.DropItem(primary);
    }

    /// <summary>
    /// Cria um item de invent�rio.
    /// </summary>
    /// <param name="data">Os dados deste item.</param>
    /// <param name="updateUiLayout">Se o layout deve ser atualizado ap�s a adi��o.</param>
    /// <returns>O item criado.</returns>
    public StoreItem CreateInventoryItem(StoreItemData data, bool updateUiLayout = false)
    {
        Transform column;
        StoreItem prefabItem = StoreItemPrefab.GetComponent<StoreItem>();
        if (data is StoreWeaponData weaponData)
        {
            prefabItem.Data = weaponData;
            column = weaponsColumn;
        }
        else if (data is StoreThrowableData throwableData)
        {
            prefabItem.Data = throwableData;
            column = throwablesColumn;
        }
        else if (data is StoreTacticalAbilityData abilityData)
        {
            prefabItem.Data = abilityData;
            column = skillsColumn;
        }
        else
        {
            column = itemsColumn;
        }

        GameObject weaponStoreItem = Instantiate(StoreItemPrefab, column);
        StoreItem storeItem = weaponStoreItem.GetComponent<StoreItem>();
        storeItem.IsInventoryItem = true;

        if (updateUiLayout)
        {
            columnsLayoutDirty = true;
        }

        return storeItem;
    }

    /// <summary>
    /// Fun��o chamada por cada slot sempre que seu conte�do � alterado.
    /// </summary>
    public void OnSlotChanged()
    {
        var primaryWeapon = PrimarySlot.Item?.Data as StoreWeaponData;
        var secondaryWeapon = SecondarySlot.Item?.Data as StoreWeaponData;

        PistolAmmoGroup.alpha = primaryWeapon?.WeaponData.BulletType == BulletTypes.Pistol || secondaryWeapon?.WeaponData.BulletType == BulletTypes.Pistol ? 1 : 0.1f;
        ShotgunAmmoGroup.alpha = primaryWeapon?.WeaponData.BulletType == BulletTypes.Shotgun || secondaryWeapon?.WeaponData.BulletType == BulletTypes.Shotgun ? 1 : 0.1f;
        RifleAmmoGroup.alpha = primaryWeapon?.WeaponData.BulletType == BulletTypes.AssaultRifle || secondaryWeapon?.WeaponData.BulletType == BulletTypes.AssaultRifle ? 1 : 0.1f;
        SniperAmmoGroup.alpha = primaryWeapon?.WeaponData.BulletType == BulletTypes.Sniper || secondaryWeapon?.WeaponData.BulletType == BulletTypes.Sniper ? 1 : 0.1f;
        RocketAmmoGroup.alpha = primaryWeapon?.WeaponData.BulletType == BulletTypes.Rocket || secondaryWeapon?.WeaponData.BulletType == BulletTypes.Rocket ? 1 : 0.1f;
    }

    /// <summary>
    /// Carrega todos os itens do invent�rio.
    /// </summary>
    void LoadAll()
    {
        ClearInventoryItems();
        LoadWeapons();
        LoadThrowables();
        LoadTacticalAbilities();
        loaded = true;
    }

    /// <summary>
    /// Limpa todos os items do grid do invent�rio.
    /// </summary>
    void ClearInventoryItems()
    {
        foreach (Transform weapon in weaponsColumn)
            Destroy(weapon.gameObject);
        foreach (Transform throwable in throwablesColumn)
            Destroy(throwable.gameObject);
        foreach (Transform item in itemsColumn)
            Destroy(item.gameObject);
        foreach (Transform skill in skillsColumn)
            Destroy(skill.gameObject);
    }

    /// <summary>
    /// Carrega as armas do invent�rio.
    /// </summary>
    void LoadWeapons()
    {
        var weaponDatas = Resources.LoadAll<StoreWeaponData>("ScriptableObjects/Store/Weapons");
        var primaryWeapon = Inventory.PrimaryWeaponsSelection.Concat(Inventory.SecondaryWeaponsSelection).FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary);
        var secondaryWeapon = Inventory.SecondaryWeaponsSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary);

        foreach (var weapon in Inventory.PrimaryWeaponsSelection.Concat(Inventory.SecondaryWeaponsSelection))
        {
            StoreWeaponData weaponData = weaponDatas.FirstOrDefault(x => x.WeaponData.Type == weapon.Type);
            var storeItem = CreateInventoryItem(weaponData);

            if (primaryWeapon != null && primaryWeapon.Type == weapon.Type)
                PrimarySlot.DropItem(storeItem);
            else if (secondaryWeapon != null && secondaryWeapon.Type == weapon.Type)
                SecondarySlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Carraga os itens arremess�veis do invent�rio.
    /// </summary>
    void LoadThrowables()
    {
        var throwablesDatas = Resources.LoadAll<StoreThrowableData>("ScriptableObjects/Store/Throwables");
        var equippedThrowable = Inventory.ThrowableItemsSelection.FirstOrDefault(x => x.IsEquipped);

        foreach (var throwable in Inventory.ThrowableItemsSelection)
        {
            StoreThrowableData throwableData = throwablesDatas.FirstOrDefault(x => x.ThrowableData.Type == throwable.Type);
            var storeItem = CreateInventoryItem(throwableData);

            if (equippedThrowable != null && equippedThrowable.Type == throwable.Type)
                GrenadeSlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Carrega as habilidades t�ticas do invent�rio.
    /// </summary>
    void LoadTacticalAbilities()
    {
        var abilitiesDatas = Resources.LoadAll<StoreTacticalAbilityData>("ScriptableObjects/Store/TacticalAbilities");
        var equippedAbility = Inventory.TacticalAbilitiesSelection.FirstOrDefault(x => x.IsEquipped);

        foreach (var ability in Inventory.TacticalAbilitiesSelection)
        {
            StoreTacticalAbilityData abilityData = abilitiesDatas.FirstOrDefault(x => x.AbilityType == ability.Type);
            var storeItem = CreateInventoryItem(abilityData);

            if (equippedAbility != null && equippedAbility.Type == ability.Type)
                AbilitySlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Seleciona um item para a visualiza��o no preview.
    /// </summary>
    /// <param name="item">O item a ser selecionado.</param>
    public void SelectItem(StoreItem item)
    {
        PreviewTitleText.text = item.Data.Title;

        BtnSell.interactable = item.Data.IsSellable;
        if (item.Data.IsSellable)
            PreviewBtnSellText.text = $"Sell for $ {(item.Data.Price - item.Data.Discount) / 2}";
        else
            PreviewBtnSellText.text = "Can't sell";

        if (item.Data is StoreWeaponData storeWeaponData)
        {
            PreviewBtnSellText.text = $"Sell for $ {GetWeaponSellPrice(storeWeaponData)}";

            int pelletsCount;
            float pelletsDispersion;
            if (storeWeaponData.WeaponData is ShotgunData shotgunData)
            {
                pelletsCount = shotgunData.ShellPelletsCount;
                pelletsDispersion = shotgunData.PelletsDispersion;
            }
            else
            {
                pelletsCount = 0;
                pelletsDispersion = 0;
            }

            SetIconStats(
                headshot: storeWeaponData.WeaponData.HeadshotMultiplier.ToString("N1"),
                magazine: storeWeaponData.WeaponData.MagazineSize.ToString(),
                pellets: pelletsCount > 0 ? pelletsCount.ToString() : null,
                dispersion: pelletsDispersion > 0 ? pelletsDispersion.ToString() : null,
                bulletType: storeWeaponData.WeaponData.BulletType
            );

            SetBarStats(
                damage: storeWeaponData.WeaponData,
                fireRate: storeWeaponData.WeaponData,
                reloadSpeed: storeWeaponData.WeaponData,
                range: storeWeaponData.WeaponData,
                bulletSpeed: storeWeaponData.WeaponData
            );
        }
        else if (item.Data is StoreThrowableData storeThrowableData)
        {
            SetIconStats(
                headshot: storeThrowableData.ThrowableData.HeadshotMultiplier.ToString("N1")
            );

            SetBarStats();
        }
        else
        {
            SetIconStats();

            SetBarStats();
        }
    }

    /// <summary>
    /// Prepara os textos e ativa os gameobjects de cada estat�stica. Caso n�o haja estat�sticas, desativa os gameobjects.
    /// </summary>
    /// <param name="magazine">O texto de capacidade do carregador. Null para desativar.</param>
    /// <param name="headshot">O texto de multiplicador de dano na cabe�a. Null para desativar.</param>
    /// <param name="pellets">O texto do n�mero de balins. Null para desativar.</param>
    /// <param name="dispersion">O texto da dispers�o dos balins. Null para desativar.</param>
    /// <param name="bulletType">O tipo de muni��o para definir no �cone de capacidade do carregador.</param>
    private void SetIconStats(string? magazine = null, string? headshot = null, string? pellets = null, string? dispersion = null, BulletTypes? bulletType = null)
    {
        PreviewMagazineBulletsText.text = magazine ?? "";
        PreviewHeadshotMultiplierText.text = headshot ?? "";
        PreviewPelletsCountText.text = pellets ?? "";
        PreviewDispersionText.text = dispersion ?? "";

        PreviewMagazineBulletsText.transform.parent.gameObject.SetActive(magazine != null);
        PreviewHeadshotMultiplierText.transform.parent.gameObject.SetActive(headshot != null);
        PreviewPelletsCountText.transform.parent.gameObject.SetActive(pellets != null);
        PreviewDispersionText.transform.parent.gameObject.SetActive(dispersion != null);

        PreviewBulletIcon.sprite = bulletType switch
        {
            BulletTypes.Pistol => storeScreen.PistolBulletIcon,
            BulletTypes.Shotgun => storeScreen.ShotgunBulletIcon,
            BulletTypes.AssaultRifle => storeScreen.RifleAmmoIcon,
            BulletTypes.Sniper => storeScreen.SniperAmmoIcon,
            BulletTypes.Rocket => storeScreen.RocketAmmoIcon,
            BulletTypes.Melee => storeScreen.MeleeAmmoIcon,
            _ => null,
        };

        PreviewMagazineBulletsText.transform.parent.parent.gameObject.SetActive(magazine != null || headshot != null || pellets != null || dispersion != null);
    }

    /// <summary>
    /// Preenche as barras de estat�sticas da arma. Caso n�o haja nenhuma, desativa os gameobjects.
    /// </summary>
    /// <param name="damage">As informa��es da arma para calcular o dano. Null para desativar.</param>
    /// <param name="fireRate">As informa��es da arma para calcular a cad�ncia. Null para desativar.</param>
    /// <param name="reloadSpeed">As informa��es da arma para calcular a velocidade de recarga. Null para desativar.</param>
    /// <param name="range">As informa��es da arma para calcular o alcance. Null para desativar.</param>
    private void SetBarStats(BaseWeaponData? damage = null, BaseWeaponData? fireRate = null, BaseWeaponData? reloadSpeed = null, BaseWeaponData? range = null, BaseWeaponData? bulletSpeed = null)
    {
        if (damage != null)
        {
            DamageBar.MaxValue = Constants.MaxWeaponDamage;
            DamageBar.Value = Constants.CalculateDamage(damage);
            DamageBar.CalculateSections();
        }

        if (fireRate != null)
        {
            FireRateBar.MaxValue = Constants.MaxWeaponFireRate;
            FireRateBar.Value = Constants.CalculateFireRate(fireRate);
            FireRateBar.CalculateSections();
        }

        if (reloadSpeed != null)
        {
            ReloadSpeedBar.MaxValue = Constants.MaxWeaponReloadSpeed;
            ReloadSpeedBar.Value = Constants.CalculateReloadSpeed(reloadSpeed);
            ReloadSpeedBar.CalculateSections();
        }

        if (range != null)
        {
            RangeBar.MaxValue = Constants.MaxWeaponRange;
            RangeBar.Value = Constants.CalculateRange(range);
            RangeBar.CalculateSections();
        }

        if (bulletSpeed != null)
        {
            BulletSpeedBar.MaxValue = Constants.MaxWeaponBulletSpeed;
            BulletSpeedBar.Value = Constants.CalculateBulletSpeed(range);
            BulletSpeedBar.CalculateSections();
        }

        DamageBar.transform.parent.parent.gameObject.SetActive(damage != null || fireRate != null || reloadSpeed != null || range != null);
    }

    /// <summary>
    /// Fun��o chamada quando o bot�o de upgrade do atributo es�cificado � destacado com o mouse.
    /// </summary>
    /// <param name="attribute">O atributo do upgrade.</param>
    /// <param name="button">O bot�o destacado.</param>
    /// <param name="hovered">Se est� sendo destacado ou desfocado.</param>
    public void OnHoverUpgrade(WeaponAttributes attribute, BaseButton button, bool hovered)
    {
        if (storeScreen.SelectedItem.Data is StoreWeaponData storeWeapon)
        {
            if (storeWeapon.WeaponData.Upgrades.IsNullOrEmpty())
            {
                button.gameObject.SetActive(false);
                button.transform.parent.Find("UpgradePrice").gameObject.SetActive(false);
                return;
            }

            var upgradeItem = GetWeaponUpgradeStep(storeWeapon.WeaponData, attribute);
            if (upgradeItem == null)
                return;

            if (!storeWeapon.WeaponData.Upgrades.Any(x => x.Attribute == attribute))
                return;

            void UpdateBar(SectionedBar bar)
            {
                if (hovered)
                {
                    bar.ModificationValue = upgradeItem.Value;
                    bar.BlinkModification = true;
                }
                else
                    bar.ModificationValue = 0;
                bar.CalculateSections();
            }

            void UpdateIconStat(TextMeshProUGUI statText, string newValue, string defaultValue)
            {
                if (hovered)
                {
                    statText.text = newValue;
                    statText.GetComponent<BlinkingText>().enabled = true;
                }
                else
                {
                    statText.text = defaultValue;
                    statText.GetComponent<BlinkingText>().enabled = false;
                    statText.color = Color.white;
                }
            }

            switch (attribute)
            {
                case WeaponAttributes.Damage:
                    UpdateBar(DamageBar);
                    break;

                case WeaponAttributes.FireRate:
                    UpdateBar(FireRateBar);
                    break;

                case WeaponAttributes.ReloadSpeed:
                    UpdateBar(ReloadSpeedBar);
                    break;

                case WeaponAttributes.Range:
                    UpdateBar(RangeBar);
                    break;

                case WeaponAttributes.BulletSpeed:
                    UpdateBar(BulletSpeedBar);
                    break;

                case WeaponAttributes.HeadshotMultiplier:
                    UpdateIconStat(PreviewHeadshotMultiplierText,
                        (upgradeItem.Value + storeWeapon.WeaponData.HeadshotMultiplier).ToString("N1"),
                        storeWeapon.WeaponData.HeadshotMultiplier.ToString("N1"));
                    break;

                case WeaponAttributes.MagazineSize:
                    UpdateIconStat(PreviewMagazineBulletsText,
                        (upgradeItem.Value + storeWeapon.WeaponData.MagazineBullets).ToString(),
                        storeWeapon.WeaponData.MagazineBullets.ToString());
                    break;

                case WeaponAttributes.BallinsConcentration:
                    UpdateIconStat(PreviewDispersionText,
                        (upgradeItem.Value + (storeWeapon.WeaponData as ShotgunData).PelletsDispersion).ToString(),
                        (storeWeapon.WeaponData as ShotgunData).PelletsDispersion.ToString());
                    break;
            }
        }
    }

    /// <summary>
    /// Fun��o chamada quando o bot�o de upgrade do atributo es�cificado � clicado.
    /// </summary>
    /// <param name="attributeValue">O atributo do upgrade;</param>
    public void BuyWeaponUpgrade(int attributeValue)
    {
        WeaponAttributes attribute = (WeaponAttributes)attributeValue;

        if (storeScreen.SelectedItem.Data is StoreWeaponData storeWeapon)
        {
            var upgradeMap = GetWeaponUpgradeMap(storeWeapon.WeaponData, attribute);
            if (upgradeMap == null)
                return;

            var upgradeItem = GetWeaponUpgradeStep(storeWeapon.WeaponData, attribute);

            var nextUpgradeItem = GetWeaponUpgradeStep(storeWeapon.WeaponData, attribute, upgradeMap.UpgradeStep + 1);

            if (upgradeItem == null)
                return;

            if (!storeWeapon.WeaponData.Upgrades.Any(x => x.Attribute == attribute))
                return;

            if (upgradeItem.Price > storeScreen.PlayerData.Money)
                return;

            void UpdateBar(SectionedBar bar)
            {
                bar.AddValue(upgradeItem.Value);
                if (nextUpgradeItem != null)
                {
                    if (nextUpgradeItem.Price > storeScreen.PlayerData.Money - upgradeItem.Price)
                        bar.ModificationValue = 0;
                    else
                        bar.ModificationValue = nextUpgradeItem.Value;
                }
                else
                    bar.ModificationValue = 0;
                bar.CalculateSections();
            }

            void UpdateIconStat(TextMeshProUGUI statText, string newValue)
            {
                statText.text = newValue;
                statText.GetComponent<BlinkingText>().enabled = false;
                statText.color = Color.white;
            }

            switch (attribute)
            {
                case WeaponAttributes.Damage:
                    storeWeapon.WeaponData.Damage += storeWeapon.WeaponData.BulletType switch
                    {
                        BulletTypes.Shotgun => upgradeItem.Value * (storeWeapon.WeaponData as ShotgunData).ShellPelletsCount,
                        _ => upgradeItem.Value
                    };
                    UpdateBar(DamageBar);
                    break;

                case WeaponAttributes.FireRate:
                    storeWeapon.WeaponData.FireRate += upgradeItem.Value;
                    UpdateBar(FireRateBar);
                    break;

                case WeaponAttributes.ReloadSpeed:
                    storeWeapon.WeaponData.ReloadTimeMs = storeWeapon.WeaponData.ReloadType switch
                    {
                        ReloadTypes.SingleBullet => 5000 / ((ReloadSpeedBar.Value + upgradeItem.Value) * storeWeapon.WeaponData.MagazineSize),
                        _ => 5000 / (ReloadSpeedBar.Value + upgradeItem.Value)
                    };
                    UpdateBar(ReloadSpeedBar);
                    break;

                case WeaponAttributes.Range:
                    var step = RangeBar.MaxValue / RangeBar.SectionsCount;
                    storeWeapon.WeaponData.MinDamageRange += step * upgradeItem.Value / 3;
                    storeWeapon.WeaponData.MaxDamageRange += step * upgradeItem.Value / 3;
                    storeWeapon.WeaponData.BulletMaxRange += step * upgradeItem.Value / 3;
                    UpdateBar(RangeBar);
                    break;

                case WeaponAttributes.BulletSpeed:
                    storeWeapon.WeaponData.BulletSpeed += upgradeItem.Value;
                    UpdateBar(BulletSpeedBar);
                    break;

                case WeaponAttributes.HeadshotMultiplier:
                    storeWeapon.WeaponData.HeadshotMultiplier += upgradeItem.Value;
                    UpdateIconStat(PreviewHeadshotMultiplierText, storeWeapon.WeaponData.HeadshotMultiplier.ToString("N1"));
                    break;

                case WeaponAttributes.MagazineSize:
                    storeWeapon.WeaponData.MagazineBullets += (int)upgradeItem.Value;
                    UpdateIconStat(PreviewMagazineBulletsText, storeWeapon.WeaponData.MagazineBullets.ToString());
                    break;

                case WeaponAttributes.BallinsConcentration:
                    var data = storeWeapon.WeaponData as ShotgunData;
                    data.PelletsDispersion = 90 / (data.PelletsDispersion + upgradeItem.Value);
                    UpdateIconStat(PreviewDispersionText, data.PelletsDispersion.ToString());
                    break;
            }

            storeScreen.PlayerData.TakeMoney(upgradeItem.Price);
            storeScreen.audioSource.PlayOneShot(storeScreen.PurchaseSound.Audio, storeScreen.PurchaseSound.Volume);
            storeScreen.ShowPopup($"-{upgradeItem.Price:N2}", Color.red, storeScreen.PlayerMoneyText.transform.position);

            upgradeMap.UpgradeStep++;

            PreviewBtnSellText.text = $"Sell for $ {GetWeaponSellPrice(storeWeapon)}";
        }
    }

    private InventoryData.WeaponSelection GetWeaponSelection(BaseWeaponData weaponData)
    {
        var weaponsSelection = storeScreen.PlayerData.InventoryData.PrimaryWeaponsSelection.Concat(storeScreen.PlayerData.InventoryData.SecondaryWeaponsSelection);
        var weaponSelection = weaponsSelection.FirstOrDefault(x => x.Type == weaponData.Type);

        if (weaponSelection == null)
            return null;

        return weaponSelection;
    }

    private InventoryData.WeaponSelection.WeaponUpgradeMap GetWeaponUpgradeMap(BaseWeaponData weaponData, WeaponAttributes attribute)
    {
        var weaponSelection = GetWeaponSelection(weaponData);

        if (weaponSelection == null)
            return null;

        var upgradeMap = weaponSelection.UpgradesMap.FirstOrDefault(x => x.Attribute == attribute);
        if (upgradeMap == null)
            return null;

        return upgradeMap;
    }

    private BaseWeaponData.WeaponUpgradeGroup.WeaponUpgrade GetWeaponUpgradeStep(BaseWeaponData weaponData, WeaponAttributes attribute, int? index = null)
    {
        var upgradeMap = GetWeaponUpgradeMap(weaponData, attribute);
        if (upgradeMap == null)
            return null;

        var upgradeItem = weaponData.Upgrades.FirstOrDefault(x => x.Attribute == attribute).UpgradeSteps.ElementAtOrDefault(index ?? upgradeMap.UpgradeStep);
        if (upgradeItem == null)
            return null;

        return upgradeItem;
    }

    private float GetWeaponSellPrice(StoreWeaponData storeWeapon)
    {
        if (storeWeapon == null || storeWeapon.WeaponData == null)
            return 0;

        if (!storeWeapon.IsSellable)
            return 0;

        float value = storeWeapon.Price - storeWeapon.Discount +
            storeWeapon.WeaponData.Upgrades
                .SelectMany((upgrade, upgradeIndex) => upgrade.UpgradeSteps.Select((step, stepIndex) => new { Price = step.Price, StepIndex = stepIndex, UpgradeIndex = upgradeIndex }))
                .Where(steps => steps.StepIndex <= GetWeaponUpgradeMap(storeWeapon.WeaponData, storeWeapon.WeaponData.Upgrades[steps.UpgradeIndex].Attribute).UpgradeStep - 1)
                .Sum(step => step.Price);

        return value / 2;
    }

    /// <summary>
    /// Fun��o chamada quando o player clica no bot�o de vender.
    /// </summary>
    public void SellItem()
    {
        var item = storeScreen.SelectedItem;

        if (item == null || item.Data == null)
            return;

        if (!item.Data.IsSellable)
            return;

        bool sold = false;
        float value = 0;

        if (item.Data is StoreWeaponData storeWeapon)
        {
            value = GetWeaponSellPrice(storeWeapon);
            sold = SellWeapon(storeWeapon);
        }

        if (item.Data is StoreThrowableData storeThrowable)
            sold = SellThrowable(storeThrowable);

        if (!sold)
            return;

        if (value == 0)
            value = (item.Data.Price - item.Data.Discount) / 2;

        storeScreen.PlayerData.GetMoney(value);
        storeScreen.audioSource.PlayOneShot(storeScreen.PurchaseSound.Audio, storeScreen.PurchaseSound.Volume);
        storeScreen.ShowPopup($"+{value:N2}", Color.green, storeScreen.PlayerMoneyText.transform.position);
    }

    /// <summary>
    /// Verifica se o player possui a arma e a remove.
    /// </summary>
    /// <param name="data">Os dados da arma a ser vendida.</param>
    /// <returns>Se a arma foi encontrada e removida.</returns>
    private bool SellWeapon(StoreWeaponData data)
    {
        var weapon = Inventory.PrimaryWeaponsSelection
                 .Concat(Inventory.SecondaryWeaponsSelection)
                 .FirstOrDefault(x => x.Type == data.WeaponData.Type);

        if (weapon == null)
            return false;

        if (weapon.EquippedSlot == WeaponEquippedSlot.Primary)
            PrimarySlot.ClearSlot();
        else if (weapon.EquippedSlot == WeaponEquippedSlot.Secondary)
            SecondarySlot.ClearSlot();

        Transform inventoryWeapon = weaponsColumn.Find(data.WeaponData.Type.ToString());
        if (inventoryWeapon != null)
            Destroy(inventoryWeapon.gameObject);

        Inventory.PrimaryWeaponsSelection = Inventory.PrimaryWeaponsSelection.Where(x => x.Type != data.WeaponData.Type).ToList();
        Inventory.SecondaryWeaponsSelection = Inventory.SecondaryWeaponsSelection.Where(x => x.Type != data.WeaponData.Type).ToList();

        string SoPath = $"ScriptableObjects/Weapons/{data.WeaponData.WeaponClass}/{data.WeaponData.Type}";
        data.WeaponData.UnloadSO();
        var reloadedWeaponData = Resources.Load<BaseWeaponData>(SoPath);

        data.WeaponData = reloadedWeaponData;
        (storeScreen.SelectedItem.Data as StoreWeaponData).WeaponData = reloadedWeaponData;

        storeScreen.SelectedItem = null;

        return true;
    }

    /// <summary>
    /// Verifica se o player possui o arremess�vel e o remove.
    /// </summary>
    /// <param name="data">Os dados do arremess�vel a ser vendido.</param>
    /// <returns>Se o arremess�vel foi encontrado e removido.</returns>
    private bool SellThrowable(StoreThrowableData data)
    {
        var throwable = Inventory.ThrowableItemsSelection
                .FirstOrDefault(x => x.Type == data.ThrowableData.Type);

        if (throwable == null)
            return false;

        if (throwable.Count > 1)
            throwable.Count--;
        else
        {
            if (throwable.IsEquipped)
                GrenadeSlot.ClearSlot();

            Transform inventoryThrowable = throwablesColumn.Find(data.ThrowableData.Type.ToString());
            if (inventoryThrowable != null)
                Destroy(inventoryThrowable.gameObject);

            Inventory.ThrowableItemsSelection = Inventory.ThrowableItemsSelection.Where(x => x.Type != data.ThrowableData.Type).ToList();

            storeScreen.SelectedItem = null;
        }

        return true;
    }
}
