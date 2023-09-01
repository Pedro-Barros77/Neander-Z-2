using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Neander Z/Player/Player Data", order = 2)]
public class PlayerData : ScriptableObject
{
    public CharacterTypes Character;

    public float MaxHealth;
    public float Health;

    public float MaxMovementSpeed;
    public float MovementSpeed;
    public float AccelerationSpeed;
    public float SprintSpeedMultiplier;

    public float JumpForce;
    public float RollForce;

    public float RollCooldownMs;

    public float Score;
    public float Money;
}
