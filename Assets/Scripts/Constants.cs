using System.Linq;
using UnityEngine;

public static class Constants
{
    public static bool EnableDevKeybinds { get; private set; } = true;
    public static bool FormatSaveFiles { get; private set; } = true;
    public static float MaxWeaponDamage { get; private set; } = 60; // half section = 2.5
    public static float MaxWeaponFireRate { get; private set; } = 18; // half section = 0.75
    public static float MaxWeaponReloadSpeed { get; private set; } = 12; // half section = 0.5
    public static float MaxWeaponRange { get; private set; } = 24; // half section = 1
    public static float MaxWeaponBulletSpeed { get; private set; } = 60; // half section = 2.5

    public static float MaxPlayerMaxHealth { get; private set; } = 360; // half section = 15
    public static float MaxPlayerMovementSpeed { get; private set; } = 5.04f; // half section = 0.21
    public static float MaxPlayerSprintSpeed { get; private set; } = 2.16f; // half section = 0.09
    public static float MaxPlayerJumpForce { get; private set; } = 2208; // half section = 92
    public static float MaxPlayerMaxStamina { get; private set; } = 480; // half section = 20
    public static float MaxPlayerStaminaRegen { get; private set; } = 60; // half section = 2.5
    public static float MaxPlayerStaminaHaste { get; private set; } = 12f; // half section = 0.5
    public static float MaxPlayerJumpStamina { get; private set; } = 48; // half section = 2
    public static float MaxPlayerSprintStamina { get; private set; } = 120; // half section = 5
    public static float MaxPlayerAttackStamina { get; private set; } = 120; // half section = 5

    //Crouch Recovery
    public static float CrouchRecoveryHealthTick { get; private set; } = 1f;
    public static float CrouchRecoveryTickIntervalMs { get; private set; } = 500f;

    //Cautious 
    public static float CautiousDamageMultiplier { get; private set; } = 0.2f;

    //Second Chance
    public static float SecondChanceStaminaDrainMultiplier { get; private set; } = 0.2f;
    public static float SecondChanceDamageTakenMultiplier { get; private set; } = 0.85f;
    public static float SecondChanceHealthTrheshold { get; private set; } = 0.3f;
    public static float SecondChanceDurationMs { get; private set; } = 5000f;

    //Precision Bounty
    public static float PrecisionBountyPistolChance { get; private set; } = 0.20f;
    public static float PrecisionBountyShotgunChance { get; private set; } = 0.12f;
    public static float PrecisionBountyAssaultRifleChance { get; private set; } = 0.15f;
    public static float PrecisionBountySniperChance { get; private set; } = 0.12f;
    public static float PrecisionBountyRocketChance { get; private set; } = 0.10f;
    public static int PrecisionBountyPistolCount { get; private set; } = 6;
    public static int PrecisionBountyShotgunCount { get; private set; } = 3;
    public static int PrecisionBountyAssaultRifleCount { get; private set; } = 5;
    public static int PrecisionBountySniperCount { get; private set; } = 3;
    public static int PrecisionBountyRocketCount { get; private set; } = 1;

    static float ReloadSpeedRatio = 5000;
    static float JoystickHorizontalDeadzone = 0.2f;
    static float JoystickSprintDeadzone = 0.9f;
    static float JoystickVerticalDeadzone = 0.5f;

    public static bool GetActionDown(InputActions action) => GetActionPerformed(action, 0);
    public static bool GetAction(InputActions action) => GetActionPerformed(action, 1);
    public static bool GetActionUp(InputActions action) => GetActionPerformed(action, 2);

    private static bool GetActionPerformed(InputActions action, int pressingState)
    {
        if (MenuController.Instance.Keybind == null)
            return false;

        if (MenuController.Instance.Keybind.Inputs == null)
            MenuController.Instance.Keybind.Init();

        if (!MenuController.Instance.IsMobileInput)
        {
            if (action == InputActions.SwitchWeapon && pressingState == 0)
                return Input.mouseScrollDelta.y != 0;

            return MenuController.Instance.Keybind.Inputs.Any(inp => inp.Any(keybind => keybind.Action == action
                && (
                           (pressingState == 0 && Input.GetKeyDown(keybind.Key))
                        || (pressingState == 1 && Input.GetKey(keybind.Key))
                        || (pressingState == 2 && Input.GetKeyUp(keybind.Key))
                   )));
        }

        bool ButtonState(BaseButton button) => pressingState switch
        {
            0 => button.Pressed,
            1 => button.IsPressing,
            2 => button.Released,
            _ => false
        };

        switch (action)
        {
            case InputActions.MoveLeft:
                if (pressingState == 0 || pressingState == 1)
                    return MenuController.Instance.MobileMovementJoystick.Horizontal <= -JoystickHorizontalDeadzone;
                break;
            case InputActions.MoveRight:
                if (pressingState == 0 || pressingState == 1)
                    return MenuController.Instance.MobileMovementJoystick.Horizontal >= JoystickHorizontalDeadzone;
                break;
            case InputActions.Jump:
                if (pressingState == 0)
                    return MenuController.Instance.MobileMovementJoystick.Vertical >= JoystickVerticalDeadzone;
                break;
            case InputActions.Crouch:
                if (pressingState == 0 || pressingState == 1)
                    return MenuController.Instance.MobileMovementJoystick.Vertical <= -JoystickVerticalDeadzone;
                break;
            case InputActions.Sprint:
                if (pressingState == 0 || pressingState == 1)
                    return Mathf.Abs(MenuController.Instance.MobileMovementJoystick.Horizontal) >= JoystickSprintDeadzone;
                break;
            case InputActions.Reload:
                return ButtonState(MenuController.Instance.MobileReloadButton);

            case InputActions.SwitchWeapon:
                return ButtonState(MenuController.Instance.MobileSwitchWeaponsButton);

            case InputActions.TacticalAbility:
                return ButtonState(MenuController.Instance.MobileTacticalAbilityButton);

            case InputActions.Shoot:
                return ButtonState(MenuController.Instance.MobileTouchBackgroundFire);

            case InputActions.ThrowGrenade:
                if (pressingState == 0 || pressingState == 1)
                    return MenuController.Instance.MobileGrenadeJoystick.Vertical != 0 || MenuController.Instance.MobileGrenadeJoystick.Horizontal != 0;
                else
                    return MenuController.Instance.MobileGrenadeJoystick.Vertical == 0 && MenuController.Instance.MobileGrenadeJoystick.Horizontal == 0;
        }

        return false;
    }


