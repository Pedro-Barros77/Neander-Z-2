using UnityEngine;

/// <summary>
/// Objetos que podem ser alvos do inimigo (player, torretas etc).
/// </summary>
public interface IEnemyTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public void TakeDamage(float damage);
}