using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameScreen : MonoBehaviour
{
    [SerializeField]
    Player Player;
    [SerializeField]
    private GameObject PausePanel, GameOverPanel;
    [SerializeField]
    private Image ActiveWeaponImage, ActiveAmmoImage, ActiveThrowableImage;
    [SerializeField]
    private TextMeshProUGUI MagazineBulletsText, TotalBulletsText, ThrowablesCountText, PlayerMoneyText, WaveScoreText;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon;
    [SerializeField]
    Sprite Colt_1911Sprite, ShortBarrelSprite, UZISprite, SV98Sprite, M16Sprite, RPGSprite, MacheteSprite, DeagleSprite, Beretta_93RSprite, ScarSprite, ScarDebugSprite;
    [SerializeField]
    Sprite FragGrenadeSprite, MolotovSprite;

    void Start()
    {
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)) && Player.IsAlive && !WavesManager.Instance.WaveSummaryPanel.activeSelf)
        {
            if (MenuController.Instance.IsGamePaused)
                ContinueGame();
            else
                PauseGame();
        }

        UpdateInGameUI();
    }

    /// <summary>
    /// Atualiza as informações na tela do jogo atual.
    /// </summary>
    private void UpdateInGameUI()
    {
        PlayerMoneyText.text = Player.Data.Money.ToString("N2");
        WaveScoreText.text = WavesManager.Instance.CurrentWave.P1Score.ToString("N0");

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
            _ => null,
        };

        if (Player.Backpack.EquippedThrowableType != ThrowableTypes.None)
            ActiveThrowableImage.sprite = Player.Backpack.EquippedThrowableType switch
            {
                ThrowableTypes.FragGrenade => FragGrenadeSprite,
                ThrowableTypes.Molotov => MolotovSprite,
                _ => null,
            };

        ActiveWeaponImage.sprite = Player.CurrentWeapon.Type switch
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
            WeaponTypes.ScarDebug => ScarDebugSprite,
            _ => null,
        };
    }

    /// <summary>
    /// Change the scene to the game store.
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
        MenuController.Instance.PauseGame();
        PausePanel.SetActive(true);
    }

    /// <summary>
    /// Continua o jogo, descongelando o tempo e escondendo o menu de pausa.
    /// </summary>
    public void ContinueGame()
    {
        MenuController.Instance.ContinueGame();
        PausePanel.SetActive(false);
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
        OpenStore();
        MenuController.Instance.OnRestartGame();
    }

    /// <summary>
    /// Sai do jogo, voltando para o menu principal.
    /// </summary>
    public void QuitGame()
    {
        ContinueGame();
        MenuController.Instance.IsInGame = false;
        GameOverPanel.SetActive(false);
        MenuController.Instance.ChangeScene(SceneNames.MainMenu, LoadSceneMode.Single);
    }

    /// <summary>
    /// Mostra o painel de fim de jogo.
    /// </summary>
    public void ShowGameOverPanel()
    {
        MenuController.Instance.ShowGameOverPanel();
        GameOverPanel.SetActive(true);
    }
}
