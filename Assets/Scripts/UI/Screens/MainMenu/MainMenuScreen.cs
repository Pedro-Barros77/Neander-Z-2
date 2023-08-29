using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScreen : MonoBehaviour
{
    void Start()
    {
        
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
