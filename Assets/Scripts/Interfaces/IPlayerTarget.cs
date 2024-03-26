using UnityEngine;

/// <summary>
/// Objetos que podem ser alvos do player (inimigos).
/// </summary>
public interface IPlayerTarget
{
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public bool IsAlive { get; }
    public void TakeDamage(float damage, float headshotMultiplier, string bodyPartName, IEnemyTarget attacker, Vector3? hitPosition = null);
    public void GetHealth(float health);
    /// <summary>
    /// Função chamada ao ser atingido em um ponto específico.
    /// </summary>
    /// <param name="hitPoint">A posição do acerto.</param>
    /// <param name="pointToDirection">A direção em que os efeitos devem ser lançados.</param>
    void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection, string bodyPartName);
    /// <summary>
    /// Muda a cor do material desse inimigo.
    /// </summary>
    /// <param name="color">A cor destino.</param>
    void HandleSpriteColorChange(Color32 color);
}