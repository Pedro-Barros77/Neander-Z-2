using System;
using System.Collections;
using UnityEngine;

public abstract class BaseSupportEquipment : MonoBehaviour
{
    public Player Player { get; set; }
    public float PlayerFlipDir { get; set; }
    public BaseSupportEquipmentData Data;
    public SupportEquipmentTypes Type => Data.Type;
    protected SpriteRenderer HandSpriteRenderer;
    protected Action OnFinishedUsing;
    protected bool Started;
    protected float StartXLocalPosition, StartXLocalScale;
    protected bool isHandColorSet;

    protected virtual void Awake()
    {
        var sprite = transform.Find("Sprite");
        HandSpriteRenderer = sprite.transform.Find("Hand").GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if (Started)
            return;
        StartXLocalPosition = transform.localPosition.x;
        StartXLocalScale = transform.localScale.x;
        Started = true;
    }

    protected virtual void Update()
    {
        if (!Started)
            return;

        transform.localScale = transform.localScale.WithX(PlayerFlipDir * StartXLocalScale);
        transform.localPosition = transform.localPosition.WithX(PlayerFlipDir * StartXLocalPosition);
    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, ao utilizar o equipamento.
    /// </summary>
    public virtual void OnTrigger()
    {

    }

    /// <summary>
    /// Fun��o chamada pelo evento de anima��o, ao terminar de utilizar o equipamento.
    /// </summary>
    public virtual void OnTriggerEnd()
    {

    }

    /// <summary>
    /// Inicia o uso do equipamento.
    /// </summary>
    /// <param name="onFinishedUsing"></param>
    public virtual void Use(Action onFinishedUsing)
    {
        if (!Started)
            Start();
        OnFinishedUsing = onFinishedUsing;
    }

    /// <summary>
    /// Define a cor de pele da m�o do jogador neste equipamento.
    /// </summary>
    /// <param name="color">A cor de pele a ser definida na m�o.</param>
    public virtual void SetHandSkinColor(Color32 color)
    {
        if (isHandColorSet)
            return;

        if (HandSpriteRenderer != null)
        {
            HandSpriteRenderer.color = color;
            isHandColorSet = true;
        }
    }
}
