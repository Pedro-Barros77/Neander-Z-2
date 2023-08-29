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
    }

    void SpawnRogerTest(EnemyTypes type)
    {
        float randonX = Random.Range(LevelData.TopLeftSpawnLimit.x, LevelData.BottomRightSpawnLimit.x);
        Vector3 spawnPosition = new Vector3(randonX, LevelData.BottomRightSpawnLimit.y, 0);

        GameObject roger = Instantiate(Resources.Load<GameObject>($"Prefabs/Enemies/{type}"), spawnPosition, Quaternion.identity);
    }
}
