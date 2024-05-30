using UnityEngine;

public interface IBurnable
{
    public void ActiveBurningParticles(BurningEffect burnFx);
    public void DeactivateFireParticles();

    /// <summary>
    /// Muda a cor do material desse inimigo.
    /// </summary>
    /// <param name="color">A cor destino.</param>
    void HandleSpriteColorChange(Color32 color);
}
