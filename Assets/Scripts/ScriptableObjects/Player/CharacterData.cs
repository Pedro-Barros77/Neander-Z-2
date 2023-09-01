using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Neander Z/Player/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    public CharacterTypes Character;

    public float MaxHealth;

    public float MaxMovementSpeed;
    public float AccelerationSpeed;
    public float SprintSpeedMultiplier;

    public float JumpForce;
}
