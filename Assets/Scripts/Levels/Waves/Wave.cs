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
    public float P1Score { get; private set; }
    public float P1Money { get; private set; }
    public float P1TotalKills { get; private set; }
    public float P1HeadshotKills { get; private set; }
    public float P1Precision { get; private set; }
    public int P1AttacksCount { get; private set; }
    public int P1AttacksHit { get; private set; }
    public bool HasStarted { get; private set; }

    public float FloorHeight => LevelData.BottomRightSpawnLimit.y;
    float LeftBoundary => LevelData.TopLeftSpawnLimit.x;
    float RightBoundary => LevelData.BottomRightSpawnLimit.x;

    public Transform EnemiesContainer { get; private set; }
    LevelData LevelData;
    Coroutine EnemySpawner;

    void Start()
    {
        LevelData = GameObject.Find("Environment").GetComponent<LevelData>();
        EnemiesContainer = GameObject.Find("EnemiesContainer").transform;
    }

    void Update()
    {
        if (!HasStarted)
            return;

        EnemiesAlive = EnemiesAlive.Where(x => x != null && x.IsAlive).ToList();

        if (Input.GetKeyDown(KeyCode.End))
            KillAllWave();

        if (EnemiesAlive.Count <= Data.MinEnemiesAlive && SpawnCount > 0)
        {
            int diff = Data.MinEnemiesAlive - EnemiesAlive.Count;
            SpawnMultipleEnemies(diff);
        }

        if (SpawnCount >= TotalEnemiesCount && EnemiesAlive.Count == 0 && !IsFinished)
        {
            StopCoroutine(EnemySpawner);
            StartCoroutine(EndWaveDelayed());
        }
    }

    /// <summary>
    /// Inicia a wave.
    /// </summary>
    public void StartWave()
    {
        TotalEnemiesCount = Data.EnemyGroups.Sum(x => x.Count);
        HasStarted = true;
        EnemySpawner = StartCoroutine(EnemiesSpawner());
    }

    /// <summary>
    /// Atualiza as pontua��es do jogador ao eliminar um inimigo.
    /// </summary>
    /// <param name="enemy">O inimigo eliminado.</param>
    /// <param name="attacker">O jogador que matou o inimigo.</param>
    /// <param name="headshotKill">Se a morte foi feita com um tiro na cabe�a.</param>
    public void HandleScore(BaseEnemy enemy, IEnemyTarget attacker, bool headshotKill = false)
    {
        float newScore = enemy.KillScore;
        if (headshotKill)
            newScore *= enemy.HeadshotScoreMultiplier;

        P1Score += newScore;
        P1TotalKills++;
        if (headshotKill)
            P1HeadshotKills++;
    }

    /// <summary>
    /// Atualiza a precis�o do jogador ao atacar ou acertar um ataque.
    /// </summary>
    /// <param name="count">A quantidade de ataques/tiros disparados.</param>
    /// <param name="hitCount">A quantidade de ataques/tiros acertados.</param>
    public void HandlePlayerAttack(int count, int hitCount)
    {
        P1AttacksCount += count;
        P1AttacksHit += hitCount;
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
    }

    /// <summary>
    /// Spawna X inimigos aleat�rios.
    /// </summary>
    /// <param name="count">N�mero de inimigos a serem spawnados.</param>
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
    /// <param name="position">A posi��o para spawnar.</param>
    /// <param name="parent">O pai/container do transform do inimigo.</param>
    /// <returns>O script BaseEnemy da inst�ncia do inimigo spawnado.</returns>
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
    /// Retorna uma posi��o aleat�ria dentro dos limites do mapa.
    /// </summary>
    /// <param name="hideFromCamera">Se a posi��o deve ser fora da �rea vis�vel da c�mera.</param>
    /// <returns>Um valor aleat�rio do X da posi��o.</returns>
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
    /// Seleciona um grupo de inimigos aleat�rio.
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
    /// <returns>O script BaseEnemy da inst�ncia do inimigo spawnado.</returns>
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
    /// Termina a wave ap�s aguardar o delay em MS.
    /// </summary>
    private IEnumerator EndWaveDelayed()
    {
        IsFinished = true;
        yield return new WaitForSeconds(Data.EndDelayMs / 1000);

        P1Money = (P1Score / 4) * Data.MoneyMultiplier;
        P1Precision = P1AttacksHit * 100f / P1AttacksCount;
        WavesManager.Instance.ShowWaveSummary();
    }

    public void KillAllWave()
    {
        SpawnMultipleEnemies(TotalEnemiesCount - SpawnCount);

        EnemiesAlive.ForEach(x => x.Die("", null));
        EnemiesAlive.Clear();

        P1AttacksCount++;
    }
}