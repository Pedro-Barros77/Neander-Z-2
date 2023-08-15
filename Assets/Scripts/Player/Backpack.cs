using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A mochila do jogador, carrega suas armas e acessórios.
/// </summary>
public class Backpack
{
    /// <summary>
    /// ´Define o nível de melhoria atual da mochila. Inicia em zero (sem upgrade).
    /// </summary>
    public int UpgradeStep { get; private set; }
    /// <summary>
    /// Número de munições de pistola restantes.
    /// </summary>
    public int PistolAmmo { get; private set; }
    /// <summary>
    /// Número de munições de escopeta restantes.
    /// </summary>
    public int ShotgunAmmo { get; private set; }
    /// <summary>
    /// Número de munições de fuzil restantes.
    /// </summary>
    public int RifleAmmo { get; private set; }
    /// <summary>
    /// Número de munições de sniper restantes.
    /// </summary>
    public int SniperAmmo { get; private set; }
    /// <summary>
    /// Número de munições de foguete restantes.
    /// </summary>
    public int RocketAmmo { get; private set; }
    //public List<Throwable> Throwables { get; set; }
    /// <summary>
    /// Lista de armas primárias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> PrimaryWeaponsArsenal { get; private set; }
    /// <summary>
    /// Lista de armas secundárias adquiridas pelo jogador.
    /// </summary>
    public List<BaseWeapon> SecondaryWeaponsArsenal { get; private set; }
    /// <summary>
    /// Número máximo de munições de pistola que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxPistolAmmo { get; private set; }
    /// <summary>
    /// Número máximo de munições de escopeta que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxShotgunAmmo { get; private set; }
    /// <summary>
    /// Número máximo de munições de fuzil que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxRifleAmmo { get; private set; }
    /// <summary>
    /// Número máximo de munições de sniper que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxSniperAmmo { get; private set; }
    /// <summary>
    /// Número máximo de munições de foguete que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxRocketAmmo { get; private set; }
    /// <summary>
    /// Número máximo de itens arremessáveis de cada tipo que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    public int MaxThrowableType { get; private set; }
    /// <summary>
    /// Referência ao jogador portador dessa mochila.
    /// </summary>
    public Player Player { get; private set; }

    /// <summary>
    /// Índice da arma atualmente equipada nas mãos do jogador (0= Primária, 1= Secundária).
    /// </summary>
    public int CurrentWeaponIndex { get; private set; }
    /// <summary>
    /// O tipo da arma atualmente escolhida como primária pelo jogador.
    /// </summary>
    public WeaponTypes EquippedPrimaryType { get; private set; }
    /// <summary>
    /// O tipo da arma atualmente escolhida como secundária pelo jogador.
    /// </summary>
    public WeaponTypes EquippedSecondaryType { get; private set; }

    /// <summary>
    /// Retorna a arma primária escolhida, caso exista no arsenal de primárias.
    /// </summary>
    public BaseWeapon EquippedPrimaryWeapon => PrimaryWeaponsArsenal.Find(w => w.Type == EquippedPrimaryType);
    /// <summary>
    /// Retorna a arma secundária escolhida, caso exista no arsenal de secundárias.
    /// </summary>
    public BaseWeapon EquippedSecondaryWeapon => SecondaryWeaponsArsenal.Find(w => w.Type == EquippedSecondaryType);
    /// <summary>
    /// Retorna a arma atualmente equipada nas mãos do jogador.
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
    /// <param name="equip">Se essa arma deve ser equipada nos slots de Primária ou Secundária, logo após ser adicionada.</param>
    /// <returns>A instância da arma adicionada.</returns>
    public BaseWeapon AddWeapon(WeaponTypes weaponType, bool equip = true)
    {
        if(PrimaryWeaponsArsenal.Any(x => x.Type == weaponType) || SecondaryWeaponsArsenal.Any(x => x.Type == weaponType))
        {
            Debug.LogWarning($"Tentativa de adicionar arma {weaponType} ao arsenal do jogador, mas ela já existe.");
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
    /// Retorna a quantidade de munições do tipo especificado restantes que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de munição a ser avaliado.</param>
    /// <returns>O número de munições restantes.</returns>
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
    /// Retorna a quantidade máxima de munições do tipo especificado que o jogador pode carregar, neste nível de upgrade da mochila.
    /// </summary>
    /// <param name="type">O tipo de munição a ser avaliado.</param>
    /// <returns>O número máximo de munições que podem ser carregadas.</returns>
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
    /// Define a quantidade de munições do tipo especificado que o jogador possui.
    /// </summary>
    /// <param name="type">O tipo de munição a ser definida.</param>
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
