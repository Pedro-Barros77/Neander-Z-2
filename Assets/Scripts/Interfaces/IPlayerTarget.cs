using UnityEngine;

/// <summary>
/// Objetos que podem ser alvos do player (inimigos).
/// </summary>
public interface IPlayerTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public bool IsAlive { get; }
    public void TakeDamage(TakeDamageProps props);
    public void GetHealth(float health);
    /// <summary>
    /// Muda a cor do material desse inimigo.
    /// </summary>
    /// <param name="color">A cor destino.</param>
    void HandleSpriteColorChange(Color32 color);
}