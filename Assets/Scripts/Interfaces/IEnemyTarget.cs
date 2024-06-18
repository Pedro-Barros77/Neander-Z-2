using UnityEngine;

/// <summary>
/// Objetos que podem ser alvos do inimigo (player, torretas etc).
/// </summary>
public interface IEnemyTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public bool IsAlive { get; }
    public Bounds Bounds { get; }
    public Rigidbody2D RigidBody { get; }
    public void HandleEnemyKill(BaseEnemy enemy, string lastDamagedBodyPartName);
    public void TakeDamage(TakeDamageProps props);
    public void GetHealth(float value);
    /// <summary>
    /// Muda a cor do material desse player.
    /// </summary>
    /// <param name="color">A cor destino.</param>
    void HandleSpriteColorChange(Color32 color);
}