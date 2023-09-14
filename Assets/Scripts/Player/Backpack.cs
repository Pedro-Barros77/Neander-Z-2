using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A mochila do jogador, carrega suas armas e acessórios.
/// </summary>
public class Backpack
{
    public InventoryData Data;

    #region Data Properties forwarding

    /// <summary>
    /// Número de munições de pistola restantes.
    /// </summary>
    public int PistolAmmo => Data.PistolAmmo;
    /// <summary>
    /// Número de munições de escopeta restantes.
    /// </summary>
    public int ShotgunAmmo => Data.ShotgunAmmo;
    /// <summary>
    /// Número de munições de fuzil restantes.
    /// </summary>
    public int RifleAmmo => Data.RifleAmmo;
    /// <summary>
    /// Número de munições de sniper restantes.
    /// </summary>
    public int SniperAmmo => Data.SniperAmmo;
    /// <summary>
    /// Número de munições de foguete restantes.
    /// </summary>
    public int RocketAmmo => Data.RocketAmmo;
    /// <summary>
    /// Número máximo de munições de pistola que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxPistolAmmo => Data.MaxPistolAmmo;
    /// <summary>
    /// Número máximo de munições de escopeta que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxShotgunAmmo => Data.MaxShotgunAmmo;
    /// <summary>
    /// Número máximo de munições de fuzil que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxRifleAmmo => Data.MaxRifleAmmo;
    /// <summary>
    /// Número máximo de munições de sniper que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxSniperAmmo => Data.MaxSniperAmmo;
    /// <summary>
    /// Número máximo de munições de foguete que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxRocketAmmo => Data.MaxRocketAmmo;
    /// <summary>
    /// Índice da arma atualmente equipada nas mãos do jogador (0= Primária, 1= Secundária).
    /// </summary>
    public int CurrentWeaponIndex => Data.CurrentWeaponIndex;

    #endregion

    /// <summary>
    /// Lista de armas primárias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> PrimaryWeaponsInstances { get; private set; }
    /// <summary>
    /// Lista de armas secundárias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> SecondaryWeaponsInstances { get; private set; }
    /// <summary>
    /// Referência ao jogador portador dessa mochila.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// O tipo da arma atualmente escolhida como primária pelo jogador.
    /// </summary>
    public WeaponTypes EquippedPrimaryType => Data.PrimaryWeaponsSelection?.Concat(Data.SecondaryWeaponsSelection).FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary)?.Type ?? WeaponTypes.None;
    /// <summary>
    /// O tipo da arma atualmente escolhida como secundária pelo jogador.
    /// </summary>
    public WeaponTypes EquippedSecondaryType => Data.SecondaryWeaponsSelection?.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary)?.Type ?? WeaponTypes.None;
    /// <summary>
    /// O tipo do item arremessável atualmente escolhido pelo jogador.
    /// </summary>
    public ThrowableTypes EquippedThrowableType => Data.ThrowableItemsSelection?.FirstOrDefault(x => x.IsEquipped)?.Type ?? ThrowableTypes.None;

    public bool HasPrimaryEquipped => EquippedPrimaryType != WeaponTypes.None;
    public bool HasSecondaryEquipped => EquippedSecondaryType != WeaponTypes.None;

    /// <summary>
    /// Retorna a arma primária escolhida, caso exista no arsenal de primárias.
    /// </summary>
    public BaseWeapon EquippedPrimaryWeapon => PrimaryWeaponsInstances.Concat(SecondaryWeaponsInstances).First(w => w.Type == EquippedPrimaryType);
    /// <summary>
    /// Retorna a arma secundária escolhida, caso exista no arsenal de secundárias.
    /// </summary>
    public BaseWeapon EquippedSecondaryWeapon => SecondaryWeaponsInstances.First(w => w.Type == EquippedSecondaryType);
    /// <summary>
    /// Retorna a arma atualmente equipada nas mãos do jogador.
    /// </summary>
    public BaseWeapon EquippedWeapon => CurrentWeaponIndex == 0 ? EquippedPrimaryWeapon : EquippedSecondaryWeapon;
    /// <summary>
    /// Arremessável atualmente equipado pelo jogador.
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
    /// <param name="slot">Se essa arma deve ser equipada nos slots de Primária ou Secundária, logo após ser adicionada.</param>
    /// <returns>A instância da arma adicionada.</returns>
    public BaseWeapon AddWeapon(WeaponTypes weaponType, WeaponEquippedSlot slot = WeaponEquippedSlot.None)
    {
        if (PrimaryWeaponsInstances.Any(x => x.Type == weaponType) || SecondaryWeaponsInstances.Any(x => x.Type == weaponType))
        {
            Debug.LogWarning($"Tentativa de adicionar arma {weaponType} ao arsenal do jogador, mas ela já existe.");
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
                    Data.UnequipAllWeapons(true, true);
                    Data.UnequipAllWeapons(false, true);
                }

                if (slot == WeaponEquippedSlot.Secondary)
                    Data.UnequipAllWeapons(false, false);
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

    /// <summary>
    /// Alternar entre as armas primária e secundária equipadas.
    /// </summary>
    /// <param name="index">O índice da arma a ser equipada. 0 = primária, 1 = secundária. Null = inverter.</param>
    public void SwitchWeapon(int? index = null)
    {
        Data.CurrentWeaponIndex = index ?? (CurrentWeaponIndex == 0 ? 1 : 0);
    }

    /// <summary>
    /// Retorna a quantidade de munições do tipo especificado restantes que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de munição a ser avaliado.</param>
    /// <returns>O número de munições restantes.</returns>
    public int GetAmmo(BulletTypes type) => Data.GetAmmo(type);

    /// <summary>
    /// Retorna a quantidade máxima de munições do tipo especificado que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    /// <param name="type">O tipo de munição a ser avaliado.</param>
    /// <returns>O número máximo de munições que podem ser carregadas.</returns>
    public int GetMaxAmmo(BulletTypes type) => Data.GetMaxAmmo(type);

    /// <summary>
    /// Define a quantidade de munições do tipo especificado que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de munição a ser definida.</param>
    /// <param name="count">A quantidade a ser definida.</param>
    public void SetAmmo(BulletTypes type, int count) => Data.SetAmmo(type, count);
}
