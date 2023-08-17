using UnityEngine;

public class WeaponAnimEventHandler : MonoBehaviour
{
    /// <summary>
    /// A arma que este gerenciador de eventos de anima��o est� associado.
    /// </summary>
    private BaseWeapon weapon;
    void Start()
    {
        weapon = transform.parent.GetComponent<BaseWeapon>();
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, no �ltimo frame de recarregamento da arma.
    /// </summary>
    void OnReloadEnd()
    {
        weapon.OnReloadEnd();
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, no frame de recarregamento da arma em que o carregador � posicionado.
    /// </summary>
    void OnReloadedChamber()
    {
        weapon.OnReloadedChamber();
    }
}
