using System;
using System.Collections.Generic;
using UnityEngine;

public class SupportAnimEventHandler : MonoBehaviour
{
    /// <summary>
    /// A arma que este gerenciador de eventos de animação está associado.
    /// </summary>
    private BaseSupportEquipment support;

    void Start()
    {
        support = transform.parent.GetComponent<BaseSupportEquipment>();
    }

    /// <summary>
    /// Função chamada pelo evento de animação, ao utilizar o equipamento.
    /// </summary>
    void OnTrigger()
    {
        support.OnTrigger();
    }

    /// <summary>
    /// Função chamada pelo evento de animação, ao terminar de utilizar o equipamento.
    /// </summary>
    void OnTriggerEnd()
    {
        support.OnTriggerEnd();
    }
}
