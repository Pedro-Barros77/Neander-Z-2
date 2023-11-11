using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Neander Z/Store/Player Data", order = 8)]
public class StorePlayerData : StoreItemData
{
    public List<PlayerUpgrade> MaxHealthUpgrades;
    public List<PlayerUpgrade> MovementSpeedUpgrades;
    public List<PlayerUpgrade> SprintSpeedUpgrades;
    public List<PlayerUpgrade> JumpForceUpgrades;
    public List<PlayerUpgrade> MaxStaminaUpgrades;
    public List<PlayerUpgrade> StaminaRegenUpgrades;
    public List<PlayerUpgrade> StaminaHasteUpgrades;
    public List<PlayerUpgrade> JumpStaminaUpgrades;
    public List<PlayerUpgrade> SprintStaminaUpgrades;
    public List<PlayerUpgrade> AttackStaminaUpgrades;

    public List<PlayerUpgrade> GetPlayerUpgrades(PlayerAttributes attribute) => attribute switch
    {
        PlayerAttributes.MaxHealth => MaxHealthUpgrades,
        PlayerAttributes.MovementSpeed => MovementSpeedUpgrades,
        PlayerAttributes.SprintSpeed => SprintSpeedUpgrades,
        PlayerAttributes.JumpForce => JumpForceUpgrades,
        PlayerAttributes.MaxStamina => MaxStaminaUpgrades,
        PlayerAttributes.StaminaRegen => StaminaRegenUpgrades,
        PlayerAttributes.StaminaHaste => StaminaHasteUpgrades,
        PlayerAttributes.JumpStamina => JumpStaminaUpgrades,
        PlayerAttributes.SprintStamina => SprintStaminaUpgrades,
        PlayerAttributes.AttackStamina => AttackStaminaUpgrades,
        _ => null
    };

    [Serializable]
    public class PlayerUpgrade
    {
        public float Price;
        public float Value;
    }
}
