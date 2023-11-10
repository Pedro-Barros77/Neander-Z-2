using System.Collections.Generic;

public class NZSave
{
    #region MainInfo
    public int CurrentWave;
    public float PlayerTotalMoney;
    public float PlayerHealth;
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

    #region WavesStats
    public List<WaveStats.Data> WavesStats;
    #endregion

    public string FolderPath;
    public string FileName;
}
