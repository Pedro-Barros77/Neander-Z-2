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
}
