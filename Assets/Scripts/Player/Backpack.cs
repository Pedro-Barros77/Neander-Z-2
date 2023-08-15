using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A mochila do jogador, carrega suas armas e acess�rios.
/// </summary>
public class Backpack
{
    /// <summary>
    /// �Define o n�vel de melhoria atual da mochila. Inicia em zero (sem upgrade).
    /// </summary>
    public int UpgradeStep { get; private set; }
    /// <summary>
    /// N�mero de muni��es de pistola restantes.
    /// </summary>
    public int PistolAmmo { get; private set; }
    /// <summary>
    /// N�mero de muni��es de escopeta restantes.
    /// </summary>
    public int ShotgunAmmo { get; private set; }
    /// <summary>
    /// N�mero de muni��es de fuzil restantes.
    /// </summary>
    public int RifleAmmo { get; private set; }
    /// <summary>
    /// N�mero de muni��es de sniper restantes.
    /// </summary>
    public int SniperAmmo { get; private set; }
    /// <summary>
    /// N�mero de muni��es de foguete restantes.
    /// </summary>
    public int RocketAmmo { get; private set; }
    //public List<Throwable> Throwables { get; set; }
    /// <summary>
    /// Lista de armas prim�rias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> PrimaryWeaponsArsenal { get; private set; }
    /// <summary>
    /// Lista de armas secund�rias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> SecondaryWeaponsArsenal { get; private set; }
    /// <summary>
    /// N�mero m�ximo de muni��es de pistola que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxPistolAmmo { get; private set; }
    /// <summary>
    /// N�mero m�ximo de muni��es de escopeta que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxShotgunAmmo { get; private set; }
    /// <summary>
    /// N�mero m�ximo de muni��es de fuzil que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxRifleAmmo { get; private set; }
    /// <summary>
    /// N�mero m�ximo de muni��es de sniper que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxSniperAmmo { get; private set; }
    /// <summary>
    /// N�mero m�ximo de muni��es de foguete que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxRocketAmmo { get; private set; }
    /// <summary>
    /// N�mero m�ximo de itens arremess�veis de cada tipo que o jogador pode carregar, neste n�vel de upgrade da mochila.
    /// </summary>
    public int MaxThrowableType { get; private set; }
    /// <summary>
    /// Refer�ncia ao jogador portador dessa mochila.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// �ndice da arma atualmente equipada nas m�os do jogador (0= Prim�ria, 1= Secund�ria).
    /// </summary>
    public int CurrentWeaponIndex { get; private set; }
    /// <summary>
    /// O tipo da arma atualmente escolhida como prim�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedPrimaryType { get; private set; }
    /// <summary>
    /// O tipo da arma atualmente escolhida como secund�ria pelo jogador.
    /// </summary>
    public WeaponTypes EquippedSecondaryType { get; private set; }

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

    public Backpack(Player player)
    {
        PistolAmmo = 30;
        ShotgunAmmo = 0;
        RifleAmmo = 0;
        SniperAmmo = 0;
        RocketAmmo = 0;

        MaxPistolAmmo = 50;
        MaxShotgunAmmo = 20;
        MaxRifleAmmo = 90;
        MaxSniperAmmo = 15;
        MaxRocketAmmo = 5;
        MaxThrowableType = 3;
        PrimaryWeaponsArsenal = new List<BaseWeapon>();
        SecondaryWeaponsArsenal = new List<BaseWeapon>();
        Player = player;
    }

    /// <summary>
    /// Adiciona uma nova arma ao arsenal do jogador.
    /// </summary>
    /// <param name="weaponType">Tipo da nova arma a ser adicionada.</param>
    /// <param name="equip">Se essa arma deve ser equipada nos slots de Prim�ria ou Secund�ria, logo ap�s ser adicionada.</param>
    /// <returns>A inst�ncia da arma adicionada.</returns>
    public BaseWeapon AddWeapon(WeaponTypes weaponType, bool equip = true)
    {
        if(PrimaryWeaponsArsenal.Any(x => x.Type == weaponType) || SecondaryWeaponsArsenal.Any(x => x.Type == weaponType))
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
                EquippedPrimaryType = weaponType;
                CurrentWeaponIndex = 0;
            }
        }
        else
        {
            SecondaryWeaponsArsenal.Add(weapon);
            if (equip)
            {
                EquippedSecondaryType = weaponType;
                CurrentWeaponIndex = 1;
            }
        }

        return weapon;
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
                PistolAmmo = count;
                break;
            case BulletTypes.Shotgun:
                ShotgunAmmo = count;
                break;
            case BulletTypes.AssaultRifle:
                RifleAmmo = count;
                break;
            case BulletTypes.Sniper:
                SniperAmmo = count;
                break;
            case BulletTypes.Rocket:
                RocketAmmo = count;
                break;
            default:
                break;
        }
    }
}
