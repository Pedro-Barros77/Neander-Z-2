using System;
using System.Collections.Generic;
using UnityEngine;

public class SupportAnimEventHandler : MonoBehaviour
{
    /// <summary>
    /// A arma que este gerenciador de eventos de anima��o est� associado.
    /// </summary>
    private BaseSupportEquipment support;

    void Start()
    {
        support = transform.parent.GetComponent<BaseSupportEquipment>();
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, ao utilizar o equipamento.
    /// </summary>
    void OnTrigger()
    {
        support.OnTrigger();
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, ao terminar de utilizar o equipamento.
    /// </summary>
    void OnTriggerEnd()
    {
        support.OnTriggerEnd();
    }
}
