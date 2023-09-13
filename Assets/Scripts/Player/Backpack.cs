using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A mochila do jogador, carrega suas armas e acess�rios.
/// </summary>
public class Backpack
{
    public InventoryData Data;

    #region Data Properties forwarding

    /// <summary>
    /// N�mero de muni��es de pistola restantes.
    /// </summary>
    public int PistolAmmo => Data.PistolAmmo;
    /// <summary>
    /// N�mero de muni��es de escopeta restantes.
    /// </summary>
    public int ShotgunAmmo => Data.ShotgunAmmo;
    /// <summary>
    /// N�mero de muni��es de fuzil restantes.
    /// </summary>
    public int RifleAmmo => Data.RifleAmmo;
    /// <summary>
    /// N�mero de muni��es de sniper restantes.
    /// </summary>
    public int SniperAmmo => Data.SniperAmmo;
    /// <summary>
    /// N�mero de muni��es de foguete restantes.
    /// </summary>
    public int RocketAmmo => Data.RocketAmmo;
    /// <summary>
    /// N�mero m�ximo de muni��es de pistola que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxPistolAmmo => Data.MaxPistolAmmo;
    /// <summary>
    /// N�mero m�ximo de muni��es de escopeta que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxShotgunAmmo => Data.MaxShotgunAmmo;
    /// <summary>
    /// N�mero m�ximo de muni��es de fuzil que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxRifleAmmo => Data.MaxRifleAmmo;
    /// <summary>
    /// N�mero m�ximo de muni��es de sniper que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxSniperAmmo => Data.MaxSniperAmmo;
    /// <summary>
    /// N�mero m�ximo de muni��es de foguete que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxRocketAmmo => Data.MaxRocketAmmo;
    /// <summary>
    /// �ndice da arma atualmente equipada nas m�os do jogador (0= Prim�ria, 1= Secund�ria).
    /// </summary>
    public int CurrentWeaponIndex => Data.CurrentWeaponIndex;

    #endregion

    /// <summary>
    /// Lista de armas prim�rias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> PrimaryWeaponsInstances { get; private set; }
    /// <summary>
    /// Lista de armas secund�rias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> SecondaryWeaponsInstances { get; private set; }
    /// <summary>
    /// Refer�ncia ao jogador portador dessa mochila.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// O tipo da arma atualmente escolhida como prim�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedPrimaryType => Data.PrimaryWeaponsSelection.Concat(Data.SecondaryWeaponsSelection).FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary)?.Type ?? WeaponTypes.None;
    /// <summary>
    /// O tipo da arma atualmente escolhida como secund�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedSecondaryType => Data.SecondaryWeaponsSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary)?.Type ?? WeaponTypes.None;
    /// <summary>
    /// O tipo do item arremess�vel atualmente escolhido pelo jogador.
    /// </summary>
    public ThrowableTypes EquippedThrowableType => Data.ThrowableItemsSelection.FirstOrDefault(x => x.IsEquipped)?.Type ?? ThrowableTypes.None;

    public bool HasPrimaryEquipped => EquippedPrimaryType != WeaponTypes.None;
    public bool HasSecondaryEquipped => EquippedSecondaryType != WeaponTypes.None;

    /// <summary>
    /// Retorna a arma prim�ria escolhida, caso exista no arsenal de prim�rias.
    /// </summary>
    public BaseWeapon EquippedPrimaryWeapon => PrimaryWeaponsInstances.Concat(SecondaryWeaponsInstances).First(w => w.Type == EquippedPrimaryType);
    /// <summary>
    /// Retorna a arma secund�ria escolhida, caso exista no arsenal de secund�rias.
    /// </summary>
    public BaseWeapon EquippedSecondaryWeapon => SecondaryWeaponsInstances.First(w => w.Type == EquippedSecondaryType);
    /// <summary>
    /// Retorna a arma atualmente equipada nas m�os do jogador.
    /// </summary>
    public BaseWeapon EquippedWeapon => CurrentWeaponIndex == 0 ? EquippedPrimaryWeapon : EquippedSecondaryWeapon;
    /// <summary>
    /// Arremess�vel atualmente equipado pelo jogador.
    /// </summary>
    public InventoryData.ThrowableSelection EquippedThrowable => Data.ThrowableItemsSelection.FirstOrDefault(x => x.Type == EquippedThrowableType);
    /// <summary>
    /// O item atualmente sendo arremessado pelo jogador.
    /// </summary>
    public BaseThrowable ThrowingThrowable;

    public Backpack(Player player, InventoryData data)
    {
        Data = data;
        data.CurrentWeaponIndex = EquippedPrimaryType != WeaponTypes.None ? 0 : 1;
        Player = player;
        PrimaryWeaponsInstances = new List<BaseWeapon>();
        SecondaryWeaponsInstances = new List<BaseWeapon>();

        bool isEquippedPrimary = CurrentWeaponIndex == 0;

        if (EquippedPrimaryType != WeaponTypes.None)
        {
            var primWeapon = AddWeapon(EquippedPrimaryType, WeaponEquippedSlot.Primary);
            EquippedPrimaryWeapon.IsActive = isEquippedPrimary;
        }

        if (EquippedSecondaryType != WeaponTypes.None)
        {
            var secWeapon = AddWeapon(EquippedSecondaryType, Data.PrimaryWeaponsSelection.Concat(Data.SecondaryWeaponsSelection).FirstOrDefault(x => x.Type == EquippedSecondaryType).EquippedSlot);
            EquippedSecondaryWeapon.IsActive = !isEquippedPrimary;
        }
    }

