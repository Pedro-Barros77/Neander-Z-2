using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryTab : MonoBehaviour
{
    StoreScreen storeScreen;

    [SerializeField]
    Transform weaponsColumn, throwablesColumn, itemsColumn, skillsColumn;
    [SerializeField]
    GameObject StoreItemPrefab;

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
}
