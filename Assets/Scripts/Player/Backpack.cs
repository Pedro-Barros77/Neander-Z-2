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
    public IEnumerable<BaseWeapon> WeaponsInstances =>
        (!PrimaryWeaponsInstances.IsNullOrEmpty() ? PrimaryWeaponsInstances : new())
        .Concat(!SecondaryWeaponsInstances.IsNullOrEmpty() ? SecondaryWeaponsInstances : new());
    /// <summary>
    /// Refer�ncia ao jogador portador dessa mochila.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// O tipo da arma atualmente escolhida como prim�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedPrimaryType => Data.WeaponsSelection.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Primary)?.Type ?? WeaponTypes.None;
    /// <summary>
    /// O tipo da arma atualmente escolhida como secund�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedSecondaryType => Data.WeaponsSelection?.FirstOrDefault(x => x.EquippedSlot == WeaponEquippedSlot.Secondary)?.Type ?? WeaponTypes.None;
    /// <summary>
    /// O tipo do item arremess�vel atualmente escolhido pelo jogador.
    /// </summary>
    public ThrowableTypes EquippedThrowableType => Data.ThrowableItemsSelection?.FirstOrDefault(x => x.IsEquipped)?.Type ?? ThrowableTypes.None;
    /// <summary>
    /// O tipo da habilidade t�tica atualmente escolhida pelo jogador.
    /// </summary>
    public TacticalAbilityTypes EquippedTacticalAbilityType => Data.TacticalAbilitiesSelection?.FirstOrDefault(x => x.IsEquipped)?.Type ?? TacticalAbilityTypes.None;
    /// <summary>
    /// O tipo da habilidade t�tica atualmente escolhida pelo jogador.
    /// </summary>
    public PassiveSkillTypes EquippedPassiveSkillType => Data.PassiveSkillsSelection?.FirstOrDefault(x => x.IsEquipped)?.Type ?? PassiveSkillTypes.None;

    public bool HasPrimaryEquipped => EquippedPrimaryType != WeaponTypes.None;
    public bool HasSecondaryEquipped => EquippedSecondaryType != WeaponTypes.None;

    /// <summary>
    /// Retorna a arma prim�ria escolhida, caso exista no arsenal de prim�rias.
    /// </summary>
    public BaseWeapon EquippedPrimaryWeapon => WeaponsInstances.FirstOrDefault(w => w.Type == EquippedPrimaryType);
    /// <summary>
    /// Retorna a arma secund�ria escolhida, caso exista no arsenal de secund�rias.
    /// </summary>
    public BaseWeapon EquippedSecondaryWeapon => WeaponsInstances.FirstOrDefault(w => w.Type == EquippedSecondaryType);
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
            var primWeapon = AddWeapon(EquippedPrimaryType, Data.WeaponsSelection.FirstOrDefault(x => x.Type == EquippedPrimaryType).EquippedSlot);
            EquippedPrimaryWeapon.IsActive = isEquippedPrimary;
        }

        if (EquippedSecondaryType != WeaponTypes.None)
        {
            var secWeapon = AddWeapon(EquippedSecondaryType, Data.WeaponsSelection.FirstOrDefault(x => x.Type == EquippedSecondaryType).EquippedSlot);
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
        if (WeaponsInstances.Any(x => x.Type == weaponType))
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
                    Data.UnequipAllWeapons(true, true);
                    Data.UnequipAllWeapons(false, true);
                }

                if (slot == WeaponEquippedSlot.Secondary)
                    Data.UnequipAllWeapons(false, false);
            }
        }

        if (weapon.IsPrimary)
        {
            if (slot == WeaponEquippedSlot.Primary && !Data.WeaponsSelection.Any(x => x.Type == weaponType))
            {
                Unequip();
                Data.PrimaryWeaponsSelection.Add(new InventoryData.WeaponSelection(weaponType, slot, weapon.Data.WeaponClass));
            }

            PrimaryWeaponsInstances.Add(weapon);
        }
        else
        {
            if (slot != WeaponEquippedSlot.None && !Data.WeaponsSelection.Any(x => x.Type == weaponType))
            {
                Unequip();
                Data.SecondaryWeaponsSelection.Add(new InventoryData.WeaponSelection(weaponType, slot, weapon.Data.WeaponClass));
            }

            SecondaryWeaponsInstances.Add(weapon);
        }

        return weapon;
    }

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
    public int GetAmmo(BulletTypes type) => Data.GetAmmo(type);

    /// <summary>
    /// Retorna a quantidade m�xima de muni��es do tipo especificado que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    /// <param name="type">O tipo de muni��o a ser avaliado.</param>
    /// <returns>O n�mero m�ximo de muni��es que podem ser carregadas.</returns>
    public int GetMaxAmmo(BulletTypes type) => Data.GetMaxAmmo(type);

    /// <summary>
    /// Define a quantidade de muni��es do tipo especificado que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de muni��o a ser definida.</param>
    /// <param name="count">A quantidade a ser definida.</param>
    public void SetAmmo(BulletTypes type, int count) => Data.SetAmmo(type, count);
}
