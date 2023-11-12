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

        WaveEnemiesCountText.transform.parent.gameObject.SetActive(!MenuController.Instance.IsTutorialActive);
        if (!MenuController.Instance.IsTutorialActive)
            StartWave();
    }

    void Update()
    {
        if (Constants.EnableDevKeybinds)
        {
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRoger))
                SpawnEnemyTest(EnemyTypes.Z_Roger);
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRobert))
                SpawnEnemyTest(EnemyTypes.Z_Robert);
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRonald))
                SpawnEnemyTest(EnemyTypes.Z_Ronald);
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRonaldo))
                SpawnEnemyTest(EnemyTypes.Z_Ronaldo);
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRaven))
                SpawnEnemyTest(EnemyTypes.Z_Raven);
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRaimundo))
                SpawnEnemyTest(EnemyTypes.Z_Raimundo);
            if (Constants.GetActionDown(InputActions.DEBUG_SpawnRUI))
                SpawnEnemyTest(EnemyTypes.Z_Rui);
            if (Constants.GetActionDown(InputActions.DEBUG_KillAllEnemiesAlive))
                CurrentWave.KillAllAlive();
            if (Constants.GetActionDown(InputActions.DEBUG_EndWave))
                CurrentWave.KillAllWave();
            if (Constants.GetActionDown(InputActions.DEBUG_CenterEnemies))
                CurrentWave.CenterEnemies();
        }

        WaveEnemiesCountText.text = $"{Mathf.Clamp(CurrentWave.SpawnCount - CurrentWave.InfiniteEnemiesKilled - CurrentWave.EnemiesAlive.Count, 0, int.MaxValue)}/{(CurrentWave.Data.IsBossWave ? "∞" : CurrentWave.TotalEnemiesCount)}";
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

        string path = $"ScriptableObjects/Waves/Wave_{WaveNumber:D2}";
        var SO = Resources.Load<WaveData>(path);

        if (SO != null)
        {
            SO.UnloadSO();
            CurrentWave.Data = Resources.Load<WaveData>(path);
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
        P1WaveScoreValue.text = CurrentWave.Stats.Score.ToString("N0");
        P1EarnedMoneyValue.text = $"$ {CurrentWave.Stats.MoneyEarned:N2}";
        int enemiesKilled = CurrentWave.Stats.EnemiesKilled;
        int headshotKills = CurrentWave.Stats.HeadshotKills;
        int headshotPrecision = 0;
        if (headshotKills + enemiesKilled > 0)
            headshotPrecision = (int)((float)headshotKills / enemiesKilled * 100);
        P1TotalKillsValue.text = enemiesKilled.ToString();
        P1HeadshotKillsValue.text = $"{headshotKills} ({headshotPrecision:N0}%)";

        P1PrecisionValue.text = $"{CurrentWave.Stats.Precision:N1} %";
        Player.Data.CurrentWaveIndex = WaveNumber + 1;
        Player.Data.GetMoney(CurrentWave.Stats.MoneyEarned);
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
