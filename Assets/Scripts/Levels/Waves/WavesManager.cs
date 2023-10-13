using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    static WavesManager _instance;

    /// <summary>
    /// A instância deste Singleton.
    /// </summary>
    public static WavesManager Instance
    {
        get
        {
            if (_instance == null) _instance = GameObject.Find("WavesManager").GetComponent<WavesManager>();
            return _instance;
        }
    }

    public int WaveNumber;
    public TextMeshProUGUI WaveStartTitleText, WaveStartDescriptionText, WaveTitleText, WaveEnemiesCountText, SummaryTitle, P1WaveScoreValue, P1EarnedMoneyValue, P1TotalKillsValue, P1HeadshotKillsValue, P1PrecisionValue;
    public GameObject WaveSummaryPanel;
    public Player Player;

    public Wave CurrentWave { get; private set; }

    BlinkingText WaveStartTitleContainer, WaveStartDescriptionContainer;
    Coroutine DeactivateStartPanelRoutine;

    /// <summary>
    /// Objetos alvos dos inimigos (player, torretas etc).
    /// </summary>
    public List<IEnemyTarget> EnemiesTargets { get; set; } = new();

    private void Awake()
    {
        CurrentWave = GetComponent<Wave>();
        WaveNumber = Player.Data.CurrentWaveIndex;
        LoadWaveData();
    }

    void Start()
    {
        WaveStartTitleContainer = WaveStartTitleText.GetComponentInParent<BlinkingText>(true);
        WaveStartDescriptionContainer = WaveStartDescriptionText.GetComponentInParent<BlinkingText>(true);

        StartWave();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SpawnEnemyTest(EnemyTypes.Z_Roger);
        if (Input.GetKeyDown(KeyCode.X))
            SpawnEnemyTest(EnemyTypes.Z_Robert);
        if (Input.GetKeyDown(KeyCode.V))
            SpawnEnemyTest(EnemyTypes.Z_Ronald);
        if (Input.GetKeyDown(KeyCode.B))
            SpawnEnemyTest(EnemyTypes.Z_Ronaldo);
        if (Input.GetKeyDown(KeyCode.N))
            SpawnEnemyTest(EnemyTypes.Z_Raven);
        if (Input.GetKeyDown(KeyCode.M))
            SpawnEnemyTest(EnemyTypes.Z_Raimundo);
        if (Input.GetKeyDown(KeyCode.L))
            SpawnEnemyTest(EnemyTypes.Z_Rui);

        WaveEnemiesCountText.text = $"{CurrentWave.SpawnCount - CurrentWave.EnemiesAlive.Count}/{(CurrentWave.Data.IsBossWave ? "∞" : CurrentWave.TotalEnemiesCount)}";
    }

    /// <summary>
    /// Inicia a próxima Wave.
    /// </summary>
    public void StartNextWave()
    {
        WaveNumber++;
        LoadWaveData();
        StartWave();
    }

    /// <summary>
    /// Carrega os dados para a Wave atual.
    /// </summary>
    void LoadWaveData()
    {
        Destroy(CurrentWave);
        CurrentWave = gameObject.AddComponent<Wave>();

        var SO = Resources.Load<WaveData>($"ScriptableObjects/Waves/Wave_{WaveNumber:D2}");

        if (SO != null)
        {
            SO.UnloadSO();
            CurrentWave.Data = Resources.Load<WaveData>($"ScriptableObjects/Waves/Wave_{WaveNumber:D2}");
        }
        else
        {
            Debug.LogWarning("Acabaram as waves. Iniciando da primeira");
            WaveNumber = 1;
            LoadWaveData();
        }
    }

    /// <summary>
    /// Inicia a Wave atual.
    /// </summary>
    void StartWave()
    {
        if (DeactivateStartPanelRoutine != null)
            StopCoroutine(DeactivateStartPanelRoutine);
        WaveSummaryPanel.SetActive(false);
        WaveStartTitleContainer.transform.parent.gameObject.SetActive(true);
        WaveStartTitleText.text = CurrentWave.Data.Title.ValueOrDefault($"Wave {WaveNumber}");
        WaveStartDescriptionText.text = CurrentWave.Data.Description;

        bool hasDescription = !WaveStartDescriptionText.text.IsNullOrEmpty();
        WaveStartDescriptionContainer.gameObject.SetActive(hasDescription);
        WaveStartTitleContainer.RestartBlinking();
        if (hasDescription)
            WaveStartDescriptionContainer.RestartBlinking();

        WaveTitleText.text = WaveStartTitleText.text;

        CurrentWave.StartWave();

        DeactivateStartPanelRoutine = StartCoroutine(DeactivateWavePanel());
    }

    /// <summary>
    /// Exibe o painel de resumo da wave na tela.
    /// </summary>
    public void ShowWaveSummary()
    {
        MenuController.Instance.PauseGame();
        WaveSummaryPanel.SetActive(true);
        SummaryTitle.text = $"Survived {CurrentWave.Data.Title.ValueOrDefault($"Wave {WaveNumber}")}";
        P1WaveScoreValue.text = CurrentWave.P1Score.ToString();
        P1EarnedMoneyValue.text = $"$ {CurrentWave.P1Money:N2}";
        P1TotalKillsValue.text = CurrentWave.P1TotalKills.ToString();
        P1HeadshotKillsValue.text = CurrentWave.P1HeadshotKills.ToString();
        P1PrecisionValue.text = $"{CurrentWave.P1Precision:N1} %";
        Player.Data.CurrentWaveIndex = WaveNumber + 1;
        Player.Data.GetMoney(CurrentWave.P1Money);
    }

    /// <summary>
    /// Desativa o painel de WaveStart após 5 segundos.
    /// </summary>
    IEnumerator DeactivateWavePanel()
    {
        yield return new WaitForSeconds(5);
        WaveStartTitleContainer.transform.parent.gameObject.SetActive(false);
    }

    void SpawnEnemyTest(EnemyTypes type)
    {
        BaseEnemy enemy = CurrentWave.SpawnEnemy(type, new Vector3(CurrentWave.GetRandomXPosition(), CurrentWave.FloorHeight, 0), CurrentWave.EnemiesContainer);
    }
}
