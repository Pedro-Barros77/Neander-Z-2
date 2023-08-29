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
    private Image ActiveWeaponImage, ActiveAmmoImage;
    [SerializeField]
    private TextMeshProUGUI MagazineBulletsText, TotalBulletsText;
    [SerializeField]
    Sprite PistolBulletIcon, ShotgunBulletIcon, RifleAmmoIcon, SniperAmmoIcon, RocketAmmoIcon, MeleeAmmoIcon;
    [SerializeField]
    Sprite Colt_1911Sprite, ShortBarrelSprite;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && Player.IsAlive)
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
        MagazineBulletsText.text = Player.CurrentWeapon.MagazineBullets.ToString();
        TotalBulletsText.text = Player.Backpack.GetAmmo(Player.CurrentWeapon.BulletType).ToString();

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

        ActiveWeaponImage.sprite = Player.CurrentWeapon.Type switch
        {
            WeaponTypes.Colt_1911 => Colt_1911Sprite,
            WeaponTypes.ShortBarrel => ShortBarrelSprite,
            _ => null,
        };
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
    /// Reinicia o jogo, recarregando a cena atual.
    /// </summary>
    public void RestartGame()
    {
        ContinueGame();
        GameOverPanel.SetActive(false);
        MenuController.Instance.RestartScene();
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
