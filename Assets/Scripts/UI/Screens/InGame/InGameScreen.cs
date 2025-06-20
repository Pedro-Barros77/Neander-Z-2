using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameScreen : MonoBehaviour
{
    [SerializeField]
    Player Player;
    [SerializeField]
    GameObject PausePanel, GameOverPanel, PopupPrefab;
    [SerializeField]
    Image ActiveWeaponImage, ActiveAmmoImage, ActiveThrowableImage, ActiveSupportEquipmentImage, SwitchWeaponImage, PlayerHeadImage;
    [SerializeField]
    TextMeshProUGUI MagazineBulletsText, TotalBulletsText, ThrowablesCountText, SupportEquipmentsCountText, PlayerMoneyText, WaveScoreText, PauseTitle, WaveSummaryCharacterNameText;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon, FuelAmmoIcon, TacticalRollIcon, DoubleJumpIcon, HealthSupplyIcon, AmmoSupplyIcon;
    [SerializeField]
    Sprite Colt_1911Sprite, ShortBarrelSprite, UZISprite, SV98Sprite, M16Sprite, RPGSprite, MacheteSprite, DeagleSprite, Beretta_93RSprite, ScarSprite, ChainsawSprite, Spas12Sprite, Kar98Sprite, FlameThrowerSprite, M79GrenadeLauncherSprite, AA12Sprite, ScarDebugSprite;
    [SerializeField]
    Sprite FragGrenadeSprite, MolotovSprite, KunaiSprite, ClaymoreSprite;
    [SerializeField]
    GameObject TutorialPanel, MobileInputPanel, PauseContent, OptionsContent;
    [SerializeField]
    Button BtnReady;

    [SerializeField]
    Joystick MobileMovementJoystick, MobileGrenadeJoystick;
    [SerializeField]
    BaseButton MobileReloadButton, MobileTacticalAbilityButton, MobileSwitchWeaponsButton, MobileTouchBackgroundFire, MobileSupportEquipmentButton, MobileInteractButton;
    [SerializeField]
    SkinManager PlayerHeadSkinManager, WaveSummaryPlayerHeadSkinManager;
    [SerializeField]
    ProgressBar ReloadProgressBar, TacticalAbilityCooldownBar, TacticalAbilityCooldownBarMobile;

    Image SprintThreshold;
    Color32 NotSprintingJoystickColor = new(212, 210, 159, 15), SprintingJoystickColor = new(255, 243, 73, 50);
    OptionsPanel OptionsPanel;
    AudioSource AudioSource;
    Canvas WorldPosCanvas;
    float musicStartVolume;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        musicStartVolume = AudioSource.volume;
        BtnReady.gameObject.SetActive(MenuController.Instance.IsTutorialActive);
        WorldPosCanvas = GameObject.Find("WorldPositionCanvas").GetComponent<Canvas>();
        TutorialPanel.SetActive(MenuController.Instance.IsTutorialActive);
        OptionsPanel = OptionsContent.GetComponent<OptionsPanel>();
        OptionsPanel.GoBackFunction += () =>
        {
            OptionsContent.SetActive(false);
            PauseContent.SetActive(true);
            PauseTitle.text = "Game Paused";
        };

        TacticalAbilityCooldownBar.SetMaxValue(1, 1);
        TacticalAbilityCooldownBar.UseShadows = false;
        TacticalAbilityCooldownBar.UseOutline = false;
        TacticalAbilityCooldownBar.UseAnimation = false;

        ReloadProgressBar.SetMaxValue(1, 1);
        ReloadProgressBar.UseShadows = false;
        ReloadProgressBar.UseOutline = false;
        ReloadProgressBar.UseAnimation = false;
        ReloadProgressBar.HideOnFull = true;

        PlayerHeadSkinManager.LoadSkinData(Player.Data.SkinData);
        WaveSummaryPlayerHeadSkinManager.LoadSkinData(Player.Data.SkinData);
        WaveSummaryCharacterNameText.text = Player.Data.SkinData.CharacterName;

        OptionsPanel.VolumeChangeFunction += HandleVolumeChange;
        AudioSource.volume = MenuController.Instance.MusicVolume;

        if (MenuController.Instance.IsTutorialActive)
            Player.Data.InventoryData = Resources.Load<InventoryData>("ScriptableObjects/Player/TutorialInventory");

        MobileInputPanel.SetActive(MenuController.Instance.IsMobileInput);

        Sprite tacticalAbilitySprite = Player.Data.InventoryData.TacticalAbilitiesSelection?.FirstOrDefault(x => x.IsEquipped)?.Type switch
        {
            TacticalAbilityTypes.TacticalRoll => TacticalRollIcon,
            TacticalAbilityTypes.DoubleJump => DoubleJumpIcon,
            _ => null,
        };

        if (MenuController.Instance.IsMobileInput)
        {
            MenuController.Instance.MobileMovementJoystick = MobileMovementJoystick;
            MenuController.Instance.MobileGrenadeJoystick = MobileGrenadeJoystick;
            MenuController.Instance.MobileReloadButton = MobileReloadButton;
            MenuController.Instance.MobileTacticalAbilityButton = MobileTacticalAbilityButton;
            MenuController.Instance.MobileSwitchWeaponsButton = MobileSwitchWeaponsButton;
            MenuController.Instance.MobileTouchBackgroundFire = MobileTouchBackgroundFire;
            MenuController.Instance.MobileSupportEquipmentButton = MobileSupportEquipmentButton;
            MenuController.Instance.MobileInteractButton = MobileInteractButton;

            SprintThreshold = MobileMovementJoystick.transform.Find("SprintThresholdMask").GetChild(0).GetComponent<Image>();

            TacticalAbilityCooldownBar.gameObject.SetActive(false);
            TacticalAbilityCooldownBarMobile.SetMaxValue(1, 1);
            TacticalAbilityCooldownBarMobile.UseShadows = false;
            TacticalAbilityCooldownBarMobile.UseOutline = false;
            TacticalAbilityCooldownBarMobile.UseAnimation = false;

            MobileTacticalAbilityButton.transform.Find("Icon").GetComponent<Image>().sprite = tacticalAbilitySprite;
        }

        TacticalAbilityCooldownBar.transform.Find("Icon").GetComponent<Image>().sprite = tacticalAbilitySprite;
    }

    void Update()
    {
        if ((Constants.GetActionDown(InputActions.PauseContinueGame)) && Player.IsAlive && !WavesManager.Instance.WaveSummaryPanel.activeSelf)
        {
            if (MenuController.Instance.IsGamePaused)
                ContinueGame();
            else
                PauseGame();
        }

        if (MenuController.Instance.IsMobileInput)
            SprintThreshold.color = Constants.GetAction(InputActions.Sprint) ? SprintingJoystickColor : NotSprintingJoystickColor;

        if (Constants.EnableDevKeybinds)
        {
            if (Constants.GetActionDown(InputActions.DEBUG_RefillAllAmmo))
            {
                Player.Data.InventoryData.SetAmmo(BulletTypes.Pistol, Player.Data.InventoryData.GetMaxAmmo(BulletTypes.Pistol));
                Player.Data.InventoryData.SetAmmo(BulletTypes.Shotgun, Player.Data.InventoryData.GetMaxAmmo(BulletTypes.Shotgun));
                Player.Data.InventoryData.SetAmmo(BulletTypes.AssaultRifle, Player.Data.InventoryData.GetMaxAmmo(BulletTypes.AssaultRifle));
                Player.Data.InventoryData.SetAmmo(BulletTypes.Sniper, Player.Data.InventoryData.GetMaxAmmo(BulletTypes.Sniper));
                Player.Data.InventoryData.SetAmmo(BulletTypes.Rocket, Player.Data.InventoryData.GetMaxAmmo(BulletTypes.Rocket));
                Player.Data.InventoryData.SetAmmo(BulletTypes.Fuel, Player.Data.InventoryData.GetMaxAmmo(BulletTypes.Fuel));
                if (Player.Backpack.EquippedThrowableType != ThrowableTypes.None)
                    Player.Backpack.EquippedThrowable.Count = Player.Backpack.EquippedThrowable.MaxCount;
                if(Player.Backpack.EquippedSupportEquipmentType != SupportEquipmentTypes.None)
                    Player.Backpack.EquippedSupportEquipment.Count = Player.Backpack.EquippedSupportEquipment.MaxCount;
            }
        }

        UpdateInGameUI();
    }

    private void HandleVolumeChange(AudioTypes audioType, float volume)
    {
        if (audioType == AudioTypes.Music)
            AudioSource.volume = musicStartVolume * volume;
    }

    /// <summary>
    /// Atualiza as informa��es na tela do jogo atual.
    /// </summary>
    private void UpdateInGameUI()
    {
        PlayerMoneyText.text = Player.Data.Money.ToString("N2");
        WaveScoreText.text = WavesManager.Instance.CurrentWave.Stats != null ? WavesManager.Instance.CurrentWave.Stats.Score.ToString("N0") : "";

        MagazineBulletsText.text = Player.CurrentWeapon.MagazineBullets.ToString();
        TotalBulletsText.text = Player.Backpack.GetAmmo(Player.CurrentWeapon.BulletType).ToString();
        if (Player.Backpack.EquippedThrowableType != ThrowableTypes.None)
            ThrowablesCountText.text = Player.Backpack.EquippedThrowable?.Count.ToString();
        if (Player.Backpack.EquippedSupportEquipmentType != SupportEquipmentTypes.None)
            SupportEquipmentsCountText.text = Player.Backpack.EquippedSupportEquipment?.Count.ToString();

        ActiveAmmoImage.sprite = Player.CurrentWeapon.BulletType switch
        {
            BulletTypes.Pistol => PistolBulletIcon,
            BulletTypes.Shotgun => ShotgunBulletIcon,
            BulletTypes.AssaultRifle => RifleAmmoIcon,
            BulletTypes.Sniper => SniperAmmoIcon,
            BulletTypes.Rocket => RocketAmmoIcon,
            BulletTypes.Melee => MeleeAmmoIcon,
            BulletTypes.Fuel => FuelAmmoIcon,
            _ => null,
        };

        if (Player.Backpack.EquippedThrowableType != ThrowableTypes.None)
            ActiveThrowableImage.sprite = Player.Backpack.EquippedThrowableType switch
            {
                ThrowableTypes.FragGrenade => FragGrenadeSprite,
                ThrowableTypes.Molotov => MolotovSprite,
                ThrowableTypes.Kunai => KunaiSprite,
                ThrowableTypes.Claymore => ClaymoreSprite,
                _ => null,
            };

        if (Player.Backpack.EquippedSupportEquipmentType != SupportEquipmentTypes.None)
            ActiveSupportEquipmentImage.sprite = Player.Backpack.EquippedSupportEquipmentType switch
            {
                SupportEquipmentTypes.AmmoSupply => AmmoSupplyIcon,
                SupportEquipmentTypes.HealthSupply => HealthSupplyIcon,
                _ => null,
            };

        Sprite GetWeaponSprite(WeaponTypes type) => type switch
        {
            WeaponTypes.Colt_1911 => Colt_1911Sprite,
            WeaponTypes.ShortBarrel => ShortBarrelSprite,
            WeaponTypes.UZI => UZISprite,
            WeaponTypes.SV98 => SV98Sprite,
            WeaponTypes.M16 => M16Sprite,
            WeaponTypes.RPG => RPGSprite,
            WeaponTypes.Machete => MacheteSprite,
            WeaponTypes.Deagle => DeagleSprite,
            WeaponTypes.Beretta_93R => Beretta_93RSprite,
            WeaponTypes.Scar => ScarSprite,
            WeaponTypes.Chainsaw => ChainsawSprite,
            WeaponTypes.Spas12 => Spas12Sprite,
            WeaponTypes.Kar98 => Kar98Sprite,
            WeaponTypes.FlameThrower => FlameThrowerSprite,
            WeaponTypes.M79GrenadeLauncher => M79GrenadeLauncherSprite,
            WeaponTypes.ScarDebug => ScarDebugSprite,
            WeaponTypes.AA12 => AA12Sprite,
            _ => null,
        };

        ActiveWeaponImage.sprite = GetWeaponSprite(Player.CurrentWeapon.Type);

        var switchType = Player.Backpack.CurrentWeaponIndex == 0 ? Player.Backpack.EquippedSecondaryType : Player.Backpack.EquippedPrimaryType;
        SwitchWeaponImage.transform.parent.gameObject.SetActive(switchType != WeaponTypes.None);
        SwitchWeaponImage.sprite = GetWeaponSprite(switchType);

        float reloadProgress = ((Player.CurrentWeapon.ReloadStartTime ?? 0) + (Player.CurrentWeapon.ReloadTimeMs / 1000) - Time.time)
            .MapRange(0, Player.CurrentWeapon.ReloadTimeMs / 1000, 0, 1);

        ReloadProgressBar.RemoveValue(1);
        ReloadProgressBar.AddValue(1 - reloadProgress);

        switch (Player.Backpack.EquippedTacticalAbilityType)
        {
            case TacticalAbilityTypes.TacticalRoll:
                float rollCooldownProgress = (Player.PlayerMovement.LastRollTime + (Player.RollCooldownMs / 1000) - Time.time)
                    .MapRange(0, Player.RollCooldownMs / 1000, 0, 1);
                if (MenuController.Instance.IsMobileInput)
                {
                    TacticalAbilityCooldownBarMobile.RemoveValue(1);
                    TacticalAbilityCooldownBarMobile.AddValue(1 - rollCooldownProgress);
                }
                else
                {
                    TacticalAbilityCooldownBar.RemoveValue(1);
                    TacticalAbilityCooldownBar.AddValue(1 - rollCooldownProgress);
                }
                break;

            case TacticalAbilityTypes.DoubleJump:
                if (Player.PlayerMovement.IsGrounded || Player.PlayerMovement.DoubleJumped)
                    TacticalAbilityCooldownBar.RemoveValue(1);
                else
                    TacticalAbilityCooldownBar.AddValue(1);
                break;
        }
    }

    /// <summary>
    /// Troca para a cena da loja ap�s o tutorial conclu�do.
    /// </summary>
    public void ExitTutorial()
    {
        MenuController.Instance.IsTutorialActive = false;
        MenuController.Instance.OnRestartGame();

        OpenStore();
    }

    /// <summary>
    /// Muda a cena para a loja do jogo.
    /// </summary>
    public void OpenStore()
    {
        ContinueGame();
        MenuController.Instance.ChangeScene(SceneNames.Store, LoadSceneMode.Single);
    }

    /// <summary>
    /// Pausa o jogo, congelando o tempo e exibindo o menu de pausa.
    /// </summary>
    public void PauseGame()
    {
        if (!MenuController.Instance.CanPause)
            return;
        MenuController.Instance.PauseGame();
        PausePanel.SetActive(true);
    }

    /// <summary>
    /// Continua o jogo, descongelando o tempo e escondendo o menu de pausa.
    /// </summary>
    public void ContinueGame()
    {
        MenuController.Instance.ContinueGame();
        OptionsPanel.Open();
        OptionsPanel.GoBack();
        PausePanel.SetActive(false);
        OptionsContent.SetActive(false);
        PauseContent.SetActive(true);
        OptionsPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Salva o progresso do jogo.
    /// </summary>
    public void SaveGame()
    {
        if (SavesManager.SaveGame(GameModes.WaveMastery, SavesManager.SelectedSaveName))
            ShowPopup("Game progress saved!", Constants.Colors.GreenMoney, BtnReady.transform.position + new Vector3(10, 40));
        else
            ShowPopup("Failed to save game progress!", Color.red, BtnReady.transform.position + new Vector3(10, 40));

        if (!SavesManager.SavePrefs())
            ShowPopup("Failed to save preferences!", Color.red, BtnReady.transform.position + new Vector3(10, 40));
    }

    /// <summary>
    /// Inicia a pr�xima wave.
    /// </summary>
    public void NextWave()
    {
        ContinueGame();
        WavesManager.Instance.StartNextWave();
    }

    /// <summary>
    /// Reinicia o jogo, recarregando a cena atual.
    /// </summary>
    public void RestartGame()
    {
        GameOverPanel.SetActive(false);
        if (MenuController.Instance.IsTutorialActive)
        {
            ContinueGame();
            MenuController.Instance.RestartScene();
        }
        else
            OpenStore();
        MenuController.Instance.OnRestartGame();
        SavesManager.LoadSavedGame(GameModes.WaveMastery, SavesManager.SelectedSaveName);
    }

    /// <summary>
    /// Abre o painel de op��es.
    /// </summary>
    public void OpenOptions()
    {
        OptionsContent.SetActive(true);
        PauseContent.SetActive(false);
        OptionsPanel.Open();
    }

    /// <summary>
    /// Sai do jogo, voltando para o menu principal.
    /// </summary>
    public void QuitGame()
    {
        GameOverPanel.SetActive(false);
        ContinueGame();
        MenuController.Instance.IsInGame = false;
        MenuController.Instance.ChangeScene(SceneNames.MainMenu, LoadSceneMode.Single);
        MenuController.Instance.OnRestartGame();
    }

    /// <summary>
    /// Mostra o painel de fim de jogo.
    /// </summary>
    public void ShowGameOverPanel()
    {
        MenuController.Instance.ShowGameOverPanel();
        GameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Exibe um Popup na tela.
    /// </summary>
    /// <param name="text">O texto a ser exibido.</param>
    /// <param name="textColor">A cor do texto a ser exibido.</param>
    /// <param name="position">A posi��o do texto a ser exibido.</param>
    private void ShowPopup(string text, Color32 textColor, Vector3 position)
    {
        var popup = Instantiate(PopupPrefab, position, Quaternion.identity, WorldPosCanvas.transform);
        var popupSystem = popup.GetComponent<PopupSystem>();
        if (popupSystem != null)
        {
            popupSystem.Init(text, position, 2000f, textColor);
        }
    }
}
