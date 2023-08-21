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
    /// <summary>
    /// Função chamada ao ser atingido em um ponto específico.
    /// </summary>
    /// <param name="hitPoint">A posição do acerto.</param>
    /// <param name="pointToDirection">A direção em que os efeitos devem ser lançados.</param>
    void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection);
}