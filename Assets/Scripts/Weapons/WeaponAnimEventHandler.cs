using UnityEngine;

public class WeaponAnimEventHandler : MonoBehaviour
{
    /// <summary>
    /// A arma que este gerenciador de eventos de animação está associado.
    /// </summary>
    private BaseWeapon weapon;
    void Start()
    {
        weapon = transform.parent.GetComponent<BaseWeapon>();
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame de recarregamento da arma.
    /// </summary>
    void OnReloadEnd()
    {
        weapon.OnReloadEnd();
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no frame de recarregamento da arma em que o carregador é posicionado.
    /// </summary>
    void OnReloadedChamber()
    {
        weapon.OnReloadedChamber();
    }
}
