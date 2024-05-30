using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Data", menuName = "Neander Z/Player/Inventory Data", order = 2)]
public class InventoryData : AutoRevertSO
{
    public int UpgradeIndex;

    public int PistolAmmo;
    public int ShotgunAmmo;
    public int RifleAmmo;
    public int SniperAmmo;
    public int RocketAmmo;
    public int FuelAmmo;

    public int MaxPistolAmmo;
    public int MaxShotgunAmmo;
    public int MaxRifleAmmo;
    public int MaxSniperAmmo;
    public int MaxRocketAmmo;
    public int MaxFuelAmmo;

    public int CurrentWeaponIndex;

    public List<WeaponSelection> PrimaryWeaponsSelection;
    public List<WeaponSelection> SecondaryWeaponsSelection;
    public List<ThrowableSelection> ThrowableItemsSelection;
    public List<TacticalAbilitySelection> TacticalAbilitiesSelection;
    public List<PassiveSkillSelection> PassiveSkillsSelection;
    public List<SupportEquipmentSelection> SupportEquipmentsSelection;

    public IEnumerable<WeaponSelection> WeaponsSelection =>
        (!PrimaryWeaponsSelection.IsNullOrEmpty() ? PrimaryWeaponsSelection : new())
        .Concat(!SecondaryWeaponsSelection.IsNullOrEmpty() ? SecondaryWeaponsSelection : new());

    public bool HasWeapon(WeaponTypes weaponType)
            => PrimaryWeaponsSelection.Any(w => w.Type == weaponType)
            || SecondaryWeaponsSelection.Any(w => w.Type == weaponType);

    public bool HasThrowable(ThrowableTypes throwableType) => ThrowableItemsSelection.Any(t => t.Type == throwableType);

    /// <summary>
    /// Marca todas as armas especificadas como n�o equipadas.
    /// </summary>
    /// <param name="primaryWeapons">Se a a��o deve ser realizada na lista de prim�rias, caso contr�rio, lista de secund�rias.</param>
    /// <param name="primarySlots">Se a a��o deve ser realizada para as armas equipadas no slot de prim�ria, caso contr�rio, slot de secund�ria.</param>
    public void UnequipAllWeapons(bool primaryWeapons, bool primarySlots)
    {
        var list = primaryWeapons ? PrimaryWeaponsSelection : SecondaryWeaponsSelection;

        foreach (var weapon in list)
        {
            if (primarySlots && weapon.EquippedSlot == WeaponEquippedSlot.Primary)
                weapon.EquippedSlot = WeaponEquippedSlot.None;
            else if (!primarySlots && weapon.EquippedSlot == WeaponEquippedSlot.Secondary)
                weapon.EquippedSlot = WeaponEquippedSlot.None;
        }
    }

    /// <summary>
    /// Marca todos os items arremess�veis como n�o equipados.
    /// </summary>
    public void UnequipAllThrowables() => ThrowableItemsSelection.ForEach(t => t.IsEquipped = false);

    /// <summary>
    /// Marca todas as habilidades passivas como n�o equipadas.
    /// </summary>
    public void UnequipAllPassiveSkills() => PassiveSkillsSelection.ForEach(t => t.IsEquipped = false);

    /// <summary>
    /// Marca todas as habilidades t�ticas como n�o equipadas.
    /// </summary>
    public void UnequipAllTacticalAbilities() => TacticalAbilitiesSelection.ForEach(t => t.IsEquipped = false);

    /// <summary>
    /// Marca todos os equipamentos de suporte como n�o equipados.
    /// </summary>
    public void UnequipAllSupportEquipments() => SupportEquipmentsSelection.ForEach(t => t.IsEquipped = false);

    /// <summary>
    /// Retorna a quantidade de muni��es do tipo especificado restantes que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de muni��o a ser avaliado.</param>
    /// <returns>O n�mero de muni��es restantes.</returns>
    public int GetAmmo(BulletTypes type) => type switch
    {
        BulletTypes.Pistol => PistolAmmo,
        BulletTypes.Shotgun => ShotgunAmmo,
        BulletTypes.AssaultRifle => RifleAmmo,
        BulletTypes.Sniper => SniperAmmo,
        BulletTypes.Rocket => RocketAmmo,
        BulletTypes.Fuel => FuelAmmo,
        _ => 0
    };

