using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public WaveData Data { get; set; }
    public WaveStats Stats { get; private set; }
    public int SpawnCount { get; private set; }
    public bool IsFinished { get; private set; }
    public int TotalEnemiesCount { get; set; }
    public bool HasMoreSpawns => SpawnCount - InfiniteGroupKills < TotalEnemiesCount;
    public List<BaseEnemy> EnemiesAlive { get; private set; } = new List<BaseEnemy>();
    public int P1AttacksCount { get; private set; }
    public int P1AttacksHit { get; private set; }
    public bool HasStarted { get; private set; }
    public float StartTime { get; private set; }
    public float EndTime { get; private set; }

    public float FloorHeight => LevelData.BottomRightSpawnLimit.y;
    float LeftBoundary => LevelData.TopLeftSpawnLimit.x;
    float RightBoundary => LevelData.BottomRightSpawnLimit.x;
    int InfiniteGroupKills;

    public Transform EnemiesContainer { get; private set; }
    LevelData LevelData;
    Coroutine EnemySpawner;
    EnemyGroup BossGroup;

    /// <summary>
    /// Distância mínima entre os zumbis, antes de serem considerados sobrepostos.
    /// </summary>
    readonly float ClusteringMinDistance = 0.6f;
    /// <summary>
    /// Número máximo de zumbis sobrepostos, antes de começar a repelir.
    /// </summary>
    readonly int ClusteringMaxZombies = 5;
    /// <summary>
    /// Força em que os zumbis sobrepostos se repelem.
    /// </summary>
    readonly float ClusteringRepulsionForce = 80f;
    LayerMask EnemiesLayerMask;

    void Start()
    {
        LevelData = GameObject.Find("Environment").GetComponent<LevelData>();
        EnemiesContainer = GameObject.Find("EnemiesContainer").transform;
        EnemiesLayerMask = LayerMask.GetMask("Enemy");
    }

    void Update()
    {
        if (!HasStarted)
            return;

        EnemiesAlive = EnemiesAlive.Where(x => x != null && x.IsAlive).ToList();

        if (EnemiesAlive.Count <= Data.MinEnemiesAlive && SpawnCount > 0)
        {
            int diff = Data.MinEnemiesAlive - EnemiesAlive.Count;
            SpawnMultipleEnemies(diff);
        }

        if (SpawnCount - InfiniteGroupKills >= TotalEnemiesCount && EnemiesAlive.Count == 0 && !IsFinished)
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
        var stat = SavesManager.UpdateWaveStats(Data.Number, started: true);
        stat.InputMode = MenuController.Instance.IsMobileInput ? 1 : 0;
        Stats = Resources.Load<WaveStats>($"ScriptableObjects/Waves/Stats/{Data.Number:D2}");
        Stats.Started = true;
        Stats.RestartCount = stat.RestartCount;

        TotalEnemiesCount = Data.EnemyGroups.Sum(x => x.Count);
        if (Data.IsBossWave)
            BossGroup = Data.EnemyGroups[Data.BossGroupIndex];
        HasStarted = true;
        EnemySpawner = StartCoroutine(EnemiesSpawner());
        StartCoroutine(ClusteringChecker());
        StartTime = Time.time;
    }

    /// <summary>
    /// Atualiza as pontuações do jogador ao eliminar um inimigo.
    /// </summary>
    /// <param name="enemy">O inimigo eliminado.</param>
    /// <param name="attacker">O jogador que matou o inimigo.</param>
    /// <param name="headshotKill">Se a morte foi feita com um tiro na cabeça.</param>
    public void HandleScore(BaseEnemy enemy, IEnemyTarget attacker, bool headshotKill = false)
    {
        float newScore = enemy.KillScore;
        if (headshotKill)
            newScore *= enemy.HeadshotScoreMultiplier;

        Stats.Score += newScore;
        Stats.EnemiesKilled++;
        if (headshotKill)
            Stats.HeadshotKills++;

        if (enemy.IsBoss && !EnemiesAlive.Where(x => x.IsAlive).Any(x => x.IsBoss))
        {
            Data.EnemyGroups.Remove(BossGroup);
            OnBossDeath();
        }
    }

    /// <summary>
    /// Atualiza a precisão do jogador ao atacar ou acertar um ataque.
    /// </summary>
    /// <param name="count">A quantidade de ataques/tiros disparados.</param>
    /// <param name="hitCount">A quantidade de ataques/tiros acertados.</param>
    public void HandlePlayerAttack(int count, int hitCount)
    {
        P1AttacksCount += count;
        P1AttacksHit += hitCount;
    }

    /// <summary>
    /// Função chamada quando o ultimo boss do BossGroup da wave é morto.
    /// </summary>
    private void OnBossDeath()
    {
        foreach (EnemyGroup group in Data.EnemyGroups)
            group.IsInfinite = false;

        Data.IsBossWave = false;
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
            float cameraWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
            float cameraLeft = Camera.main.transform.position.x - (cameraWidth / 2);
            float cameraLeftLimit = cameraLeft;
            float cameraRightLimit = cameraLeft + cameraWidth;

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
        var groups = Data.EnemyGroups.Where(x => !x.IsDisabled).ToList();

        if (!groups.Any())
            return null;

        float weightSum = groups.Sum(x => x.SpawnChanceMultiplier);
        var randomWeight = Random.Range(0f, weightSum);

        float processedWeight = 0;
        for (int i = 0; i < groups.Count; i++)
        {
            processedWeight += groups[i].SpawnChanceMultiplier;
            if (randomWeight <= processedWeight)
                return groups[i];
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
        if (group == null || group.Count <= 0) return null;

        if (group.IsInfinite)
            InfiniteGroupKills++;
        else
        {
            group.Count--;
            if (group.Count <= 0 && group != BossGroup)
                Data.EnemyGroups.Remove(group);
        }

        BaseEnemy enemy = SpawnEnemy(group.EnemyType, new Vector3(GetRandomXPosition(), FloorHeight, 0), EnemiesContainer);

        float health = Random.Range(group.MinHealth, group.MaxHealth);
        float speed = Random.Range(group.MinSpeed, group.MaxSpeed);
        float damage = Random.Range(group.MinDamage, group.MaxDamage);
        int killscore = Random.Range(group.MinKillScore, group.MaxKillScore);

        enemy.OnStartFinished += () => enemy.SetRandomValues(health, speed, damage, killscore, enemy, group == BossGroup);

        return enemy;
    }

    /// <summary>
    /// Termina a wave após aguardar o delay em MS.
    /// </summary>
    private IEnumerator EndWaveDelayed()
    {
        IsFinished = true;
        yield return new WaitForSeconds(Data.EndDelayMs / 1000);

        EndTime = Time.time;

        float attackHitRatio = (float)P1AttacksHit / P1AttacksCount;
        Stats.MoneyEarned = (Stats.Score / 4) * Data.MoneyMultiplier * (1 + (attackHitRatio / 2));
        Stats.Precision = attackHitRatio * 100;

        Stats.WaveNumber = Data.Number;
        Stats.Completed = true;
        Stats.TimeTaken = EndTime - StartTime;

        WavesManager.Instance.ShowWaveSummary();
    }

    private IEnumerator ClusteringChecker()
    {
        while (true)
        {
            CheckClustering();

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Verifica quais zumbis estão sobrepostos acima do limite e repulsa um dos outros.
    /// </summary>
    private void CheckClustering()
    {
        foreach (BaseEnemy enemy in EnemiesAlive)
        {
            var colliders = Physics2D.OverlapCircleAll(enemy.transform.position, ClusteringMinDistance, EnemiesLayerMask);
            colliders = colliders.Where(x => x.gameObject != enemy.gameObject && x.gameObject.name.StartsWith("Z_") && x.GetComponentInParent<BaseEnemy>().IsAlive).ToArray();

            if (colliders.Length < ClusteringMaxZombies)
                continue;

            foreach (Collider2D collider in colliders)
            {
                float enemy1X = enemy.transform.position.x;
                float enemy2X = collider.transform.position.x;
                if (enemy1X == enemy2X)
                    enemy2X += Random.Range(-0.001f, 0.001f);
                float repulsionDirection = Mathf.Sign(enemy1X - enemy2X);

                enemy.RigidBody.AddForce(new Vector3(repulsionDirection, 0) * ClusteringRepulsionForce, ForceMode2D.Force);
                collider.GetComponent<Rigidbody2D>().AddForce(new Vector3(-(repulsionDirection / 2), 0) * ClusteringRepulsionForce, ForceMode2D.Force);
            }
        }
    }

    public void KillAllWave()
    {
        SpawnMultipleEnemies(TotalEnemiesCount - (SpawnCount - InfiniteGroupKills));
        KillAllAlive();
    }

    public void KillAllAlive()
    {
        EnemiesAlive.ForEach(x => x.Die("", null));
        EnemiesAlive.Clear();

        P1AttacksCount++;
    }

    public void CenterEnemies()
    {
        foreach (var enemy in EnemiesAlive)
        {
            enemy.transform.position = new Vector3(0, 3);
        }
    }
}
