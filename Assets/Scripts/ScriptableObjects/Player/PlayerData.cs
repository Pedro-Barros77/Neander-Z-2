using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Neander Z/Player/Player Data", order = 1)]
public class PlayerData : AutoRevertSO
{
    public InventoryData InventoryData;

    public SkinData SkinData;

    public float MaxHealth;
    public float Health;

    public float MaxMovementSpeed;
    public float MovementSpeed;
    public float AccelerationSpeed;
    public float SprintSpeedMultiplier;

    public float JumpForce;
    public float RollForce;

    public float MaxStamina;
    public float Stamina;
    public float StaminaRegenDelayMs;
    public float StaminaRegenRate;

    public float SprintStaminaDrain;
    public float JumpStaminaDrain;
    public float AttackStaminaDrain;
    public float RollStaminaDrain;

    public float RollCooldownMs;

    public float Score;
    public float Money;
    public int CurrentWaveIndex;

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

    public int GetUpgradeIndex(PlayerAttributes attribute) => attribute switch
    {
        PlayerAttributes.MaxHealth => MaxHealthUpgradeIndex,
        PlayerAttributes.MovementSpeed => MovementSpeedUpgradeIndex,
        PlayerAttributes.SprintSpeed => SprintSpeedUpgradeIndex,
        PlayerAttributes.JumpForce => JumpForceUpgradeIndex,
        PlayerAttributes.MaxStamina => MaxStaminaUpgradeIndex,
        PlayerAttributes.StaminaRegen => StaminaRegenUpgradeIndex,
        PlayerAttributes.StaminaHaste => StaminaHasteUpgradeIndex,
        PlayerAttributes.JumpStamina => JumpStaminaUpgradeIndex,
        PlayerAttributes.SprintStamina => SprintStaminaUpgradeIndex,
        PlayerAttributes.AttackStamina => AttackStaminaUpgradeIndex,
        _ => 0
    };

    public void GetMoney(float value)
    {
        if (value < 0) return;

        Money += value;
    }

    public void TakeMoney(float value)
    {
        if (value < 0) return;

        Money -= value;
    }
}
