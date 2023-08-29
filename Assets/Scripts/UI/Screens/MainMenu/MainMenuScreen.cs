using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    void Start()
    {
        MenuController.Instance.SetCursor(Cursors.Arrow);
    }

    void Update()
    {
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
}
