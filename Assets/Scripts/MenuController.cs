using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    static MenuController _instance;

    /// <summary>
    /// A inst�ncia deste Singleton.
    /// </summary>
    public static MenuController Instance
    {
        get
        {
            if (_instance == null) _instance = GameObject.Find("MenuController").GetComponent<MenuController>();
            return _instance;
        }
    }

    public bool IsGamePaused { get; set; }
    public bool IsInGame { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    /// <summary>
    /// Troca a cena atual para a cena especificada.
    /// </summary>
    /// <param name="scene">O nome da nova cena.</param>
    /// <param name="mode">O modo de carregamento.</param>
    public void ChangeScene(SceneNames scene, LoadSceneMode mode)
    {
        SceneManager.LoadScene(scene.ToString(), mode);
    }

    /// <summary>
    /// Reinicia a cena ativa.
    /// </summary>
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Reinicia o estado e anima��o do bot�o.
    /// </summary>
    /// <param name="btnAnimator">Bot�o a ser reiniciado.</param>
    public void ResetButton(Animator btnAnimator)
    {
        btnAnimator.ResetTrigger("Highlighted");
        btnAnimator.ResetTrigger("Pressed");
        btnAnimator.ResetTrigger("Selected");
        btnAnimator.SetTrigger("Normal");
        btnAnimator.transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Pausa o jogo, congelando o tempo.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
        IsGamePaused = true;
    }

    /// <summary>
    /// Continua o jogo, descongelando o tempo.
    /// </summary>
    public void ContinueGame()
    {
        Time.timeScale = 1;
        IsGamePaused = false;
    }
}
