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

    [SerializeField]
    Player Player;
    [SerializeField]
    private GameObject PausePanel;
    [SerializeField]
    private Image ActiveWeaponImage, ActiveAmmoImage;
    [SerializeField]
    private TextMeshProUGUI MagazineBulletsText, TotalBulletsText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (IsGamePaused)
                ContinueGame();
            else
                PauseGame();
        }

        UpdateInGameUI();
    }

    /// <summary>
    /// Pausa o jogo, congelando o tempo e exibindo o menu de pausa.
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
        IsGamePaused = true;
        PausePanel.SetActive(true);
    }

    /// <summary>
    /// Continua o jogo, descongelando o tempo e escondendo o menu de pausa.
    /// </summary>
    public void ContinueGame()
    {
        Time.timeScale = 1;
        IsGamePaused = false;
        PausePanel.SetActive(false);
    }

    /// <summary>
    /// Reinicia o jogo, recarregando a cena atual.
    /// </summary>
    public void RestartGame()
    {
        ContinueGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Sai do jogo, voltando para o menu principal.
    /// </summary>
    public void QuitGame()
    {
        ContinueGame();
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
    /// Atualiza as informa��es na tela do jogo atual.
    /// </summary>
    private void UpdateInGameUI()
    {
        MagazineBulletsText.text = Player.CurrentWeapon.MagazineBullets.ToString();
        TotalBulletsText.text = Player.Backpack.GetAmmo(Player.CurrentWeapon.BulletType).ToString();
    }
}