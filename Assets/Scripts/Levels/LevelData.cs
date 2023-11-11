using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField]
    public Vector2 TopLeftSpawnLimit, BottomRightSpawnLimit, LeftMapBoundary, RightMapBoundary;
    [SerializeField]
    public bool InitEnemyTargets;
    [SerializeField]
    public Player Player;

    private void Start()
    {
        MenuController.Instance.IsInGame = true;
        if (InitEnemyTargets)
            WavesManager.Instance.EnemiesTargets.Add(Player);
    }
}
