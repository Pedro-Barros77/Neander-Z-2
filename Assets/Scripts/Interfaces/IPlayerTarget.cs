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
    /// <summary>
    /// Fun��o chamada ao ser atingido em um ponto espec�fico.
    /// </summary>
    /// <param name="hitPoint">A posi��o do acerto.</param>
    /// <param name="pointToDirection">A dire��o em que os efeitos devem ser lan�ados.</param>
    void OnPointHit(Vector3 hitPoint, Vector3 pointToDirection, string bodyPartName);
}