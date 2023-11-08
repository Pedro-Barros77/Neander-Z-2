using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public MenuScreens ActiveScreen { get; private set; } = MenuScreens.MainMenu;
    public delegate void OnScreenChanged(MenuScreens newScreen, MenuScreens previousScreen);
    public OnScreenChanged OnScreenChangedEvent { get; set; }

    [SerializeField]
    Image KeyboardMouseImage, MobileImage;

    [SerializeField]
    GameObject MainMenuContent, OptionsContent, SelectGameModeContent, SelectSaveContent;

    OptionsPanel OptionsPanel;
    AudioSource AudioSource;
    float musicStartVolume;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        musicStartVolume = AudioSource.volume;
        MenuController.Instance.SetCursor(Cursors.Arrow);
        OptionsPanel = OptionsContent.GetComponent<OptionsPanel>();
        OptionsPanel.GoBackFunction = () => OpenScreen(MenuScreens.MainMenu);
    }

    void Update()
    {
        if (ActiveScreen != MenuScreens.MainMenu)
            return;

        if (MenuController.Instance.IsMobileInput)
        {
            MobileImage.color = Constants.Colors.SelectedOptionColor;
            KeyboardMouseImage.color = Constants.Colors.UnselectedOptionColor;
        }
        else
        {
            MobileImage.color = Constants.Colors.UnselectedOptionColor;
            KeyboardMouseImage.color = Constants.Colors.SelectedOptionColor;
        }

        AudioSource.volume = musicStartVolume * MenuController.Instance.MusicVolume;
    }

    /// <summary>
    /// Exibe a tela especificada e esconde todas as outras.
    /// </summary>
    /// <param name="screen">A nova tela a ser aberta.</param>
    public void OpenScreen(MenuScreens screen)
    {
        var prevScreen = ActiveScreen;
        ActiveScreen = screen;

        MainMenuContent.SetActive(false);
        OptionsContent.SetActive(false);
        SelectGameModeContent.SetActive(false);
        SelectSaveContent.SetActive(false);

        switch (screen)
        {
            case MenuScreens.MainMenu:
                MainMenuContent.SetActive(true);
                break;
            case MenuScreens.SelectGameMode:
                SelectGameModeContent.SetActive(true);
                break;
            case MenuScreens.SelectSave:
                SelectSaveContent.SetActive(true);
                break;
            case MenuScreens.Options:
                OptionsContent.SetActive(true);
                OptionsPanel.Open();
                break;
        }

        OnScreenChangedEvent?.Invoke(screen, prevScreen);
    }

    /// <summary>
    /// Navega para a tela de seleção de modo de jogo.
    /// </summary>
    public void PlayGame()
    {
        OpenScreen(MenuScreens.SelectGameMode);
    }

    /// <summary>
    /// Abre o painel de opções.
    /// </summary>
    public void OpenOptions()
    {
        OpenScreen(MenuScreens.Options);
        OptionsPanel.Open();
    }

    /// <summary>
    /// Fecha o jogo.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Seleciona o modo de input atual
    /// </summary>
    /// <param name="mode">O index do modo de input.</param>
    public void SetInputMode(int mode)
    {
        MenuController.Instance.IsMobileInput = mode != 0;
    }
}
