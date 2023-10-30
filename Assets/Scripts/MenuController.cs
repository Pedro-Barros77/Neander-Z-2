using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    /// <summary>
    /// A instância deste Singleton.
    /// </summary>
    public static MenuController Instance { get; private set; }

    public bool IsGamePaused { get; private set; }
    public bool IsInGame { get; set; }
    public bool IsTutorialActive { get; set; }
    public bool IsMobileInput { get; set; }
    public float MusicVolume { get; set; }
    public float UIVolume { get; set; }
    public float PlayerVolume { get; set; }
    public float EnemiesVolume { get; set; }
    public FPSCount FpsCount { get; private set; }
    public Image GameCursor { get; private set; }
    public delegate void OnRestart();
    /// <summary>
    /// Evento chamado quando a cena é reiniciada.
    /// </summary>
    public static event OnRestart RestartEvent;
    public KeybindConfig Keybind;

    public Joystick MobileMovementJoystick { get; set; }
    public Joystick MobileGrenadeJoystick { get; set; }
    public BaseButton MobileReloadButton { get; set; }
    public BaseButton MobileTacticalAbilityButton { get; set; }
    public BaseButton MobileSwitchWeaponsButton { get; set; }
    public BaseButton MobileTouchBackgroundFire { get; set; }

    [SerializeField]
    Sprite ArrowCursor, PointerCursor;

    Vector3 CursorPointOffset = Vector3.zero;

    private GameObject PersistentCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PersistentCanvas = GameObject.Find("PersistentCanvas");

        if (Instance == null)
        {
            Instance = GameObject.Find("MenuController").GetComponent<MenuController>();
            //IsTutorialActive = true;
            MusicVolume = 1;
            UIVolume = 1;
            PlayerVolume = 1;
            EnemiesVolume = 1;
        }

        if (Instance == this)
        {
            gameObject.tag = "Original";
            if (PersistentCanvas != null)
                PersistentCanvas.tag = "Original";

            static void OnSceneLoaded(Scene scene, LoadSceneMode loadMode) => SceneManager.SetActiveScene(scene);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        var fpsObj = GameObject.Find("GameFPS");
        if (fpsObj != null)
            FpsCount = fpsObj.GetComponent<FPSCount>();

        if (PersistentCanvas != null)
        {
            GameCursor = PersistentCanvas.transform.Find("GameCursor").GetComponent<Image>();
            DontDestroyOnLoad(PersistentCanvas);
        }
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

        if (Constants.EnableDevKeybinds)
        {
            if (Input.GetKeyDown(KeyCode.U))
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

        if (IsMobileInput)
        {
            GameCursor.enabled = false;
            return;
        }

        GameCursor.enabled = true;

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

    /// <summary>
    /// Função chamada quando o botão de reiniciar o jogo é pressionado.
    /// </summary>
    public void OnRestartGame()
    {
        RestartEvent?.Invoke();
        Keybind = Resources.Load<KeybindConfig>("ScriptableObjects/Player/DefaultKeybind");
    }
}
