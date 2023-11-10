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
    BuyMaxStoreItems = 14,

    DEBUG_IncreaseHealth = 100,
    DEBUG_DecreaseHealth = 101,
    DEBUG_IncreaseMoney = 102,
    DEBUG_DecreaseMoney = 103,
    DEBUG_SpawnRoger = 104,
    DEBUG_SpawnRonald = 105,
    DEBUG_SpawnRonaldo = 106,
    DEBUG_SpawnRaven = 107,
    DEBUG_SpawnRobert = 108,
    DEBUG_SpawnRaimundo = 109,
    DEBUG_SpawnRUI = 110,
    DEBUG_KillAllEnemiesAlive = 111,
    DEBUG_EndWave = 112,
    DEBUG_CenterEnemies = 113,
    DEBUG_RefillAllAmmo = 114
}