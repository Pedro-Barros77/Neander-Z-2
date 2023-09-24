using UnityEngine;

/// <summary>
/// Objetos que podem receber knockback.
/// </summary>
public interface IKnockBackable
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public Rigidbody2D RigidBody { get; }
    public void TakeKnockBack(float pushForce, Vector3 direction);
}