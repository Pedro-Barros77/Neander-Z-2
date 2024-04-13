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
    Image ActiveWeaponImage, ActiveAmmoImage, ActiveThrowableImage, SwitchWeaponImage, PlayerHeadImage;
    [SerializeField]
    TextMeshProUGUI MagazineBulletsText, TotalBulletsText, ThrowablesCountText, PlayerMoneyText, WaveScoreText, PauseTitle, WaveSummaryCharacterNameText;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon, FuelAmmoIcon;
    [SerializeField]
    Sprite Colt_1911Sprite, ShortBarrelSprite, UZISprite, SV98Sprite, M16Sprite, RPGSprite, MacheteSprite, DeagleSprite, Beretta_93RSprite, ScarSprite, ChainsawSprite, ScarDebugSprite;
    [SerializeField]
    Sprite FragGrenadeSprite, MolotovSprite;
    [SerializeField]
    GameObject TutorialPanel, MobileInputPanel, PauseContent, OptionsContent;
    [SerializeField]
    Button BtnReady;

    [SerializeField]
    Joystick MobileMovementJoystick, MobileGrenadeJoystick;
    [SerializeField]
    BaseButton MobileReloadButton, MobileTacticalAbilityButton, MobileSwitchWeaponsButton, MobileTouchBackgroundFire;
    [SerializeField]
    SkinManager PlayerHeadSkinManager, WaveSummaryPlayerHeadSkinManager;

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

        PlayerHeadSkinManager.LoadSkinData(Player.Data.SkinData);
        WaveSummaryPlayerHeadSkinManager.LoadSkinData(Player.Data.SkinData);
        WaveSummaryCharacterNameText.text = Player.Data.SkinData.CharacterName;

        OptionsPanel.VolumeChangeFunction += HandleVolumeChange;
        AudioSource.volume = MenuController.Instance.MusicVolume;

        if (MenuController.Instance.IsTutorialActive)
            Player.Data.InventoryData = Resources.Load<InventoryData>("ScriptableObjects/Player/TutorialInventory");

        MobileInputPanel.SetActive(MenuController.Instance.IsMobileInput);

        if (MenuController.Instance.IsMobileInput)
        {
            MenuController.Instance.MobileMovementJoystick = MobileMovementJoystick;
            MenuController.Instance.MobileGrenadeJoystick = MobileGrenadeJoystick;
            MenuController.Instance.MobileReloadButton = MobileReloadButton;
            MenuController.Instance.MobileTacticalAbilityButton = MobileTacticalAbilityButton;
            MenuController.Instance.MobileSwitchWeaponsButton = MobileSwitchWeaponsButton;
            MenuController.Instance.MobileTouchBackgroundFire = MobileTouchBackgroundFire;

            SprintThreshold = MobileMovementJoystick.transform.Find("SprintThresholdMask").GetChild(0).GetComponent<Image>();
        }
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
    /// Atualiza as informações na tela do jogo atual.
    /// </summary>
    private void UpdateInGameUI()
    {
        PlayerMoneyText.text = Player.Data.Money.ToString("N2");
        WaveScoreText.text = WavesManager.Instance.CurrentWave.Stats != null ? WavesManager.Instance.CurrentWave.Stats.Score.ToString("N0") : "";

        MagazineBulletsText.text = Player.CurrentWeapon.MagazineBullets.ToString();
        TotalBulletsText.text = Player.Backpack.GetAmmo(Player.CurrentWeapon.BulletType).ToString();
        if (Player.Backpack.EquippedThrowableType != ThrowableTypes.None)
            ThrowablesCountText.text = Player.Backpack.EquippedThrowable?.Count.ToString();

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
            WeaponTypes.ScarDebug => ScarDebugSprite,
            _ => null,
        };

        ActiveWeaponImage.sprite = GetWeaponSprite(Player.CurrentWeapon.Type);

        var switchType = Player.Backpack.CurrentWeaponIndex == 0 ? Player.Backpack.EquippedSecondaryType : Player.Backpack.EquippedPrimaryType;
        SwitchWeaponImage.transform.parent.gameObject.SetActive(switchType != WeaponTypes.None);
        SwitchWeaponImage.sprite = GetWeaponSprite(switchType);
    }

    /// <summary>
    /// Troca para a cena da loja após o tutorial concluído.
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
    /// Inicia a próxima wave.
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
    /// Abre o painel de opções.
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
    /// <param name="position">A posição do texto a ser exibido.</param>
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
