using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public WaveData Data { get; set; }
    public int SpawnCount { get; private set; }
    public bool IsFinished { get; private set; }
    public int TotalEnemiesCount { get; set; }
    public bool HasMoreSpawns => SpawnCount < TotalEnemiesCount;
    public List<BaseEnemy> EnemiesAlive { get; private set; } = new List<BaseEnemy>();

    public float FloorHeight => LevelData.BottomRightSpawnLimit.y;
    float LeftBoundary => LevelData.TopLeftSpawnLimit.x;
    float RightBoundary => LevelData.BottomRightSpawnLimit.x;
    bool FinishedSpawner;

    LevelData LevelData;
    public Transform EnemiesContainer { get; private set; }

    void Start()
    {
        Debug.Log(Data.Description);
        LevelData = GameObject.Find("Environment").GetComponent<LevelData>();
        EnemiesContainer = GameObject.Find("EnemiesContainer").transform;
        TotalEnemiesCount = Data.EnemyGroups.Sum(x => x.Count);
    }

    void Update()
    {
        EnemiesAlive = EnemiesAlive.Where(x => x != null && x.IsAlive).ToList();

        if (Input.GetKeyDown(KeyCode.I))
            SpawnEnemy(EnemyTypes.Z_Roger, new Vector3(GetRandomXPosition(), FloorHeight, 0), EnemiesContainer);

        if (EnemiesAlive.Count <= Data.MinEnemiesAlive && SpawnCount > 0)
        {
            int diff = Data.MinEnemiesAlive - EnemiesAlive.Count;
            SpawnMultipleEnemies(diff);
        }

        if (FinishedSpawner && EnemiesAlive.Count == 0 && !IsFinished)
            StartCoroutine(EndWaveDelayed());
    }

    /// <summary>
    /// Inicia a wave.
    /// </summary>
    public void StartWave()
    {
        Debug.Log("Starting wave " + Data.Number);

        StartCoroutine(EnemiesSpawner());
    }

    /// <summary>
    /// Timer que spawna inimigos de tempos em tempos.
    /// </summary>
    private IEnumerator EnemiesSpawner()
    {
        yield return new WaitForSeconds(Data.StartDelayMs / 1000);

        while (HasMoreSpawns)
        {
            int randomCount = Random.Range(Data.MinSpawnCount, Data.MaxSpawnCount);
            int toSpawn = Mathf.Clamp(randomCount, 0, Data.MaxEnemiesAlive - EnemiesAlive.Count);

            SpawnMultipleEnemies(toSpawn);

            float randomDelay = Random.Range(Data.MinSpawnDelayMs, Data.MaxSpawnDelayMs);

            yield return new WaitForSeconds(randomDelay / 1000);
        }

        FinishedSpawner = true;
    }

    /// <summary>
    /// Spawna X inimigos aleatórios.
    /// </summary>
    /// <param name="count">Número de inimigos a serem spawnados.</param>
    private void SpawnMultipleEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!HasMoreSpawns)
                break;
            EnemyGroup randomGroup = ChooseEnemyGroup();
            BaseEnemy enemy = SpawnEnemyFromGroup(randomGroup);
        }
    }

    /// <summary>
    /// Spawna um inimigo.
    /// </summary>
    /// <param name="type">O tipo de inimigo.</param>
    /// <param name="position">A posição para spawnar.</param>
    /// <param name="parent">O pai/container do transform do inimigo.</param>
    /// <returns>O script BaseEnemy da instância do inimigo spawnado.</returns>
    public BaseEnemy SpawnEnemy(EnemyTypes type, Vector3 position, Transform parent)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Enemies/{type}");
        GameObject instance = Instantiate(prefab, position, Quaternion.identity, parent);
        BaseEnemy enemy = instance.GetComponent<BaseEnemy>();
        EnemiesAlive.Add(enemy);
        SpawnCount++;
        return enemy;
    }

    /// <summary>
    /// Retorna uma posição aleatória dentro dos limites do mapa.
    /// </summary>
    /// <param name="hideFromCamera">Se a posição deve ser fora da área visível da câmera.</param>
    /// <returns>Um valor aleatório do X da posição.</returns>
    public float GetRandomXPosition(bool hideFromCamera = true)
    {
        if (hideFromCamera)
        {
            float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float cameraLeftLimit = -cameraWidth;
            float cameraRightLimit = cameraWidth;

            float leftX = Random.Range(LeftBoundary, cameraLeftLimit);
            float rightX = Random.Range(cameraRightLimit, RightBoundary);

            return Random.value < 0.5 ? leftX : rightX;
        }

        return Random.Range(LeftBoundary, RightBoundary);
    }

    /// <summary>
    /// Seleciona um grupo de inimigos aleatório.
    /// </summary>
    /// <returns>O grupo de inimigos selecionado.</returns>
    private EnemyGroup ChooseEnemyGroup()
    {
        if (Data.EnemyGroups.Count == 0)
        {
            Debug.LogWarning("No Enemy Group Found");
            return null;
        }

        float weightSum = Data.EnemyGroups.Sum(x => x.SpawnChanceMultiplier);
        var randomWeight = Random.Range(0f, weightSum);

        float processedWeight = 0;
        for (int i = 0; i < Data.EnemyGroups.Count; i++)
        {
            processedWeight += Data.EnemyGroups[i].SpawnChanceMultiplier;
            if (randomWeight <= processedWeight)
                return Data.EnemyGroups[i];
        }

        return ChooseEnemyGroup();
    }

    /// <summary>
    /// Spawna um inimigo de um grupo, preenchendo seus valores.
    /// </summary>
    /// <param name="group">O grupo que o inimigo pertence.</param>
    /// <returns>O script BaseEnemy da instância do inimigo spawnado.</returns>
    private BaseEnemy SpawnEnemyFromGroup(EnemyGroup group)
    {
        if (group.Count <= 0) return null;

        if (!group.IsInfinite)
        {
            group.Count--;
            if (group.Count <= 0)
                Data.EnemyGroups.Remove(group);
        }

        BaseEnemy enemy = SpawnEnemy(group.EnemyType, new Vector3(GetRandomXPosition(), FloorHeight, 0), EnemiesContainer);

        float health = Random.Range(group.MinHealth, group.MaxHealth);
        float speed = Random.Range(group.MinSpeed, group.MaxSpeed);
        float damage = Random.Range(group.MinDamage, group.MaxDamage);

        enemy.SetRandomValues(health, speed, damage);

        return enemy;
    }

    /// <summary>
    /// Termina a wave após aguardar o delay em MS.
    /// </summary>
    private IEnumerator EndWaveDelayed()
    {
        Debug.Log("Waiting end delay");
        yield return new WaitForSeconds(Data.EndDelayMs / 1000);

        IsFinished = true;

        Debug.Log("Ending wave " + Data.Number);
    }
}
