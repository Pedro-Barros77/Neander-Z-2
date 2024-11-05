using System;
using System.Collections.Generic;
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
    /// Fun��o chamada pelo evento de anima��o, no �ltimo frame de tiro da arma.
    /// </summary>
    void OnShootEnd()
    {
        weapon.OnShootEnd();
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

    /// <summary>
    /// Executa um efeito sonoro extra da arma, para anima��es de recarregamento, tiro, etc.
    /// </summary>
    /// <param name="index">O �ndice do som a ser tocado da lista.</param>
    void PlayExtraSoundEffect(int index)
    {
        weapon.PlayExtraSoundEffect(index);
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, no �ltimo frame de pump da arma.
    /// </summary>
    void OnPumpEnd()
    {
        weapon.OnPumpEnd();
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o durante o reload, para spawnar o pente/carregador da arma.
    /// </summary>
    void SpawnMagDrop()
    {
        weapon.SpawnMagDrop();
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o durante o tiro, para spawnar o cartucho/c�psula da bala.
    /// </summary>
    void SpawnCartridgeDrop()
    {
        weapon.SpawnCartridgeDrop();
    }
}
