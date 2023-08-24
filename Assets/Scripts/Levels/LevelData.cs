using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [SerializeField]
    public Vector2 TopLeftSpawnLimit, BottomRightSpawnLimit;

    private void Start()
    {
        MenuController.Instance.IsInGame = true;
    }
}
