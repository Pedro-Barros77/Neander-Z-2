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

    /// <summary>
    /// Objetos alvos dos inimigos (player, torretas etc).
    /// </summary>
    public List<IEnemyTarget> EnemiesTargets { get; set; } = new();

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
