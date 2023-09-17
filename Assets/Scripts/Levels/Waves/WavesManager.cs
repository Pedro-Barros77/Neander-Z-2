using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public TextMeshProUGUI WaveStartTitleText, WaveStartDescriptionText, WaveTitleText, WaveEnemiesCountText;

    public Wave CurrentWave { get; private set; }

    LevelData LevelData;
    BlinkingText WaveStartTitleContainer, WaveStartDescriptionContainer;

    /// <summary>
    /// Objetos alvos dos inimigos (player, torretas etc).
    /// </summary>
    public List<IEnemyTarget> EnemiesTargets { get; set; } = new();

    private void Awake()
    {
        CurrentWave = GetComponent<Wave>();
        LoadWaveData();
    }

    void Start()
    {
        WaveStartTitleContainer = WaveStartTitleText.GetComponentInParent<BlinkingText>(true);
        WaveStartDescriptionContainer = WaveStartDescriptionText.GetComponentInParent<BlinkingText>(true);

        LevelData = GameObject.Find("Environment").GetComponent<LevelData>();
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

        WaveEnemiesCountText.text = $"{CurrentWave.SpawnCount - CurrentWave.EnemiesAlive.Count}/{CurrentWave.TotalEnemiesCount}";
    }

    /// <summary>
    /// Inicia a próxima Wave.
    /// </summary>
    void StartNextWave()
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
        CurrentWave.Data = Resources.Load<WaveData>($"ScriptableObjects/Waves/Wave_{WaveNumber:D2}");
    }

    /// <summary>
    /// Inicia a Wave atual.
    /// </summary>
    void StartWave()
    {
        WaveStartTitleContainer.transform.parent.gameObject.SetActive(true);
        WaveStartTitleContainer.RestartBlinking();
        WaveStartDescriptionContainer.RestartBlinking();

        WaveStartTitleText.text = CurrentWave.Data.Title.ValueOrDefault($"Wave {WaveNumber}");
        WaveStartDescriptionText.text = CurrentWave.Data.Description;
        bool hasDescription = !WaveStartDescriptionText.text.IsNullOrEmpty();
        WaveStartDescriptionContainer.gameObject.SetActive(hasDescription);

        WaveTitleText.text = WaveStartTitleText.text;

        CurrentWave.StartWave();

        StartCoroutine(DeactivateWavePanel());
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
