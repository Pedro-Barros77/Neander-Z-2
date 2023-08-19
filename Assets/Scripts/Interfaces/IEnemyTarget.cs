using UnityEngine;

public interface IEnemyTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
}