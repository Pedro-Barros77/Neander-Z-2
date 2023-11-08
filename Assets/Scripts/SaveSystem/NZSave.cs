using System.Collections.Generic;

public class NZSave
{
    #region MainInfo
    public int CurrentWave;
    public float PlayerTotalMoney;
    public float PlayerHealth;
    #endregion

    #region GameplayStats
    public float TotalScore;
    public float TotalGameTime;
    public float TotalInStoreTime;
    public int WavesRestarted;
    public int TotalKills;
    public int TotalHeadshotKills;
    public float TotalPrecision;
    #endregion

    #region Inventory
    public int PistolAmmo;
    public int ShotgunAmmo;
    public int RifleAmmo;
    public int SniperAmmo;
    public int RocketAmmo;
    public int BackpackUpgradeIndex;
    public List<InventoryData.WeaponSelection> PrimaryWeaponsSelection;
    public List<InventoryData.WeaponSelection> SecondaryWeaponsSelection;
    public List<InventoryData.ThrowableSelection> ThrowableItemsSelection;
    public List<InventoryData.TacticalAbilitySelection> TacticalAbilitiesSelection;
    #endregion

    #region PlayerStats
    public int MaxHealthUpgradeIndex;
    public int MovementSpeedUpgradeIndex;
    public int SprintSpeedUpgradeIndex;
    public int JumpForceUpgradeIndex;
    public int MaxStaminaUpgradeIndex;
    public int StaminaRegenUpgradeIndex;
    public int StaminaHasteUpgradeIndex;
    public int JumpStaminaUpgradeIndex;
    public int SprintStaminaUpgradeIndex;
    public int AttackStaminaUpgradeIndex;
    #endregion

    public string FolderPath;
    public string FileName;
}
