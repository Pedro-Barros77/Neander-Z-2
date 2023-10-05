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
    TextMeshProUGUI PreviewTitleText, PreviewHeadshotMultiplierText, PreviewMgazineBulletsText, PreviewPelletsCountText, PreviewDispersionText;
    [SerializeField]
    Image PreviewBulletIcon;
    [SerializeField]
    Button BtnSwitchWeapons;

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
    /// Troca as armas entre os slots primário e secundário.
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
    /// Carraga os itens arremessáveis do inventário.
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
    /// Carrega as habilidades táticas do inventário.
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
    /// Seleciona um item para a visualização no preview.
    /// </summary>
    /// <param name="item">O item a ser selecionado.</param>
    public void SelectItem(StoreItem item)
    {
        PreviewTitleText.text = item.Data.Title;

        if (item.Data.IsWeapon)
        {
            var data = item.Data as StoreWeaponData;
            int pelletsCount;
            float pelletsDispersion;
            if (data.WeaponData is ShotgunData shotgunData)
            {
                pelletsCount = shotgunData.ShellPelletsCount;
                pelletsDispersion = shotgunData.PelletsDispersion;
            }
            else
            {
                pelletsCount = 0;
                pelletsDispersion = 0;
            }

            PreviewHeadshotMultiplierText.text = data.WeaponData.HeadshotMultiplier.ToString("N1");
            PreviewMgazineBulletsText.text = data.WeaponData.MagazineSize.ToString();
            PreviewPelletsCountText.text = pelletsCount.ToString();
            PreviewDispersionText.text = pelletsDispersion.ToString();

            PreviewPelletsCountText.transform.parent.gameObject.SetActive(pelletsCount > 0);
            PreviewDispersionText.transform.parent.gameObject.SetActive(pelletsDispersion > 0);

            PreviewBulletIcon.sprite = data.WeaponData.BulletType switch
            {
                BulletTypes.Pistol => storeScreen.PistolBulletIcon,
                BulletTypes.Shotgun => storeScreen.ShotgunBulletIcon,
                BulletTypes.AssaultRifle => storeScreen.RifleAmmoIcon,
                BulletTypes.Sniper => storeScreen.SniperAmmoIcon,
                BulletTypes.Rocket => storeScreen.RocketAmmoIcon,
                BulletTypes.Melee => storeScreen.MeleeAmmoIcon,
                _ => null,
            };
        }
    }
}
