using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField]
    Image KeyboardMouseImage, MobileImage;

    [SerializeField]
    GameObject OptionsContent, MainMenuContent;

    OptionsPanel OptionsPanel;
    Color SelectedInputColor = new Color32(70, 230, 130, 255);
    Color UnselectedInputColor = new Color32(230, 230, 230, 255);
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
            MobileImage.color = SelectedInputColor;
            KeyboardMouseImage.color = UnselectedInputColor;
        }
        else
        {
            MobileImage.color = UnselectedInputColor;
            KeyboardMouseImage.color = SelectedInputColor;
        }

        AudioSource.volume = musicStartVolume * MenuController.Instance.MusicVolume;
    }

    /// <summary>
    /// Inicia o modo de jogo "Survival".
    /// </summary>
    public void StartSurvivalMode()
    {
        MenuController.Instance.ChangeScene(SceneNames.Graveyard, LoadSceneMode.Single);
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