    /// <summary>
    /// Define se a arma é primária ou secundária.
    /// </summary>
    /// <param name="weaponType">O tipo da arma.</param>
    /// <returns>Se a arma é primária, caso contrário, secundária.</returns>
    public static bool IsPrimary(WeaponTypes weaponType)
    {
        return weaponType switch
        {
            WeaponTypes.Machete => false,
            WeaponTypes.Colt_1911 => false,
            WeaponTypes.Deagle => false,
            WeaponTypes.Beretta_93R => false,

            WeaponTypes.ShortBarrel => true,
            WeaponTypes.UZI => true,
            WeaponTypes.RPG => true,
            WeaponTypes.SV98 => true,
            WeaponTypes.M16 => true,
            WeaponTypes.Scar => true,
            WeaponTypes.ScarDebug => true,

            _ => false,
        };
    }

    /// <summary>
    /// Calcula o dano dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>O Dano calculado.</returns>
    public static float CalculateDamage(BaseWeaponData weaponData)
    {
        float damage = weaponData.Damage;
        if (weaponData is ShotgunData shotgunData)
            damage = shotgunData.Damage * shotgunData.ShellPelletsCount;

        return damage;
    }

    /// <summary>
    /// Calcula a cadência de tiro dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>A cadência de tiro calculada.</returns>
    public static float CalculateFireRate(BaseWeaponData weaponData)
    {
        float fireRate = weaponData.FireRate;

        switch (weaponData.FireMode)
        {
            case FireModes.Burst:
                if (weaponData is BurstFireData burstData)
                    fireRate = (burstData.FireRate + burstData.BurstFireRate) / 2;
                break;
        }

        return fireRate;
    }

    /// <summary>
    /// Calcula a velocidade de recarga dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>A velocidade de recarga calculada.</returns>
    public static float CalculateReloadSpeed(BaseWeaponData weaponData)
    {
        float reloadSpeed = ReloadSpeedRatio / weaponData.ReloadTimeMs;

        switch (weaponData.ReloadType)
        {
            case ReloadTypes.NoReload:
                reloadSpeed = 0;
                break;
            case ReloadTypes.SingleBullet:
                reloadSpeed = ReloadSpeedRatio / (weaponData.ReloadTimeMs * weaponData.MagazineSize);
                break;
        }

        return reloadSpeed;
    }

    /// <summary>
    /// Calcula o alcance dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>O alcance calculado.</returns>
    public static float CalculateRange(BaseWeaponData weaponData)
    {
        float range = (weaponData.MinDamageRange + weaponData.MaxDamageRange + weaponData.BulletMaxRange) / 3;
        if (weaponData is MeleeData || weaponData.BulletType == BulletTypes.Melee || weaponData.FireMode == FireModes.Melee)
            range = 0;

        return range;
    }

    /// <summary>
    /// Calcula a velocidade dos projéteis dessa arma para exibir na barra de estatística, levando em consideração as variações de cada arma.
    /// </summary>
    /// <param name="weaponData">As informações dessa arma.</param>
    /// <returns>A velocidade de projétil calculada calculado.</returns>
    public static float CalculateBulletSpeed(BaseWeaponData weaponData)
    {
        float speed = weaponData.BulletSpeed;
        if (weaponData is MeleeData || weaponData.BulletType == BulletTypes.Melee || weaponData.FireMode == FireModes.Melee)
            speed = 0;

        return speed;
    }

    /// <summary>
    /// Calcula uma cor baseada em um valor e um máximo, com uma porcentagem de breakpoint.
    /// </summary>
    /// <param name="value">O valor real.</param>
    /// <param name="max">O valor máximo.</param>
    /// <param name="percentageBreakpoint">A porcentagem de breakpoint (onde a cor deixa de ser a full e passa a ser warning).</param>
    /// <param name="fullColor">A cor quando o valor está acima do breakpoint.</param>
    /// <param name="warningColor">A cor quando o valor está abaixo do breakpoint.</param>
    /// <param name="zeroColor">A cor quando o valor for zero.</param>
    /// <returns></returns>
    public static Color32 GetAlertColor(int value, int max, float percentageBreakpoint, Color32? fullColor = null, Color32? warningColor = null, Color32? zeroColor = null)
    {
        if (value == 0)
            return zeroColor ?? Colors.RedMoney;

        var percentage = (float)value / max;

        if (percentage > percentageBreakpoint)
            return fullColor ?? Color.white;

        return warningColor ?? Colors.YellowAmmo;
    }

    public readonly struct Colors
    {
        public static readonly Color32 GreenMoney = new(72, 164, 80, 255);
        public static readonly Color32 RedMoney = new(205, 86, 99, 255);
        public static readonly Color32 YellowAmmo = new(230, 220, 80, 255);
        public static readonly Color32 SelectedOptionColor = new(70, 230, 130, 255);
        public static readonly Color32 UnselectedOptionColor = new(230, 230, 230, 255);
    }
}