    /// <summary>
    /// Retorna a quantidade m�xima de muni��es do tipo especificado que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    /// <param name="type">O tipo de muni��o a ser avaliado.</param>
    /// <returns>O n�mero m�ximo de muni��es que podem ser carregadas.</returns>
    public int GetMaxAmmo(BulletTypes type) => type switch
    {
        BulletTypes.Pistol => MaxPistolAmmo,
        BulletTypes.Shotgun => MaxShotgunAmmo,
        BulletTypes.AssaultRifle => MaxRifleAmmo,
        BulletTypes.Sniper => MaxSniperAmmo,
        BulletTypes.Rocket => MaxRocketAmmo,
        BulletTypes.Fuel => MaxFuelAmmo,
        _ => 0
    };

    /// <summary>
    /// Define a quantidade de muni��es do tipo especificado que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de muni��o a ser definida.</param>
    /// <param name="count">A quantidade a ser definida.</param>
    public void SetAmmo(BulletTypes type, int count)
    {
        switch (type)
        {
            case BulletTypes.Pistol:
                PistolAmmo = Mathf.Clamp(count, 0, GetMaxAmmo(type));
                break;
            case BulletTypes.Shotgun:
                ShotgunAmmo = Mathf.Clamp(count, 0, GetMaxAmmo(type));
                break;
            case BulletTypes.AssaultRifle:
                RifleAmmo = Mathf.Clamp(count, 0, GetMaxAmmo(type));
                break;
            case BulletTypes.Sniper:
                SniperAmmo = Mathf.Clamp(count, 0, GetMaxAmmo(type));
                break;
            case BulletTypes.Rocket:
                RocketAmmo = Mathf.Clamp(count, 0, GetMaxAmmo(type));
                break;
            case BulletTypes.Fuel:
                FuelAmmo = Mathf.Clamp(count, 0, GetMaxAmmo(type));
                break;
            default:
                break;
        }
    }


    [Serializable]
    public class WeaponSelection
    {
        [SerializeField]
        public WeaponTypes Type;
        public WeaponClasses WeaponClass;
        public WeaponEquippedSlot EquippedSlot;
        public List<WeaponUpgradeMap> UpgradesMap;
        public bool IsPrimary => Constants.IsPrimary(Type);
        public int MagazineBullets;

        public WeaponSelection() { }
        public WeaponSelection(WeaponTypes type, WeaponEquippedSlot equippedSlot, WeaponClasses weaponClass)
        {
            Type = type;
            EquippedSlot = equippedSlot;
            WeaponClass = weaponClass;
        }

        [Serializable]
        public class WeaponUpgradeMap
        {
            public WeaponAttributes Attribute;
            public int UpgradeStep;
            public WeaponUpgradeMap(WeaponAttributes attribute, int upgradeStep)
            {
                Attribute = attribute;
                UpgradeStep = upgradeStep;
            }
        }
    }

    [Serializable]
    public class ThrowableSelection
    {
        public ThrowableTypes Type;
        public int Count;
        public int MaxCount;
        public bool IsEquipped;

        public ThrowableSelection() { }
        public ThrowableSelection(ThrowableTypes type, int count, int maxCount, bool isEquipped)
        {
            Type = type;
            Count = count;
            MaxCount = maxCount;
            IsEquipped = isEquipped;
        }

        public void SetCount(int count) => Count = Mathf.Clamp(count, 0, MaxCount);
    }

    [Serializable]
    public class TacticalAbilitySelection
    {
        public TacticalAbilityTypes Type;
        public bool IsEquipped;

        public TacticalAbilitySelection() { }
        public TacticalAbilitySelection(TacticalAbilityTypes type, bool isEquipped)
        {
            IsEquipped = isEquipped;
            Type = type;
        }
    }

    [Serializable]
    public class PassiveSkillSelection
    {
        public PassiveSkillTypes Type;
        public bool IsEquipped;

        public PassiveSkillSelection() { }
        public PassiveSkillSelection(PassiveSkillTypes type, bool isEquipped)
        {
            IsEquipped = isEquipped;
            Type = type;
        }
    }

    [Serializable]
    public class SupportEquipmentSelection
    {
        public SupportEquipmentTypes Type;
        public int Count;
        public int MaxCount;
        public bool IsEquipped;

        public SupportEquipmentSelection() { }
        public SupportEquipmentSelection(SupportEquipmentTypes type, int count, int maxCount, bool isEquipped)
        {
            IsEquipped = isEquipped;
            Count = count;
            MaxCount = maxCount;
            Type = type;
        }
    }
}


