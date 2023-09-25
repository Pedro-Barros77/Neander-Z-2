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

    InventoryData Inventory => storeScreen.PlayerData.InventoryData;

    void Start()
    {
        storeScreen = GetComponent<StoreScreen>();
        ClearInventoryItems();
        LoadWeapons();
        LoadThrowables();
        LoadTacticalAbilities();
    }

    void Update()
    {
        if (storeScreen.PlayerData != null)
        {
            PistolAmmo.text = Inventory.PistolAmmo.ToString();
            PistolMaxAmmo.text = $"/{Inventory.MaxPistolAmmo}";
            ShotgunAmmo.text = Inventory.ShotgunAmmo.ToString();
            ShotgunMaxAmmo.text = $"/{Inventory.MaxShotgunAmmo}";
            RifleAmmo.text = Inventory.RifleAmmo.ToString();
            RifleMaxAmmo.text = $"/{Inventory.MaxRifleAmmo}";
            SniperAmmo.text = Inventory.SniperAmmo.ToString();
            SniperMaxAmmo.text = $"/{Inventory.MaxSniperAmmo}";
            RocketAmmo.text = Inventory.RocketAmmo.ToString();
            RocketMaxAmmo.text = $"/{Inventory.MaxRocketAmmo}";
        }
    }

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

    void LoadWeapons()
    {
        var weaponDatas = Resources.LoadAll<StoreWeaponData>("ScriptableObjects/Store/Weapons");

        foreach (var weapon in Inventory.PrimaryWeaponsSelection.Concat(Inventory.SecondaryWeaponsSelection))
        {
            StoreWeaponData weaponData = weaponDatas.FirstOrDefault(x => x.WeaponType == weapon.Type);
            StoreItemPrefab.GetComponent<StoreItem>().Data = weaponData;

            GameObject weaponStoreItem = Instantiate(StoreItemPrefab, weaponsColumn);
            StoreItem storeItem = weaponStoreItem.GetComponent<StoreItem>();
            storeItem.IsInventoryItem = true;
        }
    }

    void LoadThrowables()
    {
        var throwablesDatas = Resources.LoadAll<StoreThrowableData>("ScriptableObjects/Store/Throwables");

        foreach (var throwable in Inventory.ThrowableItemsSelection)
        {
            StoreThrowableData throwableData = throwablesDatas.FirstOrDefault(x => x.ThrowableType == throwable.Type);
            StoreItemPrefab.GetComponent<StoreItem>().Data = throwableData;

            GameObject throwableStoreItem = Instantiate(StoreItemPrefab, throwablesColumn);
            StoreItem storeItem = throwableStoreItem.GetComponent<StoreItem>();
            storeItem.IsInventoryItem = true;
        }
    }

    void LoadTacticalAbilities()
    {
        var abilitiesDatas = Resources.LoadAll<StoreTacticalAbilityData>("ScriptableObjects/Store/TacticalAbilities");

        foreach (var ability in Inventory.TacticalAbilitiesSelection)
        {
            StoreTacticalAbilityData abilityData = abilitiesDatas.FirstOrDefault(x => x.AbilityType == ability.Type);
            StoreItemPrefab.GetComponent<StoreItem>().Data = abilityData;

            GameObject abilityStoreItem = Instantiate(StoreItemPrefab, skillsColumn);
            StoreItem storeItem = abilityStoreItem.GetComponent<StoreItem>();
            storeItem.IsInventoryItem = true;
        }
    }

    public void SelectItem(StoreItem item)
    {
        PreviewTitleText.text = item.Data.Title;

        if (item.Data.IsWeapon)
        {
            var data = item.Data as StoreWeaponData;
            PreviewHeadshotMultiplierText.text = data.HeadshotMultiplier.ToString("N1");
            PreviewMgazineBulletsText.text = data.MagazineBullets.ToString();
            PreviewPelletsCountText.text = data.PelletsCount.ToString();
            PreviewDispersionText.text = data.Dispersion.ToString();

            PreviewPelletsCountText.transform.parent.gameObject.SetActive(data.PelletsCount > 0);
            PreviewDispersionText.transform.parent.gameObject.SetActive(data.Dispersion > 0);

            PreviewBulletIcon.sprite = data.BulletType switch
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
