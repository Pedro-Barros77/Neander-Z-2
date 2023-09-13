using System.Collections.Generic;
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

    LevelData LevelData;

    /// <summary>
    /// Objetos alvos dos inimigos (player, torretas etc).
    /// </summary>
    public List<IEnemyTarget> EnemiesTargets { get; set; } = new();

    void Start()
    {
        LevelData = GameObject.Find("Environment").GetComponent<LevelData>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SpawnRogerTest(EnemyTypes.Z_Roger);
        if (Input.GetKeyDown(KeyCode.X))
            SpawnRogerTest(EnemyTypes.Z_Robert);
        if (Input.GetKeyDown(KeyCode.V))
            SpawnRogerTest(EnemyTypes.Z_Ronald);
        if (Input.GetKeyDown(KeyCode.B))
            SpawnRogerTest(EnemyTypes.Z_Ronaldo);
        if (Input.GetKeyDown(KeyCode.N))
            SpawnRogerTest(EnemyTypes.Z_Raven);
        if (Input.GetKeyDown(KeyCode.M))
            SpawnRogerTest(EnemyTypes.Z_Raimundo);
    }

    void SpawnRogerTest(EnemyTypes type)
    {
        float y = LevelData.BottomRightSpawnLimit.y;
        float randonX = Random.Range(LevelData.TopLeftSpawnLimit.x, LevelData.BottomRightSpawnLimit.x);
        Vector3 spawnPosition = new Vector3(randonX, y, 0);

        GameObject enemy = Instantiate(Resources.Load<GameObject>($"Prefabs/Enemies/{type}"), spawnPosition, Quaternion.identity);
    }
}
