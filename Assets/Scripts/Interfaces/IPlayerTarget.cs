using UnityEngine;

/// <summary>
/// Objetos que podem ser alvos do player (inimigos).
/// </summary>
public interface IPlayerTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public bool IsAlive { get; }
    public void TakeDamage(float damage, string bodyPartName);
}