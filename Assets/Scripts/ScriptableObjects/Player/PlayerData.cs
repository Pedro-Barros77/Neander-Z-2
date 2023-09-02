using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Neander Z/Player/Player Data", order = 2)]
public class PlayerData : ScriptableObject
{
    public InventoryData InventoryData;

    public CharacterTypes Character;

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
