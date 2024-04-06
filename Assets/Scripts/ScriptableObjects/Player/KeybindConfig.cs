
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Keybind Config Data", menuName = "Neander Z/Player/Keybind Config Data", order = 3)]
public class KeybindConfig : AutoRevertSO
{
    public List<List<KeybindItem>> Inputs;

    public void Init()
    {
        Inputs = new()
        {
            MoveLeft, MoveRight, Jump, Crouch, Sprint, Shoot, ThrowGrenade, TacticalAbility, Reload, PauseContinueGame, SwitchWeapon, EquipPrimaryWeapon, EquipSecondaryWeapon, BuyMaxStoreItems,
            DEBUG_IncreaseHealth, DEBUG_DecreaseHealth, DEBUG_IncreaseMoney, DEBUG_DecreaseMoney, DEBUG_SpawnRoger, DEBUG_SpawnRonald, DEBUG_SpawnRonaldo, DEBUG_SpawnRaven, DEBUG_SpawnRobert, DEBUG_SpawnRaimundo, DEBUG_SpawnRUI, DEBUG_SpawnRute, DEBUG_SpawnRat, DEBUG_KillAllEnemiesAlive, DEBUG_EndWave, DEBUG_CenterEnemies, DEBUG_RefillAllAmmo
        };
    }

    public List<KeybindItem> MoveLeft;
    public List<KeybindItem> MoveRight;
    public List<KeybindItem> Jump;
    public List<KeybindItem> Crouch;
    public List<KeybindItem> Sprint;
    public List<KeybindItem> Shoot;
    public List<KeybindItem> ThrowGrenade;
    public List<KeybindItem> TacticalAbility;
    public List<KeybindItem> Reload;
    public List<KeybindItem> PauseContinueGame;
    public List<KeybindItem> SwitchWeapon;
    public List<KeybindItem> EquipPrimaryWeapon;
    public List<KeybindItem> EquipSecondaryWeapon;
    public List<KeybindItem> BuyMaxStoreItems;

    #region DEBUG (Constants.EnableDevKeybinds == true)
    public List<KeybindItem> DEBUG_IncreaseHealth;
    public List<KeybindItem> DEBUG_DecreaseHealth;
    public List<KeybindItem> DEBUG_IncreaseMoney;
    public List<KeybindItem> DEBUG_DecreaseMoney;

    public List<KeybindItem> DEBUG_SpawnRoger;
    public List<KeybindItem> DEBUG_SpawnRonald;
    public List<KeybindItem> DEBUG_SpawnRonaldo;
    public List<KeybindItem> DEBUG_SpawnRaven;
    public List<KeybindItem> DEBUG_SpawnRobert;
    public List<KeybindItem> DEBUG_SpawnRaimundo;
    public List<KeybindItem> DEBUG_SpawnRUI;
    public List<KeybindItem> DEBUG_SpawnRat;
    public List<KeybindItem> DEBUG_KillAllEnemiesAlive;
    public List<KeybindItem> DEBUG_EndWave;
    public List<KeybindItem> DEBUG_CenterEnemies;
    public List<KeybindItem> DEBUG_RefillAllAmmo;
    public List<KeybindItem> DEBUG_SpawnRute;
    #endregion

    [Serializable]
    public class KeybindItem
    {
        public InputActions Action;
        public KeyCode Key;
        public bool IsListening;
    }
}
