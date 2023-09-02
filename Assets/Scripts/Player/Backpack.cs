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
    /// �Define o n�vel de melhoria atual da mochila. Inicia em zero (sem upgrade).
    /// </summary>
    public int BackpackLevel => Data.BackpackLevel;
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
    //public List<Throwable> Throwables { get; set; }
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
    /// N�mero m�ximo de itens arremess�veis de cada tipo que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxThrowableType { get; private set; }

    /// <summary>
    /// �ndice da arma atualmente equipada nas m�os do jogador (0= Prim�ria, 1= Secund�ria).
    /// </summary>
    public int CurrentWeaponIndex => Data.CurrentWeaponIndex;

    #endregion

    /// <summary>
    /// Lista de armas prim�rias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> PrimaryWeaponsArsenal { get; private set; }
    /// <summary>
    /// Lista de armas secund�rias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> SecondaryWeaponsArsenal { get; private set; }
    /// <summary>
    /// Refer�ncia ao jogador portador dessa mochila.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// O tipo da arma atualmente escolhida como prim�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedPrimaryType => Data.EquippedPrimaryType;
    /// <summary>
    /// O tipo da arma atualmente escolhida como secund�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedSecondaryType => Data.EquippedSecondaryType;

    public bool HasPrimaryEquipped => EquippedPrimaryType != WeaponTypes.None;
    public bool HasSecondaryEquipped => EquippedSecondaryType != WeaponTypes.None;

    /// <summary>
    /// Retorna a arma prim�ria escolhida, caso exista no arsenal de prim�rias.
    /// </summary>
    public BaseWeapon EquippedPrimaryWeapon => PrimaryWeaponsArsenal.Find(w => w.Type == EquippedPrimaryType);
    /// <summary>
    /// Retorna a arma secund�ria escolhida, caso exista no arsenal de secund�rias.
    /// </summary>
    public BaseWeapon EquippedSecondaryWeapon => SecondaryWeaponsArsenal.Find(w => w.Type == EquippedSecondaryType);
    /// <summary>
    /// Retorna a arma atualmente equipada nas m�os do jogador.
    /// </summary>
    public BaseWeapon EquippedWeapon => CurrentWeaponIndex == 0 ? EquippedPrimaryWeapon : EquippedSecondaryWeapon;

    public Backpack(Player player, InventoryData data)
    {
        Player = player;
        Data = data;
        MaxThrowableType = 3;
        PrimaryWeaponsArsenal = new List<BaseWeapon>();
        SecondaryWeaponsArsenal = new List<BaseWeapon>();

        bool isEquippedPrimary = CurrentWeaponIndex == 0;

        if (EquippedPrimaryType != WeaponTypes.None)
        {
            var primWeapon = AddWeapon(EquippedPrimaryType);
            EquippedPrimaryWeapon.IsActive = isEquippedPrimary;
            if (!Data.HasWeapon(primWeapon.Type))
                Data.PrimaryWeaponsArsenalData.Add(primWeapon.Data);
        }

        if (EquippedSecondaryType != WeaponTypes.None)
        {
            var secWeapon = AddWeapon(EquippedSecondaryType);
            EquippedSecondaryWeapon.IsActive = !isEquippedPrimary;
            if (!Data.HasWeapon(secWeapon.Type))
                Data.SecondaryWeaponsArsenalData.Add(secWeapon.Data);
        }
    }

    /// <summary>
    /// Adiciona uma nova arma ao arsenal do jogador.
    /// </summary>
    /// <param name="weaponType">Tipo da nova arma a ser adicionada.</param>
    /// <param name="equip">Se essa arma deve ser equipada nos slots de Prim�ria ou Secund�ria, logo ap�s ser adicionada.</param>
    /// <returns>A inst�ncia da arma adicionada.</returns>
    public BaseWeapon AddWeapon(WeaponTypes weaponType, bool equip = true)
    {
        if (PrimaryWeaponsArsenal.Any(x => x.Type == weaponType) || SecondaryWeaponsArsenal.Any(x => x.Type == weaponType))
        {
            Debug.LogWarning($"Tentativa de adicionar arma {weaponType} ao arsenal do jogador, mas ela j� existe.");
            return null;
        }
        GameObject weaponObj = Player.WeaponController.InstantiateWeaponPrefab(weaponType);
        BaseWeapon weapon = weaponObj.GetComponent<BaseWeapon>();

        if (weapon.IsPrimary)
        {
            PrimaryWeaponsArsenal.Add(weapon);
            if (equip)
            {
                Data.EquippedPrimaryType = weaponType;
            }
        }
        else
        {
            SecondaryWeaponsArsenal.Add(weapon);
            if (equip)
            {
                Data.EquippedSecondaryType = weaponType;
            }
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
        BulletTypes.Throwable => MaxThrowableType,
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
}
