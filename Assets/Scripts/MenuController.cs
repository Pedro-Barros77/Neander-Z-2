using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    static MenuController _instance;

    /// <summary>
    /// A instância deste Singleton.
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

    [SerializeField]
    Sprite ArrowCursor, PointerCursor;

    public Image GameCursor;
    Vector3 CursorPointOffset = Vector3.zero;
    public FPSCount FpsCount;

    private GameObject PersistentCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PersistentCanvas = GameObject.Find("PersistentCanvas");

        if (_instance == null || _instance == this)
        {
            _instance = this;
            gameObject.tag = "Original";
            if (PersistentCanvas != null)
                PersistentCanvas.tag = "Original";
        }

        var fpsObj = GameObject.Find("GameFPS");
        if (fpsObj != null)
            FpsCount = fpsObj.GetComponent<FPSCount>();

        if (PersistentCanvas != null)
            DontDestroyOnLoad(PersistentCanvas);
    }

    void Start()
    {
    }

    void Update()
    {
        if (GameCursor != null)
        {
            Cursor.visible = false;
            GameCursor.transform.position = Input.mousePosition + CursorPointOffset;
        }
        else
            Cursor.visible = true;
        if (Input.GetKeyDown(KeyCode.U))
        {
            FpsCount.gameObject.SetActive(!FpsCount.gameObject.activeSelf);
        }
    }

    /// <summary>
    /// Troca o cursor do jogo.
    /// </summary>
    /// <param name="cursor">O tipo de cursor a ser definido.</param>
    public void SetCursor(Cursors cursor)
    {
        if (GameCursor == null)
            return;

        float cursorLocalScale = GameCursor.rectTransform.localScale.x;
        switch (cursor)
        {
            case Cursors.Arrow:
                GameCursor.sprite = ArrowCursor;
                CursorPointOffset = Vector3.zero;
                break;
            case Cursors.Pointer:
                GameCursor.sprite = PointerCursor;
                CursorPointOffset = new Vector3(-(8 * cursorLocalScale), 0, 0);
                break;
        }
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
    /// <summary>
    /// Tela de game over, congela o tempo.
    /// </summary>
    public void ShowGameOverPanel()
    {
        Time.timeScale = 0;
        IsGamePaused = true;
    }
}
