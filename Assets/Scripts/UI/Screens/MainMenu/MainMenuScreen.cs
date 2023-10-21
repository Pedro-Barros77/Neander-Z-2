using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    [SerializeField]
    Image KeyboardMouseImage, MobileImage;

    Color SelectedInputColor = new Color32(70, 230, 130, 255);
    Color UnselectedInputColor = new Color32(230, 230, 230, 255);

    void Start()
    {
        MenuController.Instance.SetCursor(Cursors.Arrow);
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
    }

    /// <summary>
    /// Inicia o modo de jogo "Survival".
    /// </summary>
    public void StartSurvivalMode()
    {
        MenuController.Instance.ChangeScene(SceneNames.Graveyard, LoadSceneMode.Single);
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
