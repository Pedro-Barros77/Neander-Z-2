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
    SectionedBar DamageBar, FireRateBar, ReloadSpeedBar, RangeBar, BulletSpeedBar,
        PlayerMaxHealthBar, PlayerMovementSpeedBar, PlayerSprintSpeedBar, PlayerJumpForceBar, PlayerMaxStaminaBar, PlayerStaminaRegenBar, PlayerStaminaHasteBar, PlayerJumpStaminaBar, PlayerSprintStaminaBar, PlayerAttackStaminaBar;
    [SerializeField]
    GameObject BackpackContent, PlayerContent;
    [SerializeField]
    BaseButton BtnUpgradeDamage, BtnUpgradeFireRate, BtnUpgradeReloadSpeed, BtnUpgradeRange, BtnUpgradeBulletSpeed, BtnUpgradeHeadshotMultiplier, BtnUpgradeMagazineCapacity, BtnUpgradeBallinsDispersion, BackpackBtnUpgrade,
        BtnUpgradePlayerMaxHealth, BtnUpgradePlayerMovementSpeed, BtnUpgradePlayerSprintSpeed, BtnUpgradePlayerJumpForce, BtnUpgradePlayerMaxStamina, BtnUpgradePlayerStaminaRegen, BtnUpgradePlayerStaminaHaste, BtnUpgradePlayerJumpStamina, BtnUpgradePlayerSprintStamina, BtnUpgradePlayerAttackStamina;
    [SerializeField]
    TextMeshProUGUI BackpackPistolAmmo, BackpackShotgunAmmo, BackpackRifleAmmo, BackpackSniperAmmo, BackpackRocketAmmo;

    TextMeshProUGUI BackpackPistolAmmoUpgrade, BackpackShotgunAmmoUpgrade, BackpackRifleAmmoUpgrade, BackpackSniperAmmoUpgrade, BackpackRocketAmmoUpgrade, BackpackBtnUpgradeText;
    TextMeshProUGUI UpgradeDamagePrice, UpgradeFireRatePrice, UpgradeReloadSpeedPrice, UpgradeRangePrice, UpgradeBulletSpeedPrice, UpgradeHeadshotMultiplierPrice, UpgradeMagazineCapacityPrice, UpgradeBallinsDispersionPrice,
        UpgradePlayerMaxHealthPrice, UpgradePlayerMovementSpeedPrice, UpgradePlayerSprintSpeedPrice, UpgradePlayerJumpForcePrice, UpgradePlayerMaxStaminaPrice, UpgradePlayerStaminaRegenPrice, UpgradePlayerStaminaHastePrice, UpgradePlayerJumpStaminaPrice, UpgradePlayerSprintStaminaPrice, UpgradePlayerAttackStaminaPrice;
    GameObject IconStatsContainer, StatsContainer, ButtonsContainer;

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

        BtnUpgradeDamage.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.Damage, button, hovered);
        BtnUpgradeFireRate.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.FireRate, button, hovered);
        BtnUpgradeReloadSpeed.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.ReloadSpeed, button, hovered);
        BtnUpgradeRange.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.Range, button, hovered);
        BtnUpgradeBulletSpeed.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.BulletSpeed, button, hovered);
        BtnUpgradeHeadshotMultiplier.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.HeadshotMultiplier, button, hovered);
        BtnUpgradeMagazineCapacity.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.MagazineSize, button, hovered);
        BtnUpgradeBallinsDispersion.HoverEvent += (BaseButton button, bool hovered) => OnHoverWeaponUpgrade(WeaponAttributes.Dirspersion, button, hovered);
        BackpackBtnUpgradeText = BackpackBtnUpgrade.GetComponentInChildren<TextMeshProUGUI>();
        BackpackBtnUpgrade.HoverEvent += (BaseButton button, bool hovered) =>
        {
            BackpackPistolAmmoUpgrade.GetComponent<BlinkingText>().enabled = hovered;
            BackpackShotgunAmmoUpgrade.GetComponent<BlinkingText>().enabled = hovered;
            BackpackRifleAmmoUpgrade.GetComponent<BlinkingText>().enabled = hovered;
            BackpackSniperAmmoUpgrade.GetComponent<BlinkingText>().enabled = hovered;
            BackpackRocketAmmoUpgrade.GetComponent<BlinkingText>().enabled = hovered;
            if (!hovered)
            {
                BackpackPistolAmmoUpgrade.color = new Color(0, 0, 0, 0);
                BackpackShotgunAmmoUpgrade.color = new Color(0, 0, 0, 0);
                BackpackRifleAmmoUpgrade.color = new Color(0, 0, 0, 0);
                BackpackSniperAmmoUpgrade.color = new Color(0, 0, 0, 0);
                BackpackRocketAmmoUpgrade.color = new Color(0, 0, 0, 0);
            }
        };
        BtnUpgradePlayerMaxHealth.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.MaxHealth, button, hovered);
        BtnUpgradePlayerMovementSpeed.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.MovementSpeed, button, hovered);
        BtnUpgradePlayerSprintSpeed.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.SprintSpeed, button, hovered);
        BtnUpgradePlayerJumpForce.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.JumpForce, button, hovered);
        BtnUpgradePlayerMaxStamina.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.MaxStamina, button, hovered);
        BtnUpgradePlayerStaminaRegen.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.StaminaRegen, button, hovered);
        BtnUpgradePlayerStaminaHaste.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.StaminaHaste, button, hovered);
        BtnUpgradePlayerJumpStamina.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.JumpStamina, button, hovered);
        BtnUpgradePlayerSprintStamina.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.SprintStamina, button, hovered);
        BtnUpgradePlayerAttackStamina.HoverEvent += (BaseButton button, bool hovered) => OnHoverPlayerUpgrade(PlayerAttributes.AttackStamina, button, hovered);

        UpgradeDamagePrice = BtnUpgradeDamage.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeFireRatePrice = BtnUpgradeFireRate.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeReloadSpeedPrice = BtnUpgradeReloadSpeed.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeRangePrice = BtnUpgradeRange.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeBulletSpeedPrice = BtnUpgradeBulletSpeed.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeHeadshotMultiplierPrice = BtnUpgradeHeadshotMultiplier.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeMagazineCapacityPrice = BtnUpgradeMagazineCapacity.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradeBallinsDispersionPrice = BtnUpgradeBallinsDispersion.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();

        UpgradePlayerMaxHealthPrice = BtnUpgradePlayerMaxHealth.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerMovementSpeedPrice = BtnUpgradePlayerMovementSpeed.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerSprintSpeedPrice = BtnUpgradePlayerSprintSpeed.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerJumpForcePrice = BtnUpgradePlayerJumpForce.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerMaxStaminaPrice = BtnUpgradePlayerMaxStamina.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerStaminaRegenPrice = BtnUpgradePlayerStaminaRegen.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerStaminaHastePrice = BtnUpgradePlayerStaminaHaste.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerJumpStaminaPrice = BtnUpgradePlayerJumpStamina.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerSprintStaminaPrice = BtnUpgradePlayerSprintStamina.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();
        UpgradePlayerAttackStaminaPrice = BtnUpgradePlayerAttackStamina.transform.parent.Find("UpgradePrice").GetComponent<TextMeshProUGUI>();

        BackpackPistolAmmoUpgrade = BackpackPistolAmmo.transform.parent.Find("UpgradeValue").GetComponent<TextMeshProUGUI>();
        BackpackShotgunAmmoUpgrade = BackpackShotgunAmmo.transform.parent.Find("UpgradeValue").GetComponent<TextMeshProUGUI>();
        BackpackRifleAmmoUpgrade = BackpackRifleAmmo.transform.parent.Find("UpgradeValue").GetComponent<TextMeshProUGUI>();
        BackpackSniperAmmoUpgrade = BackpackSniperAmmo.transform.parent.Find("UpgradeValue").GetComponent<TextMeshProUGUI>();
        BackpackRocketAmmoUpgrade = BackpackRocketAmmo.transform.parent.Find("UpgradeValue").GetComponent<TextMeshProUGUI>();

        IconStatsContainer = PreviewMagazineBulletsText.transform.parent.parent.gameObject;
        StatsContainer = DamageBar.transform.parent.parent.gameObject;
        ButtonsContainer = BtnSell.transform.parent.gameObject;
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
                        priceText.color = Color.white;
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
                SetUpgrade(WeaponAttributes.Dirspersion, BtnUpgradeBallinsDispersion, UpgradeBallinsDispersionPrice);
            }
            else if (storeScreen.hasItem && storeScreen.SelectedItem.Data is StoreBackpackData storeBackpack)
            {
                var backpackUpgrade = storeBackpack.AmmoUpgrades.ElementAtOrDefault(Inventory.UpgradeIndex);

                BackpackPistolAmmo.text = Inventory.MaxPistolAmmo.ToString();
                BackpackShotgunAmmo.text = Inventory.MaxShotgunAmmo.ToString();
                BackpackRifleAmmo.text = Inventory.MaxRifleAmmo.ToString();
                BackpackSniperAmmo.text = Inventory.MaxSniperAmmo.ToString();
                BackpackRocketAmmo.text = Inventory.MaxRocketAmmo.ToString();

                if (backpackUpgrade != null)
                {
                    BackpackBtnUpgrade.button.interactable = storeScreen.PlayerData.Money >= backpackUpgrade.Price;

                    BackpackBtnUpgradeText.text = $"Upgrade for ${backpackUpgrade.Price:N2}";
                    BackpackPistolAmmoUpgrade.text = $"+{backpackUpgrade.PistolAmmo}";
                    BackpackShotgunAmmoUpgrade.text = $"+{backpackUpgrade.ShotgunAmmo}";
                    BackpackRifleAmmoUpgrade.text = $"+{backpackUpgrade.RifleAmmo}";
                    BackpackSniperAmmoUpgrade.text = $"+{backpackUpgrade.SniperAmmo}";
                    BackpackRocketAmmoUpgrade.text = $"+{backpackUpgrade.RocketAmmo}";
                }
                else
                {
                    BackpackBtnUpgradeText.text = "MAX";
                }

            }
            else if (storeScreen.hasItem && storeScreen.SelectedItem.Data is StorePlayerData storePlayer)
            {
                void SetUpgrade(PlayerAttributes attribute, BaseButton button, TextMeshProUGUI priceText)
                {
                    var upgrades = storePlayer.GetPlayerUpgrades(attribute);
                    var upgradeStep = upgrades.ElementAtOrDefault(storeScreen.PlayerData.GetUpgradeIndex(attribute));

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
                        priceText.color = Color.white;
                        button.button.interactable = true;
                    }

                    priceText.text = $"$ {upgradeStep.Price:N2}";
                }

                SetUpgrade(PlayerAttributes.MaxHealth, BtnUpgradePlayerMaxHealth, UpgradePlayerMaxHealthPrice);
                SetUpgrade(PlayerAttributes.MovementSpeed, BtnUpgradePlayerMovementSpeed, UpgradePlayerMovementSpeedPrice);
                SetUpgrade(PlayerAttributes.SprintSpeed, BtnUpgradePlayerSprintSpeed, UpgradePlayerSprintSpeedPrice);
                SetUpgrade(PlayerAttributes.JumpForce, BtnUpgradePlayerJumpForce, UpgradePlayerJumpForcePrice);
                SetUpgrade(PlayerAttributes.MaxStamina, BtnUpgradePlayerMaxStamina, UpgradePlayerMaxStaminaPrice);
                SetUpgrade(PlayerAttributes.StaminaRegen, BtnUpgradePlayerStaminaRegen, UpgradePlayerStaminaRegenPrice);
                SetUpgrade(PlayerAttributes.StaminaHaste, BtnUpgradePlayerStaminaHaste, UpgradePlayerStaminaHastePrice);
                SetUpgrade(PlayerAttributes.JumpStamina, BtnUpgradePlayerJumpStamina, UpgradePlayerJumpStaminaPrice);
                SetUpgrade(PlayerAttributes.SprintStamina, BtnUpgradePlayerSprintStamina, UpgradePlayerSprintStaminaPrice);
                SetUpgrade(PlayerAttributes.AttackStamina, BtnUpgradePlayerAttackStamina, UpgradePlayerAttackStaminaPrice);
            }
        }

        BtnSwitchWeapons.interactable = PrimarySlot.Data == null || (PrimarySlot.Data is StoreWeaponData primSlotWeapon && (!primSlotWeapon.WeaponData.IsPrimary || Inventory.PassiveSkillsSelection.Any(x => x.Type == PassiveSkillTypes.DualFirepower && x.IsEquipped)));

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
    /// Troca as armas entre os slots primário e secundário.
    /// </summary>
    public void SwitchWeapons()
    {
        if (PrimarySlot.Data is StoreWeaponData primSlotWeapon && (primSlotWeapon.WeaponData.IsPrimary && !Inventory.PassiveSkillsSelection.Any(x => x.Type == PassiveSkillTypes.DualFirepower && x.IsEquipped)))
            return;

        StoreItem primary = PrimarySlot.Item, secondary = SecondarySlot.Item;

        PrimarySlot.DropItem(secondary);
        SecondarySlot.DropItem(primary);

        storeScreen.IsSaveDirty = true;
    }

    /// <summary>
    /// Cria um item de inventário.
    /// </summary>
    /// <param name="data">Os dados deste item.</param>
    /// <param name="updateUiLayout">Se o layout deve ser atualizado após a adição.</param>
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
        else if (data is StorePassiveSkillData skillData)
        {
            prefabItem.Data = skillData;
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
    /// Função chamada por cada slot sempre que seu conteúdo é alterado.
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
    /// Carrega todos os itens do inventário.
    /// </summary>
    void LoadAll()
    {
        ClearInventoryItems();
        LoadWeapons();
        LoadThrowables();
        LoadTacticalAbilities();
        LoadPassiveSkills();
        loaded = true;
    }

    /// <summary>
    /// Limpa todos os items do grid do inventário.
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
    /// Carrega as armas do inventário.
    /// </summary>
    void LoadWeapons()
    {
        var weaponDatas = Resources.LoadAll<StoreWeaponData>("ScriptableObjects/Store/Weapons");
        var primaryWeapon = Inventory.WeaponsSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary);
        var secondaryWeapon = Inventory.WeaponsSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary);

        foreach (var weaponType in Inventory.WeaponsSelection.Select(x => x.Type))
        {
            StoreWeaponData weaponData = weaponDatas.FirstOrDefault(x => x.WeaponData.Type == weaponType);
            var storeItem = CreateInventoryItem(weaponData);

            if (primaryWeapon != null && primaryWeapon.Type == weaponType)
                PrimarySlot.DropItem(storeItem);
            else if (secondaryWeapon != null && secondaryWeapon.Type == weaponType)
                SecondarySlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Carraga os itens arremessáveis do inventário.
    /// </summary>
    void LoadThrowables()
    {
        var throwablesDatas = Resources.LoadAll<StoreThrowableData>("ScriptableObjects/Store/Throwables");
        var equippedThrowable = Inventory.ThrowableItemsSelection.FirstOrDefault(x => x.IsEquipped);

        foreach (var throwableType in Inventory.ThrowableItemsSelection.Select(x => x.Type))
        {
            StoreThrowableData throwableData = throwablesDatas.FirstOrDefault(x => x.ThrowableData.Type == throwableType);
            var storeItem = CreateInventoryItem(throwableData);

            if (equippedThrowable != null && equippedThrowable.Type == throwableType)
                GrenadeSlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Carrega as habilidades táticas do inventário.
    /// </summary>
    void LoadTacticalAbilities()
    {
        var abilitiesDatas = Resources.LoadAll<StoreTacticalAbilityData>("ScriptableObjects/Store/TacticalAbilities");
        var equippedAbility = Inventory.TacticalAbilitiesSelection.FirstOrDefault(x => x.IsEquipped);

        foreach (var abilityType in Inventory.TacticalAbilitiesSelection.Select(x => x.Type))
        {
            StoreTacticalAbilityData abilityData = abilitiesDatas.FirstOrDefault(x => x.AbilityType == abilityType);
            var storeItem = CreateInventoryItem(abilityData);

            if (equippedAbility != null && equippedAbility.Type == abilityType)
                AbilitySlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Carrega as habilidades passivas do inventário.
    /// </summary>
    void LoadPassiveSkills()
    {
        var skillsDatas = Resources.LoadAll<StorePassiveSkillData>("ScriptableObjects/Store/PassiveSkills");
        var equippedSkill = Inventory.PassiveSkillsSelection.FirstOrDefault(x => x.IsEquipped);

        foreach (var skillType in Inventory.PassiveSkillsSelection.Select(x => x.Type))
        {
            StorePassiveSkillData abilityData = skillsDatas.FirstOrDefault(x => x.SkillType == skillType);
            var storeItem = CreateInventoryItem(abilityData);

            if (equippedSkill != null && equippedSkill.Type == skillType)
                SkillSlot.DropItem(storeItem);
        }
    }

    /// <summary>
    /// Seleciona um item para a visualização no preview.
    /// </summary>
    /// <param name="item">O item a ser selecionado.</param>
    public void SelectItem(StoreItem item)
    {
        PreviewTitleText.text = item.Data.Title;
        ButtonsContainer.SetActive(true);
        BackpackContent.SetActive(false);
        PlayerContent.SetActive(false);

        BtnSell.interactable = item.Data.IsSellable;
        if (item.Data.IsSellable)
            PreviewBtnSellText.text = $"Sell for $ {(item.Data.Price - item.Data.Discount) / 2}";
        else
            PreviewBtnSellText.text = "Can't sell";

        if (item.Data is StoreWeaponData storeWeaponData)
        {
            PreviewBtnSellText.text = $"Sell for $ {GetWeaponSellPrice(storeWeaponData)}";

            string dispersionValue = "";
            int pelletsCount = 0;
            if (storeWeaponData.WeaponData is ShotgunData shotgunData)
            {
                pelletsCount = shotgunData.ShellPelletsCount;
                dispersionValue = shotgunData.PelletsDispersion.ToString();
            }
            else if (storeWeaponData.WeaponData is LauncherData launcherData)
                dispersionValue = ((launcherData.ExplosionMinDamageRadius + launcherData.ExplosionMaxDamageRadius) / 2).ToString("N1");
            else
                pelletsCount = 0;

            SetIconStats(
                headshot: storeWeaponData.WeaponData.HeadshotMultiplier.ToString("N1"),
                magazine: storeWeaponData.WeaponData.MagazineSize.ToString(),
                pellets: pelletsCount > 0 ? pelletsCount.ToString() : null,
                dispersion: dispersionValue.Length > 0 ? dispersionValue : null,
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
            switch (item.Data.name)
            {
                case "Backpack":
                    BackpackContent.SetActive(true);

                    PlayerContent.SetActive(false);
                    IconStatsContainer.SetActive(false);
                    StatsContainer.SetActive(false);
                    ButtonsContainer.SetActive(false);
                    break;

                case "Player":
                    PlayerContent.SetActive(true);

                    BackpackContent.SetActive(false);
                    IconStatsContainer.SetActive(false);
                    StatsContainer.SetActive(false);
                    ButtonsContainer.SetActive(false);

                    SetPlayerBarStats();
                    break;
            }

            SetIconStats();

            SetBarStats();
        }
    }

    /// <summary>
    /// Prepara os textos e ativa os gameobjects de cada estatística. Caso não haja estatísticas, desativa os gameobjects.
    /// </summary>
    /// <param name="magazine">O texto de capacidade do carregador. Null para desativar.</param>
    /// <param name="headshot">O texto de multiplicador de dano na cabeça. Null para desativar.</param>
    /// <param name="pellets">O texto do número de balins. Null para desativar.</param>
    /// <param name="dispersion">O texto da dispersão dos balins. Null para desativar.</param>
    /// <param name="bulletType">O tipo de munição para definir no ícone de capacidade do carregador.</param>
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

        if (storeScreen.SelectedItem.Data is StoreWeaponData storeWeaponData)
        {
            void setIconUpgrade(BaseButton button, WeaponAttributes attribute)
            {
                bool hasUpgrade = !storeWeaponData.WeaponData.Upgrades.IsNullOrEmpty() && storeWeaponData.WeaponData.Upgrades.Any(x => x.Attribute == attribute);
                button.gameObject.SetActive(hasUpgrade);
                button.transform.parent.Find("UpgradePrice").gameObject.SetActive(hasUpgrade);
            }

            setIconUpgrade(BtnUpgradeDamage, WeaponAttributes.Damage);
            setIconUpgrade(BtnUpgradeFireRate, WeaponAttributes.FireRate);
            setIconUpgrade(BtnUpgradeReloadSpeed, WeaponAttributes.ReloadSpeed);
            setIconUpgrade(BtnUpgradeRange, WeaponAttributes.Range);
            setIconUpgrade(BtnUpgradeBulletSpeed, WeaponAttributes.BulletSpeed);
            setIconUpgrade(BtnUpgradeMagazineCapacity, WeaponAttributes.MagazineSize);
            setIconUpgrade(BtnUpgradeHeadshotMultiplier, WeaponAttributes.HeadshotMultiplier);
            if (storeWeaponData.WeaponData is ShotgunData || storeWeaponData.WeaponData is LauncherData)
                setIconUpgrade(BtnUpgradeBallinsDispersion, WeaponAttributes.Dirspersion);
        }

        IconStatsContainer.SetActive(magazine != null || headshot != null || pellets != null || dispersion != null);
    }

    /// <summary>
    /// Preenche as barras de estatísticas da arma. Caso não haja nenhuma, desativa os gameobjects.
    /// </summary>
    /// <param name="damage">As informações da arma para calcular o dano. Null para desativar.</param>
    /// <param name="fireRate">As informações da arma para calcular a cadência. Null para desativar.</param>
    /// <param name="reloadSpeed">As informações da arma para calcular a velocidade de recarga. Null para desativar.</param>
    /// <param name="range">As informações da arma para calcular o alcance. Null para desativar.</param>
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

        StatsContainer.SetActive(damage != null || fireRate != null || reloadSpeed != null || range != null);
    }

    /// <summary>
    /// Preenche as barras de estatísticas do player.
    /// </summary>
    private void SetPlayerBarStats()
    {
        if (storeScreen.SelectedItem == null || storeScreen.SelectedItem.Data == null)
            return;

        if (storeScreen.PlayerData == null)
            return;

        PlayerMaxHealthBar.MaxValue = Constants.MaxPlayerMaxHealth;
        PlayerMaxHealthBar.Value = storeScreen.PlayerData.MaxHealth;
        PlayerMaxHealthBar.CalculateSections();

        PlayerMovementSpeedBar.MaxValue = Constants.MaxPlayerMovementSpeed;
        PlayerMovementSpeedBar.Value = storeScreen.PlayerData.MaxMovementSpeed;
        PlayerMovementSpeedBar.CalculateSections();

        PlayerSprintSpeedBar.MaxValue = Constants.MaxPlayerSprintSpeed;
        PlayerSprintSpeedBar.Value = storeScreen.PlayerData.SprintSpeedMultiplier;
        PlayerSprintSpeedBar.CalculateSections();

        PlayerJumpForceBar.MaxValue = Constants.MaxPlayerJumpForce;
        PlayerJumpForceBar.Value = storeScreen.PlayerData.JumpForce;
        PlayerJumpForceBar.CalculateSections();

        PlayerMaxStaminaBar.MaxValue = Constants.MaxPlayerMaxStamina;
        PlayerMaxStaminaBar.Value = storeScreen.PlayerData.MaxStamina;
        PlayerMaxStaminaBar.CalculateSections();

        PlayerStaminaRegenBar.MaxValue = Constants.MaxPlayerStaminaRegen;
        PlayerStaminaRegenBar.Value = storeScreen.PlayerData.StaminaRegenRate;
        PlayerStaminaRegenBar.CalculateSections();

        PlayerStaminaHasteBar.MaxValue = Constants.MaxPlayerStaminaHaste;
        PlayerStaminaHasteBar.Value = 5000 / storeScreen.PlayerData.StaminaRegenDelayMs;
        PlayerStaminaHasteBar.CalculateSections();

        PlayerJumpStaminaBar.MaxValue = Constants.MaxPlayerJumpStamina;
        PlayerJumpStaminaBar.Value = 100 / storeScreen.PlayerData.JumpStaminaDrain;
        PlayerJumpStaminaBar.CalculateSections();

        PlayerSprintStaminaBar.MaxValue = Constants.MaxPlayerSprintStamina;
        PlayerSprintStaminaBar.Value = 100 / storeScreen.PlayerData.SprintStaminaDrain;
        PlayerSprintStaminaBar.CalculateSections();

        PlayerAttackStaminaBar.MaxValue = Constants.MaxPlayerAttackStamina;
        PlayerAttackStaminaBar.Value = 100 / storeScreen.PlayerData.AttackStaminaDrain;
        PlayerAttackStaminaBar.CalculateSections();
    }

    /// <summary>
    /// Função chamada quando o botão de upgrade do atributo esécificado do player é destacado com o mouse.
    /// </summary>
    /// <param name="attribute">O atributo do upgrade do player.</param>
    /// <param name="button">O botão destacado.</param>
    /// <param name="hovered">Se está sendo destacado ou desfocado.</param>
    public void OnHoverPlayerUpgrade(PlayerAttributes attribute, BaseButton button, bool hovered)
    {
        if (storeScreen.SelectedItem.Data is StorePlayerData storePlayer)
        {
            var upgrades = storePlayer.GetPlayerUpgrades(attribute);

            if (upgrades.IsNullOrEmpty())
            {
                button.gameObject.SetActive(false);
                button.transform.parent.Find("UpgradePrice").gameObject.SetActive(false);
                return;
            }

            var upgradeItem = upgrades.ElementAtOrDefault(storeScreen.PlayerData.GetUpgradeIndex(attribute));

            if (upgradeItem == null)
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

            switch (attribute)
            {
                case PlayerAttributes.MaxHealth:
                    UpdateBar(PlayerMaxHealthBar);
                    break;

                case PlayerAttributes.MovementSpeed:
                    UpdateBar(PlayerMovementSpeedBar);
                    break;

                case PlayerAttributes.SprintSpeed:
                    UpdateBar(PlayerSprintSpeedBar);
                    break;

                case PlayerAttributes.JumpForce:
                    UpdateBar(PlayerJumpForceBar);
                    break;

                case PlayerAttributes.MaxStamina:
                    UpdateBar(PlayerMaxStaminaBar);
                    break;

                case PlayerAttributes.StaminaRegen:
                    UpdateBar(PlayerStaminaRegenBar);
                    break;

                case PlayerAttributes.StaminaHaste:
                    UpdateBar(PlayerStaminaHasteBar);
                    break;

                case PlayerAttributes.JumpStamina:
                    UpdateBar(PlayerJumpStaminaBar);
                    break;

                case PlayerAttributes.SprintStamina:
                    UpdateBar(PlayerSprintStaminaBar);
                    break;

                case PlayerAttributes.AttackStamina:
                    UpdateBar(PlayerAttackStaminaBar);
                    break;
            }
        }
    }

    /// <summary>
    /// Função chamada quando o botão de upgrade do atributo esécificado é destacado com o mouse.
    /// </summary>
    /// <param name="attribute">O atributo do upgrade.</param>
    /// <param name="button">O botão destacado.</param>
    /// <param name="hovered">Se está sendo destacado ou desfocado.</param>
    public void OnHoverWeaponUpgrade(WeaponAttributes attribute, BaseButton button, bool hovered)
    {
        if (storeScreen.SelectedItem.Data is StoreWeaponData storeWeapon)
        {
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
                        (upgradeItem.Value + storeWeapon.WeaponData.MagazineSize).ToString(),
                        storeWeapon.WeaponData.MagazineSize.ToString());
                    break;

                case WeaponAttributes.Dirspersion:
                    if (storeWeapon.WeaponData is ShotgunData shotgunData)
                    {
                        UpdateIconStat(PreviewDispersionText,
                            (shotgunData.PelletsDispersion - upgradeItem.Value).ToString(),
                            shotgunData.PelletsDispersion.ToString());
                    }
                    else if (storeWeapon.WeaponData is LauncherData launcherData)
                    {
                        UpdateIconStat(PreviewDispersionText,
                            ((launcherData.ExplosionMinDamageRadius + launcherData.ExplosionMaxDamageRadius) / 2 + upgradeItem.Value).ToString("N1"),
                            ((launcherData.ExplosionMinDamageRadius + launcherData.ExplosionMaxDamageRadius) / 2).ToString("N1"));
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Função chamada quando o botão de upgrade do atributo esécificado é clicado.
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
                        BulletTypes.Shotgun => upgradeItem.Value / (storeWeapon.WeaponData as ShotgunData).ShellPelletsCount,
                        _ => upgradeItem.Value
                    };
                    UpdateBar(DamageBar);
                    break;

                case WeaponAttributes.FireRate:
                    if (storeWeapon.WeaponData is BurstFireData burstWeapon)
                    {
                        float fireRatesSum = burstWeapon.FireRate + burstWeapon.BurstFireRate;
                        float fireRateAverage = fireRatesSum / 2;
                        float targetFireRateAverage = fireRateAverage + upgradeItem.Value;

                        float fireRateWeight = burstWeapon.FireRate / fireRatesSum;
                        float burstFireRateWeight = burstWeapon.BurstFireRate / fireRatesSum;

                        float fireRateAverageDifference = (targetFireRateAverage * 2) - fireRatesSum;

                        burstWeapon.FireRate += fireRateAverageDifference * fireRateWeight;
                        burstWeapon.BurstFireRate += fireRateAverageDifference * burstFireRateWeight;
                    }
                    else
                    {
                        storeWeapon.WeaponData.FireRate += upgradeItem.Value;
                    }

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
                    float currentValuesSum = storeWeapon.WeaponData.MinDamageRange + storeWeapon.WeaponData.MaxDamageRange + storeWeapon.WeaponData.BulletMaxRange;
                    float currentAverage = currentValuesSum / 3;
                    float targetAverage = currentAverage + upgradeItem.Value;

                    float minDamageRangeWeight = storeWeapon.WeaponData.MinDamageRange / currentValuesSum;
                    float maxDamageRangeWeight = storeWeapon.WeaponData.MaxDamageRange / currentValuesSum;
                    float bulletMaxRangeWeight = storeWeapon.WeaponData.BulletMaxRange / currentValuesSum;

                    float averageDifference = (targetAverage * 3) - currentValuesSum;

                    storeWeapon.WeaponData.MinDamageRange += averageDifference * minDamageRangeWeight;
                    storeWeapon.WeaponData.MaxDamageRange += averageDifference * maxDamageRangeWeight;
                    storeWeapon.WeaponData.BulletMaxRange += averageDifference * bulletMaxRangeWeight;
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
                    storeWeapon.WeaponData.MagazineSize += (int)upgradeItem.Value;
                    UpdateIconStat(PreviewMagazineBulletsText, storeWeapon.WeaponData.MagazineSize.ToString());
                    break;

                case WeaponAttributes.Dirspersion:
                    if (storeWeapon.WeaponData is ShotgunData shotgunData)
                    {
                        shotgunData.PelletsDispersion -= upgradeItem.Value;
                        UpdateIconStat(PreviewDispersionText, shotgunData.PelletsDispersion.ToString());
                    }
                    else if (storeWeapon.WeaponData is LauncherData launcherData)
                    {
                        float radiusSum = launcherData.ExplosionMinDamageRadius + launcherData.ExplosionMaxDamageRadius;
                        float radiusAverage = radiusSum / 2;
                        float targetRadiusAverage = radiusAverage + upgradeItem.Value;

                        float minRadiusWeight = launcherData.ExplosionMinDamageRadius / radiusSum;
                        float maxRadiusWeight = launcherData.ExplosionMaxDamageRadius / radiusSum;

                        float radiusAverageDifference = (targetRadiusAverage * 2) - radiusSum;

                        launcherData.ExplosionMinDamageRadius += radiusAverageDifference * minRadiusWeight;
                        launcherData.ExplosionMaxDamageRadius += radiusAverageDifference * maxRadiusWeight;
                        UpdateIconStat(PreviewDispersionText, targetRadiusAverage.ToString("N1"));
                    }
                    break;
            }

            storeScreen.PlayerData.TakeMoney(upgradeItem.Price);
            storeScreen.audioSource.PlayOneShot(storeScreen.PurchaseSound.Audio, storeScreen.PurchaseSound.Volume);
            storeScreen.ShowPopup($"-{upgradeItem.Price:N2}", Color.red, storeScreen.PlayerMoneyText.transform.position);

            upgradeMap.UpgradeStep++;

            PreviewBtnSellText.text = $"Sell for $ {GetWeaponSellPrice(storeWeapon)}";

            storeScreen.IsSaveDirty = true;
        }
    }

    /// <summary>
    /// Função chamada quando o botão de upgrade da mochila é clicado..
    /// </summary>
    public void BuyBackpackUpgrade()
    {
        var data = storeScreen.SelectedItem.Data as StoreBackpackData;
        var upgradeItem = data.AmmoUpgrades.ElementAtOrDefault(Inventory.UpgradeIndex);
        if (upgradeItem == null)
            return;

        if (storeScreen.PlayerData.Money < upgradeItem.Price)
            return;

        Inventory.MaxPistolAmmo += upgradeItem.PistolAmmo;
        Inventory.MaxRifleAmmo += upgradeItem.RifleAmmo;
        Inventory.MaxShotgunAmmo += upgradeItem.ShotgunAmmo;
        Inventory.MaxSniperAmmo += upgradeItem.SniperAmmo;
        Inventory.MaxRocketAmmo += upgradeItem.RocketAmmo;

        storeScreen.PlayerData.TakeMoney(upgradeItem.Price);
        storeScreen.audioSource.PlayOneShot(storeScreen.PurchaseSound.Audio, storeScreen.PurchaseSound.Volume);
        storeScreen.ShowPopup($"-{upgradeItem.Price:N2}", Color.red, storeScreen.PlayerMoneyText.transform.position);

        Inventory.UpgradeIndex++;

        if (Inventory.UpgradeIndex >= data.AmmoUpgrades.Count)
        {
            BackpackBtnUpgrade.button.interactable = false;

            void setAmmo(TextMeshProUGUI txt)
            {
                txt.GetComponent<BlinkingText>().enabled = false;
                txt.color = Constants.Colors.GreenMoney;
                txt.text = "MAX";
            }

            setAmmo(BackpackPistolAmmoUpgrade);
            setAmmo(BackpackShotgunAmmoUpgrade);
            setAmmo(BackpackRifleAmmoUpgrade);
            setAmmo(BackpackSniperAmmoUpgrade);
            setAmmo(BackpackRocketAmmoUpgrade);
        }

        storeScreen.IsSaveDirty = true;
    }

    /// <summary>
    /// Função chamada quando o botão de upgrade do atributo especificado do jogador é clicado.
    /// </summary>
    /// <param name="attributeValue"></param>
    public void BuyPlayerUpgrade(int attributeValue)
    {
        PlayerAttributes attribute = (PlayerAttributes)attributeValue;

        if (storeScreen.SelectedItem.Data is StorePlayerData storePlayer)
        {
            var upgrades = storePlayer.GetPlayerUpgrades(attribute);
            var upgradeItem = upgrades.ElementAtOrDefault(storeScreen.PlayerData.GetUpgradeIndex(attribute));

            var nextUpgradeItem = upgrades.ElementAtOrDefault(storeScreen.PlayerData.GetUpgradeIndex(attribute) + 1);

            if (upgradeItem == null)
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

            switch (attribute)
            {
                case PlayerAttributes.MaxHealth:
                    storeScreen.PlayerData.MaxHealth += upgradeItem.Value;
                    UpdateBar(PlayerMaxHealthBar);
                    storeScreen.PlayerData.MaxHealthUpgradeIndex++;
                    break;

                case PlayerAttributes.MovementSpeed:
                    storeScreen.PlayerData.MaxMovementSpeed += upgradeItem.Value;
                    UpdateBar(PlayerMovementSpeedBar);
                    storeScreen.PlayerData.MovementSpeedUpgradeIndex++;
                    break;

                case PlayerAttributes.SprintSpeed:
                    storeScreen.PlayerData.SprintSpeedMultiplier += upgradeItem.Value;
                    UpdateBar(PlayerSprintSpeedBar);
                    storeScreen.PlayerData.SprintSpeedUpgradeIndex++;
                    break;

                case PlayerAttributes.JumpForce:
                    storeScreen.PlayerData.JumpForce += upgradeItem.Value;
                    UpdateBar(PlayerJumpForceBar);
                    storeScreen.PlayerData.JumpForceUpgradeIndex++;
                    break;

                case PlayerAttributes.MaxStamina:
                    storeScreen.PlayerData.MaxStamina += upgradeItem.Value;
                    UpdateBar(PlayerMaxStaminaBar);
                    storeScreen.PlayerData.MaxStaminaUpgradeIndex++;
                    break;

                case PlayerAttributes.StaminaRegen:
                    storeScreen.PlayerData.StaminaRegenRate += upgradeItem.Value;
                    UpdateBar(PlayerStaminaRegenBar);
                    storeScreen.PlayerData.StaminaRegenUpgradeIndex++;
                    break;

                case PlayerAttributes.StaminaHaste:
                    storeScreen.PlayerData.StaminaRegenDelayMs = 5000 / (PlayerStaminaHasteBar.Value + upgradeItem.Value);
                    UpdateBar(PlayerStaminaHasteBar);
                    storeScreen.PlayerData.StaminaHasteUpgradeIndex++;
                    break;

                case PlayerAttributes.JumpStamina:
                    storeScreen.PlayerData.JumpStaminaDrain = 100 / (PlayerJumpStaminaBar.Value + upgradeItem.Value);
                    UpdateBar(PlayerJumpStaminaBar);
                    storeScreen.PlayerData.JumpStaminaUpgradeIndex++;
                    break;

                case PlayerAttributes.SprintStamina:
                    storeScreen.PlayerData.SprintStaminaDrain = 100 / (PlayerSprintStaminaBar.Value + upgradeItem.Value);
                    UpdateBar(PlayerSprintStaminaBar);
                    storeScreen.PlayerData.SprintStaminaUpgradeIndex++;
                    break;

                case PlayerAttributes.AttackStamina:
                    storeScreen.PlayerData.AttackStaminaDrain = 100 / (PlayerAttackStaminaBar.Value + upgradeItem.Value);
                    UpdateBar(PlayerAttackStaminaBar);
                    storeScreen.PlayerData.AttackStaminaUpgradeIndex++;
                    break;
            }

            storeScreen.PlayerData.TakeMoney(upgradeItem.Price);
            storeScreen.PurchaseSound.PlayIfNotNull(storeScreen.audioSource, AudioTypes.UI);
            storeScreen.ShowPopup($"-{upgradeItem.Price:N2}", Color.red, storeScreen.PlayerMoneyText.transform.position);

            storeScreen.IsSaveDirty = true;
        }
    }

    private InventoryData.WeaponSelection GetWeaponSelection(BaseWeaponData weaponData)
    {
        var weaponsSelection = storeScreen.PlayerData.InventoryData.WeaponsSelection;
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
    /// Função chamada quando o player clica no botão de vender.
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
        storeScreen.PurchaseSound.PlayIfNotNull(storeScreen.audioSource, AudioTypes.UI);
        storeScreen.ShowPopup($"+{value:N2}", Color.green, storeScreen.PlayerMoneyText.transform.position);

        storeScreen.IsSaveDirty = true;
    }

    /// <summary>
    /// Verifica se o player possui a arma e a remove.
    /// </summary>
    /// <param name="data">Os dados da arma a ser vendida.</param>
    /// <returns>Se a arma foi encontrada e removida.</returns>
    private bool SellWeapon(StoreWeaponData data)
    {
        var weapon = Inventory.WeaponsSelection
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

        storeScreen.IsSaveDirty = true;

        return true;
    }

    /// <summary>
    /// Verifica se o player possui o arremessável e o remove.
    /// </summary>
    /// <param name="data">Os dados do arremessável a ser vendido.</param>
    /// <returns>Se o arremessável foi encontrado e removido.</returns>
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

        storeScreen.IsSaveDirty = true;

        return true;
    }
}
