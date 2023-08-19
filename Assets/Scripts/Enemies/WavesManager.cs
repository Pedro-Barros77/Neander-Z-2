using System.Collections;
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

    public List<IEnemyTarget> EnemiesTargets { get; set; } = new();

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