    /// <summary>
    /// Adiciona uma nova arma ao arsenal do jogador.
    /// </summary>
    /// <param name="weaponType">Tipo da nova arma a ser adicionada.</param>
    /// <param name="slot">Se essa arma deve ser equipada nos slots de Prim�ria ou Secund�ria, logo ap�s ser adicionada.</param>
    /// <returns>A inst�ncia da arma adicionada.</returns>
    public BaseWeapon AddWeapon(WeaponTypes weaponType, WeaponEquippedSlot slot = WeaponEquippedSlot.None)
    {
        if (PrimaryWeaponsInstances.Any(x => x.Type == weaponType) || SecondaryWeaponsInstances.Any(x => x.Type == weaponType))
        {
            Debug.LogWarning($"Tentativa de adicionar arma {weaponType} ao arsenal do jogador, mas ela j� existe.");
            return null;
        }
        GameObject weaponObj = Player.WeaponController.InstantiateWeaponPrefab(weaponType);
        BaseWeapon weapon = weaponObj.GetComponent<BaseWeapon>();

        void Unequip()
        {
            if (slot != WeaponEquippedSlot.None)
            {
                if (slot == WeaponEquippedSlot.Primary)
                {
                    UnequipAll(true, true);
                    UnequipAll(false, true);
                }

                if (slot == WeaponEquippedSlot.Secondary)
                    UnequipAll(false, false);
            }
        }

        if (weapon.IsPrimary)
        {
            if (slot == WeaponEquippedSlot.Primary && !Data.PrimaryWeaponsSelection.Any(x => x.Type == weaponType))
            {
                Unequip();
                Data.PrimaryWeaponsSelection.Add(new InventoryData.WeaponSelection(weaponType, slot));
            }

            PrimaryWeaponsInstances.Add(weapon);
        }
        else
        {
            if (slot != WeaponEquippedSlot.None && !Data.SecondaryWeaponsSelection.Any(x => x.Type == weaponType))
            {
                Unequip();
                Data.SecondaryWeaponsSelection.Add(new InventoryData.WeaponSelection(weaponType, slot));
            }

            SecondaryWeaponsInstances.Add(weapon);
        }

        return weapon;
    }

    //public void AddThrowable(ThrowableTypes throwableType, bool equip = true)
    //{
    //    if (ThrowableItemsArsenal.Any(x => x.Type == throwableType))
    //    {
    //        Debug.LogWarning($"Tentativa de adicionar um item {throwableType} ao arsenal do jogador, mas ele j� existe.");
    //        return null;
    //    }

    //    ThrowableItemsArsenal.Add(weapon);

    //    if (equip)
    //    {
    //        Data.EquippedThrowableType = throwableType;
    //    }
    //}

    /// <summary>
    /// Alternar entre as armas prim�ria e secund�ria equipadas.
    /// </summary>
    /// <param name="index">O �ndice da arma a ser equipada. 0 = prim�ria, 1 = secund�ria. Null = inverter.</param>
    public void SwitchWeapon(int? index = null)
    {
        Data.CurrentWeaponIndex = index ?? (CurrentWeaponIndex == 0 ? 1 : 0);
    }

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
                Data.PistolAmmo = count;
                break;
            case BulletTypes.Shotgun:
                Data.ShotgunAmmo = count;
                break;
            case BulletTypes.AssaultRifle:
                Data.RifleAmmo = count;
                break;
            case BulletTypes.Sniper:
                Data.SniperAmmo = count;
                break;
            case BulletTypes.Rocket:
                Data.RocketAmmo = count;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Marca todas as armas especificadas como n�o equipadas.
    /// </summary>
    /// <param name="primaryWeapons">Se a a��o deve ser realizada na lista de prim�rias, caso contr�rio, lista de secund�rias.</param>
    /// <param name="primarySlots">Se a a��o deve ser realizada para as armas equipadas no slot de prim�ria, caso contr�rio, slot de secund�ria.</param>
    private void UnequipAll(bool primaryWeapons, bool primarySlots)
    {
        var list = primaryWeapons ? Data.PrimaryWeaponsSelection : Data.SecondaryWeaponsSelection;

        foreach (var weapon in list)
        {
            if (primarySlots && weapon.EquippedSlot == WeaponEquippedSlot.Primary)
                weapon.EquippedSlot = WeaponEquippedSlot.None;
            else if (!primarySlots && weapon.EquippedSlot == WeaponEquippedSlot.Secondary)
                weapon.EquippedSlot = WeaponEquippedSlot.None;
        }
    }
}
