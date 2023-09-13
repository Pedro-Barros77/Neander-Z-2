using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Data", menuName = "Neander Z/Player/Inventory Data", order = 3)]
public class InventoryData : AutoRevertSO
{
    public int PistolAmmo;
    public int ShotgunAmmo;
    public int RifleAmmo;
    public int SniperAmmo;
    public int RocketAmmo;

    public int MaxPistolAmmo;
    public int MaxShotgunAmmo;
    public int MaxRifleAmmo;
    public int MaxSniperAmmo;
    public int MaxRocketAmmo;

    public int CurrentWeaponIndex;

    public List<WeaponSelection> PrimaryWeaponsSelection;
    public List<WeaponSelection> SecondaryWeaponsSelection;
    public List<ThrowableSelection> ThrowableItemsSelection;

    public bool HasWeapon(WeaponTypes weaponType)
            => PrimaryWeaponsSelection.Any(w => w.Type == weaponType)
            || SecondaryWeaponsSelection.Any(w => w.Type == weaponType);


    [Serializable]
    public class WeaponSelection
    {
        [SerializeField]
        public WeaponTypes Type;
        public WeaponEquippedSlot EquippedSlot;
        public bool IsPrimary => Constants.IsPrimary(Type);

        public WeaponSelection() { }
        public WeaponSelection(WeaponTypes type, WeaponEquippedSlot equippedSlot)
        {
            Type = type;
            EquippedSlot = equippedSlot;
        }
    }

    [Serializable]
    public class ThrowableSelection
    {
        public ThrowableTypes Type;
        public int Count;
        public bool IsEquipped;
    }
}


