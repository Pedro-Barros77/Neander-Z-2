using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField]
    Image KeyboardMouseImage, MobileImage;

    [SerializeField]
    GameObject OptionsContent, MainMenuContent, SelectGameModeContent;

    OptionsPanel OptionsPanel;
    AudioSource AudioSource;
    float musicStartVolume;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        musicStartVolume = AudioSource.volume;
        MenuController.Instance.SetCursor(Cursors.Arrow);
        OptionsPanel = OptionsContent.GetComponent<OptionsPanel>();
        OptionsPanel.GoBackFunction = () =>
        {
            OptionsContent.SetActive(false);
            MainMenuContent.SetActive(true);
        };
    }

    void Update()
    {
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
    /// Navega para a tela de seleção de modo de jogo.
    /// </summary>
    public void PlayGame()
    {
        SelectGameModeContent.SetActive(true);
        MainMenuContent.SetActive(false);
    }

    /// <summary>
    /// Abre o painel de opções.
    /// </summary>
    public void OpenOptions()
    {
        OptionsContent.SetActive(true);
        MainMenuContent.SetActive(false);
        OptionsPanel.Open();
    }

    /// <summary>
    /// Fecha o jogo.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetInputMode(int mode)
    {
        MenuController.Instance.IsMobileInput = mode != 0;
    }
}
