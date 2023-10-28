using UnityEngine;
/// <summary>
/// Tipos de ações realizadas pelas entradas do jogador.
/// </summary>
public enum InputActions
{
    None = 0,
    MoveLeft = 1,
    MoveRight = 2,
    Jump = 3,
    Crouch = 4,
    Sprint = 5,
    Shoot = 6,
    ThrowGrenade = 7,
    TacticalAbility = 8,
    Reload = 9,
    PauseContinueGame = 10,
    SwitchWeapon = 11,
    EquipPrimaryWeapon = 12,
    EquipSecondaryWeapon = 13,

    DEBUG_IncreaseHealth,
    DEBUG_DecreaseHealth,
    DEBUG_IncreaseMoney,
    DEBUG_DecreaseMoney,
    DEBUG_SpawnRoger,
    DEBUG_SpawnRonald,
    DEBUG_SpawnRonaldo,
    DEBUG_SpawnRaven,
    DEBUG_SpawnRobert,
    DEBUG_SpawnRaimundo,
    DEBUG_SpawnRUI,
    DEBUG_KillAllEnemiesAlive,
    DEBUG_EndWave,
    DEBUG_CenterEnemies,
    DEBUG_RefillAllAmmo
}