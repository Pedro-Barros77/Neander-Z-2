using System;
using System.Collections.Generic;
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
    /// Função chamada pelo evento de animação, no último frame de tiro da arma.
    /// </summary>
    void OnShootEnd()
    {
        weapon.OnShootEnd();
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

    /// <summary>
    /// Executa um efeito sonoro extra da arma, para animações de recarregamento, tiro, etc.
    /// </summary>
    /// <param name="index">O índice do som a ser tocado da lista.</param>
    void PlayExtraSoundEffect(int index)
    {
        weapon.PlayExtraSoundEffect(index);
    }

    /// <summary>
    /// Função chamada pelo evento de animação, no último frame de pump da arma.
    /// </summary>
    void OnPumpEnd()
    {
        weapon.OnPumpEnd();
    }

    /// <summary>
    /// Função chamada pelo evento de animação durante o reload, para spawnar o pente/carregador da arma.
    /// </summary>
    void SpawnMagDrop()
    {
        weapon.SpawnMagDrop();
    }

    /// <summary>
    /// Função chamada pelo evento de animação durante o tiro, para spawnar o cartucho/cápsula da bala.
    /// </summary>
    void SpawnCartridgeDrop()
    {
        weapon.SpawnCartridgeDrop();
    }
}
