using UnityEngine;

/// <summary>
/// Objetos que podem ser alvos do inimigo (player, torretas etc).
/// </summary>
public interface IEnemyTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public bool IsAlive { get; }
    public SpriteRenderer SpriteRenderer { get; }
    public Rigidbody2D RigidBody { get; }
    public void TakeDamage(float damage, float headshotMultiplier, string bodyPartName, IPlayerTarget attacker, Vector3? hitPosition = null, bool selfDamage = false);
    public void GetHealth(float value);
    void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection, string bodyPartName);
}